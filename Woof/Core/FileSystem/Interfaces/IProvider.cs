using System.Collections.Generic;
using System.IO;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Interface for file system providers.
    /// </summary>
    public interface IProvider {

        /// <summary>
        /// Gets the flag indicating whether the file system is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the flag indicating whether file read operations are supported.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Gets the flag indicating whether file write operations are supported.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Gets the flag indicating whether adding of new items is supported.
        /// </summary>
        bool CanCreateItems { get; }

        /// <summary>
        /// Gets the flag indicating whether removing of new item is supported.
        /// </summary>
        bool CanRemoveItems { get; }

        /// <summary>
        /// Default directory separator character used in file system.
        /// </summary>
        char DirectorySeparator { get; }

        /// <summary>
        /// Normalizes file system path removing optional characters at both ends.
        /// </summary>
        /// <param name="path">Path to normalize.</param>
        void NormalizePath(ref string path);

        /// <summary>
        /// Returns true if specified path exists within file system.
        /// </summary>
        /// <param name="path">Path to test.</param>
        /// <returns></returns>
        bool Exists(string path);

        /// <summary>
        /// Gets a file system item referenced by path.
        /// </summary>
        /// <param name="path">Item path.</param>
        /// <returns>File system item object or null if the item doesn't exist.</returns>
        IItem GetItem(string path);

        /// <summary>
        /// Creates a new directory.
        /// </summary>
        /// <param name="item">Path to create.</param>
        /// <returns>True if path created.</returns>
        bool CreatePath(string path);

        /// <summary>
        /// Removes a file or an empty directory.
        /// </summary>
        /// <param name="path">Path to remove.</param>
        /// <param name="force">Set true to remove read-only items.</param>
        /// <returns>True if item deleted.</returns>
        bool RemoveItem(string path, bool force = false);

        /// <summary>
        /// Renames a file or directory.
        /// </summary>
        /// <param name="path">Original path.</param>
        /// <param name="name">New name.</param>
        /// <param name="force">Set true to rename read-only items.</param>
        /// <returns>True if renamed.</returns>
        bool RenameItem(IItem path, string name, bool force = false);

        /// <summary>
        /// Gets a stream used to read data.
        /// </summary>
        /// <param name="path">Source path.</param>
        /// <returns></returns>
        Stream GetInputStream(string path);

        /// <summary>
        /// Gets a stream used to write data.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <returns></returns>
        Stream GetOutputStream(string path);

        /// <summary>
        /// Gets all child items contained in node referenced by path, matching optional search pattern.
        /// </summary>
        /// <param name="path">File system path.</param>
        /// <param name="pattern">Search pattern.</param>
        /// <returns>A collection of file system items.</returns>
        IEnumerable<IItem> GetChildItems(string path, string pattern = "*");

    }

}