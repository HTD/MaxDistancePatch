using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Virtual file system for enumerating system drives.
    /// </summary>
    class DriveList : IProvider {

        #region Implementation

        public bool IsReadOnly => false;
        public bool CanRead => false;
        public bool CanWrite => false;
        public bool CanCreateItems => false;
        public bool CanRemoveItems => false;

        public char DirectorySeparator { get; } = LocalFS.Instance.DirectorySeparator;

        public IEnumerable<IItem> GetChildItems(string path, string pattern = "*")
           => System.IO.Directory.GetLogicalDrives().Select(i => GetItem(i));

        public bool Exists(string path) => GetItem(path) != null;

        public IItem GetItem(string path) {
            var di = new DriveInfo(path);
            var name = GetDisplayName(di);
            return new Item {
                FileSystem = this,
                Name = name,
                Path = di.Name,
                IsDirectory = true,
                Size = di.TotalSize
            };
        }

        /// <summary>
        /// Renames a file or directory.
        /// </summary>
        /// <param name="item">Original item.</param>
        /// <param name="name">New name.</param>
        /// <param name="force">Set true to rename read-only items.</param>
        /// <returns>True if renamed.</returns>
        public bool RenameItem(IItem item, string name, bool force = false) {
            var di = new DriveInfo(item.Path);
            di.VolumeLabel = name;
            item.Name = GetDisplayName(di);
            return true;
        }


        public void NormalizePath(ref string path) => LocalFS.Instance.NormalizePath(ref path);

        private string GetDisplayName(DriveInfo di) => $"{di.Name.Trim(DirectorySeparator)} {di.VolumeLabel}";

        #region Not implemented

        public bool CreatePath(string path) => false;

        public bool RemoveItem(string path, bool force = false) => false;

        public Stream GetInputStream(string path) {
            throw new NotImplementedException();
        }

        public Stream GetOutputStream(string path) {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Singleton

        private DriveList() { }

        public static readonly DriveList Instance = new DriveList();

        #endregion

    }

}