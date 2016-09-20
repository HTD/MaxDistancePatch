using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Provides mehtods for querying local file system.
    /// </summary>
    public sealed class LocalFS : IProvider {

        /// <summary>
        /// Provides a platform-specific character used to separate directory levels in a path string that reflects a hierarchical file system organization.
        /// </summary>
        public char DirectorySeparator { get; } = Path.DirectorySeparatorChar;

        /// <summary>
        /// Gets the flag indicating whether the file system is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the flag indicating whether file read operations are supported.
        /// </summary>
        public bool CanRead => true;

        /// <summary>
        /// Gets the flag indicating whether file write operations are supported.
        /// </summary>
        public bool CanWrite => true;

        /// <summary>
        /// Gets the flag indicating whether adding of new items is supported.
        /// </summary>
        public bool CanCreateItems => true;

        /// <summary>
        /// Gets the flag indicating whether removing of new item is supported.
        /// </summary>
        public bool CanRemoveItems => true;

        /// <summary>
        /// Remove optional characters from both ends of the path.
        /// </summary>
        /// <param name="path">Path to normalize.</param>
        public void NormalizePath(ref string path) => path = path.Trim(DirectorySeparator, DirectoryWhitespace);

        /// <summary>
        /// Returns true if specified path exists within file system.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path) => GetItem(path) != null;

        /// <summary>
        /// Gets a file system item referenced by path.
        /// </summary>
        /// <param name="path">Item path.</param>
        /// <returns>File system item object or null if the item doesn't exist.</returns>
        public IItem GetItem(string path) {
            if (String.IsNullOrEmpty(path)) return null;
            NormalizePath(ref path);
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists) {
                return new Item() {
                    FileSystem = this,
                    Name = fileInfo.Name,
                    Path = fileInfo.FullName,
                    IsFile = true,
                    IsDirectory = false,
                    Size = fileInfo.Length,
                    Date = fileInfo.LastWriteTime,
                    Attributes = fileInfo.Attributes
                };
            } else {
                var directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists) {
                    return new Item() {
                        FileSystem = this,
                        Name = fileInfo.Name,
                        Path = path,
                        IsFile = false,
                        IsDirectory = true,
                        Size = 0,
                        Date = fileInfo.LastWriteTime,
                        Attributes = fileInfo.Attributes
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all child items contained in node referenced by path, matching optional search pattern.
        /// </summary>
        /// <param name="path">File system path. For null path root file system items are returned.</param>
        /// <param name="pattern">Search pattern.</param>
        /// <returns>A collection of file system items.</returns>
        public IEnumerable<IItem> GetChildItems(string path, string pattern = "*") {
            if (path == null) {
                foreach (var i in KnownFS.Root.Instance.GetChildItems(null)) yield return i;
                yield break;
            }
            if (path == "" || path == ".") path = Directory.GetCurrentDirectory();
            NormalizePath(ref path);
            string rootPath = path;
            NativeMethods.WIN32_FIND_DATAW fdata;
            IntPtr handle = NativeMethods.INVALID_HANDLE_VALUE;
            bool isDirectory;
            string search;
            try {
                search = path + DirectorySeparator + pattern;
                handle = NativeMethods.FindFirstFileW(search, out fdata);
                if (handle != NativeMethods.INVALID_HANDLE_VALUE) {
                    do {
                        if (fdata.cFileName == DirectoryCurrent || fdata.cFileName == DirectoryParent) continue;
                        if ((fdata.dwFileAttributes & FileAttributes.ReparsePoint) != 0) continue;
                        path = rootPath + DirectorySeparator + fdata.cFileName;
                        isDirectory = (fdata.dwFileAttributes & FileAttributes.Directory) != 0;
                        yield return new Item() {
                            FileSystem = this,
                            Name = fdata.cFileName,
                            Path = path,
                            IsFile = !isDirectory,
                            IsDirectory = isDirectory,
                            Size = isDirectory ? 0 : (((long)fdata.nFileSizeHigh << 0x20) + fdata.nFileSizeLow),
                            Date = DateTime.FromFileTime((((long)fdata.ftLastWriteTime.dwHighDateTime << 0x20) + fdata.ftLastWriteTime.dwLowDateTime)),
                            Attributes = fdata.dwFileAttributes

                        };
                    } while (NativeMethods.FindNextFile(handle, out fdata));
                }
            }
            finally {
                if (handle != NativeMethods.INVALID_HANDLE_VALUE) NativeMethods.FindClose(handle);
            }
        }

        /// <summary>
        /// Gets all child items contained in node referenced by path, matching optional search pattern.
        /// </summary>
        /// <param name="path">File system path.</param>
        /// <param name="pattern">Search pattern.</param>
        /// <returns>A collection of file system items.</returns>
        public static IEnumerable<IItem> QueryPath(string path, string pattern = "*") {
            var rootFS = KnownFS.Root;
            if (path == null || path.Equals(rootFS.Name, StringComparison.InvariantCultureIgnoreCase) ||
                rootFS.LocalizedName != null && path.Equals(rootFS.LocalizedName, StringComparison.CurrentCultureIgnoreCase))
                return rootFS.Instance.GetChildItems(path, pattern);
            return Instance.GetChildItems(path, pattern);
        }

        /// <summary>
        /// Queires a file drop for all included files (including subdirectiories)
        /// </summary>
        /// <param name="data"><see cref="DataFormats.FileDrop"/> data object.</param>
        /// <returns>All files from dropped directories and files dropped directly.</returns>
        public static IEnumerable<IItem> QueryFileDrop(IDataObject data) {
            if (data.GetDataPresent(DataFormats.FileDrop)) {
                var drop = data.GetData(DataFormats.FileDrop) as string[];
                foreach (var path in drop) {
                    var item = Instance.GetItem(path);
                    if (item.IsDirectory) {
                        var node = new Node(item.Path);
                        foreach (var file in node.DescendantFiles) yield return file;
                    }
                    else yield return item;
                }
            }
        }

        /// <summary>
        /// Enumerats all items in file system tree, depth first.
        /// </summary>
        /// <param name="path">File system path.</param>
        /// <returns></returns>
        public static IEnumerable<INode> TraversePath(string path) => new Node(path).Traverse();

        /// <summary>
        /// Calculates total directory size.
        /// </summary>
        /// <param name="path">File system path.</param>
        /// <returns></returns>
        public static long GetDirectorySize(string path) => TraversePath(path).Sum(i => i.Size);

        /// <summary>
        /// Returns full directory tree (root node) with all directory sizes calculated.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Root node containing all directory tree, including files, with all directory sizes calculated.</returns>
        public static INode GetAllDirectorySizes(string path) => new Node(path).GetSizes();

        /// <summary>
        /// Creates a new directory.
        /// </summary>
        /// <param name="item">Path to create.</param>
        /// <returns>True if path created.</returns>
        public bool CreatePath(string path) {
            if (Directory.Exists(path)) return false;
            if (File.Exists(path)) return false;
            Directory.CreateDirectory(path);
            return true;
        }

        /// <summary>
        /// Removes a file or an empty directory.
        /// </summary>
        /// <param name="path">Path to remove.</param>
        /// <param name="force">Set true to remove read-only items.</param>
        /// <returns>True if item deleted.</returns>
        public bool RemoveItem(string path, bool force = false) {
            var item = Instance.GetItem(path);
            if (item == null) return false;
            if (item.IsFile) {
                if ((item.Attributes & FileAttributes.ReadOnly) != 0)
                    if (force) File.SetAttributes(path, item.Attributes & ~FileAttributes.ReadOnly); else return false;
                File.Delete(item.Path);
            }
            else {
                if ((item.Attributes & FileAttributes.ReadOnly) != 0) {
                    if (force) {
                        var di = new DirectoryInfo(path);
                        di.Attributes &= ~FileAttributes.ReadOnly;
                    }
                    else return false;
                }
                Directory.Delete(item.Path);
            }
            return true;
        }

        /// <summary>
        /// Renames a file or directory.
        /// </summary>
        /// <param name="path">Original path.</param>
        /// <param name="name">New name.</param>
        /// <param name="force">Set true to rename read-only items.</param>
        /// <returns>True if renamed.</returns>
        public bool RenameItem(IItem item, string name, bool force = false) {
            if (item == null) return false;
            if (name.Contains(DirectorySeparator)) return false;
            var parent = item.GetParent().Path;
            var targetPath = parent + DirectorySeparator + name;

            var isReadOnly = (item.Attributes & FileAttributes.ReadOnly) != 0;
            if (!force && isReadOnly) return false;
            if (isReadOnly) {
                if (item.IsFile) File.SetAttributes(item.Path, item.Attributes & ~FileAttributes.ReadOnly);
                if (item.IsDirectory) {
                    var di = new DirectoryInfo(item.Path);
                    di.Attributes &= ~FileAttributes.ReadOnly;
                }
            }
            if (item.IsFile) File.Move(item.Path, targetPath);
            if (item.IsDirectory) Directory.Move(item.Path, targetPath);
            item.Path = targetPath;
            item.Name = name;
            if (isReadOnly) {
                if (item.IsFile) File.SetAttributes(item.Path, item.Attributes & FileAttributes.ReadOnly);
                if (item.IsDirectory) {
                    var di = new DirectoryInfo(item.Path);
                    di.Attributes &= FileAttributes.ReadOnly;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a stream used to read data.
        /// </summary>
        /// <param name="path">Source path.</param>
        /// <returns></returns>
        public Stream GetInputStream(string path) => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        /// <summary>
        /// Gets a stream used to write data.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <returns></returns>
        public Stream GetOutputStream(string path) => new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

        #region Singleton

        /// <summary>
        /// Private constructor to prevent creating multiple instances of this class.
        /// </summary>
        private LocalFS() { }

        /// <summary>
        /// Provides singleton access to <see cref="LocalFS"/> instance.
        /// </summary>
        public readonly static LocalFS Instance;

        /// <summary>
        /// Creates singleton instance on first access.
        /// </summary>
        static LocalFS() { Instance = new LocalFS(); }

        #endregion

        #region Constants and enumerations

        /// <summary>
        /// A file name used to indicate current directory.
        /// </summary>
        public const string DirectoryCurrent = ".";

        /// <summary>
        /// A file name used to indicate parent directory.
        /// </summary>
        public const string DirectoryParent = "..";

        /// <summary>
        /// White space character to be removed from begining and end of path name when normalized.
        /// </summary>
        private const char DirectoryWhitespace = ' ';

        #endregion

    }

}
