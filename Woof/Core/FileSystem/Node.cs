using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// File system virtual node.
    /// </summary>
    public class Node : INode {

        #region Properties

        #region Direct

        /// <summary>
        /// Gets the root node reference for tree nodes.
        /// </summary>
        public INode Root { get; }

        /// <summary>
        /// Gets the parent node, null for root nodes.
        /// </summary>
        public INode Parent { get; }

        /// <summary>
        /// Child node collection.
        /// </summary>
        /// <remarks>
        /// Can be read from cache if set.
        /// </remarks>
        public IEnumerable<INode> Children {
            get {
                if (ChildrenCache == null) return FileSystem.GetChildItems(Path).Select(i => new Node(this, i));
                else return ChildrenCache;
            }
            private set {
                ChildrenCache = value.ToArray();
            }
        }

        /// <summary>
        /// Gets node depth inside directory tree.
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Gets file system used to create the node.
        /// </summary>
        public IProvider FileSystem { get; }

        /// <summary>
        /// Gets the file or directory name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the file system path of the file or directory the node represents.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets the relative path of the file or directory the node represents.
        /// </summary>
        public string RelativePath { get; private set; }

        /// <summary>
        /// Gets the size of the file (or directory, if <see cref="GetSizes()"/> was called)
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets the file or directory last write date and time.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets the file or directory attributes.
        /// </summary>
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets optional item type information.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets a flag indicating whether this node (and all its children) has the size calculated.
        /// </summary>
        /// <remarks>
        /// This flag also indicates the node children are cached.
        /// </remarks>
        public bool IsComplete { get; private set; }

        #endregion

        #region Derived

        /// <summary>
        /// Gets a flag indicating whether this node represents a file.
        /// </summary>
        public bool IsFile => (Attributes & FileAttributes.Directory) == 0;

        /// <summary>
        /// Gets a flag indicating whether this node represents a directory.
        /// </summary>
        public bool IsDirectory => (Attributes & FileAttributes.Directory) != 0;

        /// <summary>
        /// Gets a flag indicating whether this node is a root node.
        /// </summary>
        public bool IsRoot => Parent == null;

        /// <summary>
        /// Gets all file nodes from direct descendants.
        /// </summary>
        public IEnumerable<IItem> Files => Children.Where(i => i.IsFile);

        /// <summary>
        /// Gets all directory nodes from direct descendants.
        /// </summary>
        public IEnumerable<INode> Directories => Children.Where(i => i.IsDirectory);

        /// <summary>
        /// Gets all descendant nodes.
        /// </summary>
        public IEnumerable<INode> Descendants => Traverse().Skip(1);

        /// <summary>
        /// Gets all descendant file nodes.
        /// </summary>
        public IEnumerable<IItem> DescendantFiles => Descendants.Where(n => n.IsFile);

        /// <summary>
        /// Gets all descendant directory nodes.
        /// </summary>
        public IEnumerable<INode> DescendantDirectiories => Descendants.Where(n => n.IsDirectory);

        /// <summary>
        /// Gets the total number of nodes in a complete tree.
        /// </summary>
        public int TotalCount => Descendants.Count();

        /// <summary>
        /// Gets the total number of file nodes in a complete tree.
        /// </summary>
        public int TotalFileCount => Descendants.Count(n => n.IsFile);

        /// <summary>
        /// Gets the total number of directory nodes in a complete tree.
        /// </summary>
        public int TotalDirCount => Descendants.Count(n => n.IsDirectory);

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new file system node.
        /// </summary>
        /// <param name="path">Local file system path.</param>
        public Node(string path) : this(LocalFS.Instance, path) { }

        /// <summary>
        /// Creates new file system node of a file system specified.
        /// </summary>
        /// <param name="fileSystem">File system provider.</param>
        /// <param name="path">A path within the file system.</param>
        public Node(IProvider fileSystem, string path) {
            FileSystem = fileSystem;
            FileSystem.NormalizePath(ref path);
            Path = path;
            var item = FileSystem.GetItem(path);
            if (item == null) return;
            RelativeOffset = Path.Length + 1;
            Root = this;
            Attributes = item.Attributes;
            Date = item.Date;
        }

        /// <summary>
        /// Creates a new file system node clone.
        /// </summary>
        /// <param name="node">Node to be cloned.</param>
        public Node(INode node) {
            FileSystem = node.FileSystem;
            Root = node.Root;
            Parent = node.Parent;
            Children = node.Children;
            Depth = node.Depth;
            Name = node.Name;
            Path = node.Path;
            RelativePath = node.RelativePath;
            Size = node.Size;
            Date = node.Date;
            Attributes = node.Attributes;
            Type = node.Type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Enumerates a complete collection of all tree nodes, including root.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<INode> Traverse() {
            INode node = null;
            var stack = new Stack<INode>(InitialStackSize);
            stack.Push(this);
            while (stack.Any()) {
                node = stack.Pop();
                yield return node;
                foreach (var child in node.Children) stack.Push(child);
            }
        }

        /// <summary>
        /// Gets the size of each directory for the entire file system tree.
        /// </summary>
        /// <remarks>
        /// Optimized for speed vs memory.
        /// </remarks>
        /// <returns>This node with all sizes calculated and all content cached.</returns>
        public Node GetSizes() {
            // find all leaves as initial end nodes:
            var stack = new Stack<INode>(InitialStackSize);
            stack.Push(this);
            var endNodes = new Stack<INode>(InitialStackSize);
            Node node = null;
            while (stack.Any()) { // full file system scan is the most expensive part of this operation.
                node = (Node)stack.Pop();
                node.Children = node.Children; // caching is necessary for sizes calculation!
                if ((node.Children as INode[]).Length == 0) endNodes.Push(node);
                foreach (var child in node.Children) stack.Push(child);
            }
            // calculate sizes in all current end nodes depth first:
            bool isPossibleEndNode;
            long size;
            while (endNodes.Any()) {
                node = (Node)endNodes.Pop();
                if (!node.IsComplete) { // if size for this end node is not yet calculated...
                    node.Size += node.Children.Sum(i => i.Size); // update it's size from child items,
                    node.IsComplete = true; // mark this node to omit subsequent size calculations.
                }
                if (node.Parent == null) break; // when size calculated at root, this is it.
                node = (Node)node.Parent; // go towards the root
                // if a node contains only nodes with size already calculated, it becomes new end node:
                isPossibleEndNode = true;
                size = 0;
                foreach (var x in node.Children) {
                    // if sizes in children calculated, add them to current node size, if 1 size is missing give up, this is not an end node.
                    if ((x as Node).IsComplete) size += x.Size;
                    else { isPossibleEndNode = false; break; }
                }
                // if new end node: update its size and push
                if (isPossibleEndNode && !node.IsComplete) {
                    node.Size += size;
                    node.IsComplete = true;
                    endNodes.Push(node);
                }
            }
            IsComplete = true;
            return this;
        }

        #endregion

        #region Non-public

        /// <summary>
        /// Creates a new child node from <see cref="IItem"/>.
        /// </summary>
        /// <param name="parent">A parent node for this item.</param>
        /// <param name="item">File system item to upgrade to a node.</param>
        private Node(INode parent, IItem item) {
            FileSystem = item.FileSystem;
            Name = item.Name;
            Path = item.Path;
            RelativePath = item.Path.Substring((parent.Root as Node).RelativeOffset);
            Date = item.Date;
            Size = item.Size;
            Attributes = item.Attributes;
            Type = item.Type;
            Root = parent.Root;
            Parent = parent;
            Depth = parent.Depth + 1;
        }

        const int InitialStackSize = 1024;
        private readonly int RelativeOffset;
        INode[] ChildrenCache;

        #endregion

    }

}