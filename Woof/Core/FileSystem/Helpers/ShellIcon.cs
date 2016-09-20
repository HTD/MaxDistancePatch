using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Windows API helpers managed.
    /// </summary>
    public static class ShellIcon {

        /// <summary>
        /// Retrieves system icon and type description for specified file system path.
        /// </summary>
        /// <param name="path">File system path.</param>
        /// <param name="attr">File attributes, if null, file attributes will be read from path.</param>
        /// <param name="iconSize">Returned icon size.</param>
        /// <returns>File icon and type structure.</returns>
        public static FileIconAndType GetFileIconAndType(string path, FileAttributes? attr = null, SystemIconSize iconSize = SystemIconSize.Small) {
            if (path != null && path[1] == ':' && path.Length == 2) path += @"\";
            NativeMethods.SHFILEINFO shFileInfo = new NativeMethods.SHFILEINFO();
            int cbFileInfo = Marshal.SizeOf(shFileInfo);
            var flags = NativeMethods.SHGFI.TypeName;
            if (attr != null && path.Length > 3) flags |= NativeMethods.SHGFI.UseFileAttributes;
            switch (iconSize) {
                case SystemIconSize.Small: flags |= NativeMethods.SHGFI.Icon | NativeMethods.SHGFI.SmallIcon; break;
                case SystemIconSize.Medium: flags |= NativeMethods.SHGFI.Icon; break;
                case SystemIconSize.Large: flags |= NativeMethods.SHGFI.Icon | NativeMethods.SHGFI.LargeIcon; break;
            }
            NativeMethods.SHGetFileInfo(path, (int)attr, out shFileInfo, (uint)cbFileInfo, flags);
            return new FileIconAndType {
                Icon = (shFileInfo.hIcon != IntPtr.Zero) ? GetImageFromHIcon(shFileInfo.hIcon) : null,
                TypeDescription = shFileInfo.szTypeName
            };
        }

        /// <summary>
        /// Gets default system icon and type description for a directory.
        /// </summary>
        /// <param name="iconSize">Returned icon size.</param>
        /// <returns></returns>
        public static FileIconAndType GetDefaultDirectoryIconAndType(SystemIconSize iconSize = SystemIconSize.Small)
            => GetFileIconAndType(DirectoryKey, FileAttributes.Directory, iconSize);

        /// <summary>
        /// Gets default system icon and type description for a file.
        /// </summary>
        /// <param name="iconSize">Returned icon size.</param>
        /// <returns></returns>
        public static FileIconAndType GetDefaultFileIconAndType(SystemIconSize iconSize = SystemIconSize.Small)
            => GetFileIconAndType(FileKey, FileAttributes.Normal, iconSize);

        /// <summary>
        /// Returns a managed BitmapSource, based on the provided pointer to an unmanaged icon image.
        /// </summary>
        /// <param name="hIcon"></param>
        /// <returns></returns>
        private static ImageSource GetImageFromHIcon(IntPtr hIcon) {
            if (hIcon == IntPtr.Zero) return null;
            try { return Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); }
            finally { NativeMethods.DestroyIcon(hIcon); }
        }

        private const string FileKey = "File";
        private const string DirectoryKey = "Directory";

    }

    public struct FileIconAndType {
        public string TypeDescription;
        public ImageSource Icon;
    }

    public enum SystemIconSize { None, Small, Medium, Large }

}
