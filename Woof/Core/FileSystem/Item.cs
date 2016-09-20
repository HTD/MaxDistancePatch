using System;
using System.IO;

namespace Woof.Core.FileSystem {

    public class Item : IItem {

        public IProvider FileSystem { get; set; }

        public FileAttributes Attributes { get; set; }

        public DateTime? Date { get; set; }

        public bool IsDirectory { get; set; }

        public bool IsFile { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }

    }

}