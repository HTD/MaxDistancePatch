using System;
using System.IO;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Describes basic file system item properties.
    /// </summary>
    public interface IItem {

        IProvider FileSystem { get; }

        /// <summary>
        /// Gets or sets the file or directory name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the full path of the item.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the file (or directory) size in bytes.
        /// </summary>
        long Size { get; set; }

        /// <summary>
        /// Gets the last modified date of the item.
        /// </summary>
        DateTime? Date { get; set; }

        /// <summary>
        /// Gets the file or directory attributes.
        /// </summary>
        FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets optional item type information.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets the flag indicating whether the item is a file.
        /// </summary>
        bool IsFile { get; }

        /// <summary>
        /// Gets the flag indication whether the item is a directory.
        /// </summary>
        bool IsDirectory { get; }

    }

}