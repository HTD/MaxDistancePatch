using System.Collections.Generic;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Describes basic file system node properties.
    /// </summary>
    public interface INode : IItem {

        /// <summary>
        /// Gets the root node of the tree.
        /// </summary>
        INode Root { get; }

        /// <summary>
        /// Gets the parent node of this node.
        /// </summary>
        INode Parent { get; }

        /// <summary>
        /// Gets all direct descendants of this node.
        /// </summary>
        IEnumerable<INode> Children { get; }

        /// <summary>
        /// Gets the depth in tree structure.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Gets the path relative to root node path.
        /// </summary>
        string RelativePath { get; }

    }

}