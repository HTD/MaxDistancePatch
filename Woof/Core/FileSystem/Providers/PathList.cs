using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace Woof.Core.FileSystem {

    public class PathList : IProvider {

        private readonly IEnumerable<string> Paths;
        private readonly string Name;
        private readonly IProvider FileSystem;

        public char DirectorySeparator { get; }

        public bool IsReadOnly => FileSystem.IsReadOnly;

        public bool CanRead => FileSystem.CanRead;

        public bool CanWrite => FileSystem.CanWrite;

        public bool CanCreateItems => false;

        public bool CanRemoveItems => false;

        public PathList(IEnumerable<string> paths, string @as = null) {
            FileSystem = LocalFS.Instance;
            DirectorySeparator = FileSystem.DirectorySeparator;
            Paths = paths;
            Name = @as;
        }

        public PathList(IProvider fileSystem, IEnumerable<string> paths, string @as = null) {
            FileSystem = fileSystem;
            DirectorySeparator = FileSystem.DirectorySeparator;
            Paths = paths;
            Name = @as;
        }

        public void NormalizePath(ref string path) => FileSystem.NormalizePath(ref path);

        public IItem GetItem(string path) => FileSystem.GetItem(path);

        public bool Exists(string path) => FileSystem.Exists(path);

        public IEnumerable<IItem> GetChildItems(string path = null, string pattern = "*") {
            if (pattern == "*")
                return Paths.Select(i => GetItem(i)).Where(i => i != null).Select(i => { i.Name = i.Path; return i; });
            else {
                var p = new WildcardPattern(pattern, WildcardOptions.Compiled | WildcardOptions.IgnoreCase);
                return Paths.Select(i => GetItem(i)).Where(i => i != null && p.IsMatch(i.Name)).Select(i => { i.Name = i.Path; return i; });
            }
        }

        public static IEnumerable<IItem> Query(IEnumerable<string> paths, string @as = null, string pattern = "*")
            => new PathList(paths, @as).GetChildItems(null, pattern);

        public bool CreatePath(string path) => FileSystem.CreatePath(path);

        public bool RemoveItem(string path, bool force = false) => FileSystem.RemoveItem(path, force);

        public bool RenameItem(IItem item, string name, bool force = false) => false;

        public Stream GetInputStream(string path) => FileSystem.GetInputStream(path);

        public Stream GetOutputStream(string path) => FileSystem.GetInputStream(path);

    }

}