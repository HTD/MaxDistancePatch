using System;
using System.Collections;
using System.Collections.Generic;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Orders over <see cref="IItem"/> collection.
    /// </summary>
    public enum FileSystemOrder { None, Name, NameDesc, DateModified, DateModifiedDesc, Type, TypeDesc, Size, SizeDesc }

    /// <summary>
    /// Comparer for orders over <see cref="IItem"/> collection.
    /// </summary>
    class Comparer : IComparer<IItem>, IComparer {

        /// <summary>
        /// Creates a new comparer with defined order.
        /// </summary>
        /// <param name="orderBy">Order over <see cref="IItem"/> collection.</param>
        public Comparer(FileSystemOrder orderBy) { OrderBy = orderBy; }

        /// <summary>
        /// Order over <see cref="IItem"/> collection.
        /// </summary>
        public readonly FileSystemOrder OrderBy;

        /// <summary>
        /// Compares 2 <see cref="IItem"/> elements.
        /// </summary>
        /// <param name="x">First element.</param>
        /// <param name="y">Second element.</param>
        /// <returns></returns>
        public int Compare(IItem x, IItem y) {
            switch (OrderBy) {
                case FileSystemOrder.None:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    else return 0;
                case FileSystemOrder.Name:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    else return String.Compare(x.Name, y.Name, true);
                case FileSystemOrder.DateModified:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    if (x.Date < y.Date) return -1;
                    else if (x.Date > y.Date) return 1;
                    else return 0;
                case FileSystemOrder.Type:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    else return String.Compare(x.Type, y.Type);
                case FileSystemOrder.Size:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    if (x.Size < y.Size) return -1;
                    else if (x.Size > y.Size) return 1;
                    else return 0;
                case FileSystemOrder.NameDesc:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    else return -String.Compare(x.Name, y.Name, true);
                case FileSystemOrder.DateModifiedDesc:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    if (x.Date < y.Date) return 1;
                    else if (x.Date > y.Date) return -1;
                    else return 0;
                case FileSystemOrder.TypeDesc:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    else return -String.Compare(x.Type, y.Type);
                case FileSystemOrder.SizeDesc:
                    if (x.IsDirectory && !y.IsDirectory) return -1;
                    else if (!x.IsDirectory && y.IsDirectory) return 1;
                    if (x.Size < y.Size) return 1;
                    else if (x.Size > y.Size) return -1;
                    else return 0;
                default: return 0;
            }
        }

        /// <summary>
        /// Compares 2 objects as <see cref="IItem"/> elements.
        /// </summary>
        /// <param name="x">First element.</param>
        /// <param name="y">Second element.</param>
        /// <returns></returns>
        public int Compare(object x, object y) => Compare(x as IItem, y as IItem);

    }

}