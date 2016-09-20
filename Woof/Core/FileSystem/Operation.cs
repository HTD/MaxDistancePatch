using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Complex, automatic file system operation.
    /// </summary>
    public class Operation  {

        /// <summary>
        /// Gets the object which rises events for each reporeted progress change.
        /// </summary>
        public Progress Progress { get; } = new Progress();

        /// <summary>
        /// Gets deferred items list. If an item can not be processed it will be added here.
        /// </summary>
        public List<IItem> Deferred { get; } = new List<IItem>();

        /// <summary>
        /// Gets a list of errors which occured during the operation.
        /// </summary>
        public List<Error> Errors { get; } = new List<Error>();

        /// <summary>
        /// Gets or sets a flag indicating whether operation progress should be reported.
        /// </summary>
        public bool ReportProgress { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether operation should calculate total size and report byte count progress.
        /// </summary>
        public bool CalculateSize { get; set; }

        /// <summary>
        /// Deletes a file system item with all optional subitems if applicable.
        /// </summary>
        /// <param name="item">File or directory item.</param>
        /// <param name="force">Set true to remove read-only items.</param>
        /// <returns>True if all items were removed.</returns>
        private bool Delete(IItem item, bool force = false) {
            ProgressEventArgs currentProgress = null;
            if (item.FileSystem.IsReadOnly || !item.FileSystem.CanRemoveItems) return false;
            if (!item.FileSystem.Exists(item.Path)) return false;
            if (item.IsFile) {
                try {
                    return item.FileSystem.RemoveItem(item.Path, force);
                }
                catch (Exception x) {
                    Errors.Add(new Error(item.Path, x.Message));
                    return false;
                }
            }
            if (item.IsDirectory) {
                var node = item.GetNode();
                bool success = true;
                if (ReportProgress) {
                    currentProgress = new ProgressEventArgs(node.TotalCount, CalculateSize ? node.DescendantFiles.Sum(i => i.Size) : 0);
                }
                foreach (var leaf in node.Descendants) {
                    try {
                        if (leaf.FileSystem.RemoveItem(leaf.Path, force)) {
                            if (ReportProgress) {
                                currentProgress.Item++;
                                if (CalculateSize) currentProgress.Bytes += leaf.Size;
                                (Progress as IProgress<ProgressEventArgs>).Report(currentProgress);
                            }
                        }
                        else Deferred.Add(leaf);
                    }
                    catch (Exception x) {
                        success = false;
                        Deferred.Add(leaf);
                        Errors.Add(new Error(leaf.Path, x.Message));
                    }
                }
                try {
                    node.FileSystem.RemoveItem(node.Path, force);
                } catch (Exception x) {
                    success = false;
                    Deferred.Add(node);
                    Errors.Add(new Error(node.Path, x.Message));
                }
                return success;
            }
            return false;
        }

        /// <summary>
        /// Deteltes items from depth-first items collection.
        /// This operation WILL FAIL if collection is not ordered depth-first.
        /// </summary>
        /// <param name="items">Depth-first file system items collection.</param>
        /// <param name="force">Set true to remove read-only items.</param>
        /// <returns>True if all items were removed.</returns>
        private bool Delete(IEnumerable<IItem> items, bool force = false) {
            ProgressEventArgs currentProgress = null;
            if (ReportProgress) {
                currentProgress = new ProgressEventArgs(items.Count(), CalculateSize ? items.Sum(i => i.Size) : 0);
            }
            bool success = true;
            foreach (var item in items) {
                try {
                    if (item.FileSystem.RemoveItem(item.Path, force)) {
                        if (ReportProgress) {
                            currentProgress.Item++;
                            if (CalculateSize) currentProgress.Bytes += item.Size;
                            (Progress as IProgress<ProgressEventArgs>).Report(currentProgress);
                        }
                    }
                    else {
                        success = false;
                        Deferred.Add(item);
                    }
                } catch (Exception x) {
                    success = false;
                    Deferred.Add(item);
                    Errors.Add(new Error(item.Path, x.Message));
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a file system item with all optional subitems if applicable.
        /// </summary>
        /// <param name="item">File or directory item.</param>
        /// <param name="force">Set true to remove read-only items.</param>
        /// <returns>True if all items were removed.</returns>
        public async Task<bool> DeleteAsync(IItem item, bool force = false) => await Task.Run(() => Delete(item, force));

        /// <summary>
        /// Deteltes items from depth-first items collection.
        /// This operation WILL FAIL if collection is not ordered depth-first.
        /// </summary>
        /// <param name="items">Depth-first file system items collection.</param>
        /// <param name="force">Set true to remove read-only items.</param>
        /// <returns>True if all items were removed.</returns>
        public async Task<bool> DeleteAsync(IEnumerable<IItem> items, bool force = false) => await Task.Run(() => Delete(items, force));

        
    }

    public sealed class Error {
        public string Path { get; }
        public string Message { get; }
        public Error(string path, string message) {
            Path = path;
            Message = message;
        }
    }

    public sealed class Progress : System.Progress<ProgressEventArgs>, IProgress<ProgressEventArgs> { }

    public sealed class ProgressEventArgs : EventArgs {
        public string Path { get; }
        public int Item { get; set; }
        public int Count { get; }
        public long Bytes { get; set; }
        public long Size { get; }

        public ProgressEventArgs() { }

        public ProgressEventArgs(int count, long size = 0) {
            Count = count;
            Size = size;
        }

    }

    public static class IItemExtensions {

        public static Node GetNode(this IItem item) {
            if (item is Node) return item as Node;
            else return new Node(item.FileSystem, item.Path);
        }

        public static IItem GetParent(this IItem item) {
            var i1 = item.Path.LastIndexOf(item.FileSystem.DirectorySeparator);
            if (i1 < 1) return null;
            var path = item.Path.Substring(0, i1);
            var i2 = path.LastIndexOf(item.FileSystem.DirectorySeparator);
            var name = path.Substring(i2 + 1);
            return new Item {
                FileSystem = item.FileSystem,
                IsDirectory = true,
                Path = path,
                Name = name
            };
        }

    }

}
