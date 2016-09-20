using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Woof.Core.FileSystem;

namespace MaxDistancePatch {

    /// <summary>
    /// MaxDistance patching utilities.
    /// </summary>
    sealed class MaxDistance {

        /// <summary>
        /// Gets the flag indicating if the current path is valid for the program to operate.
        /// </summary>
        public static bool IsValidPath { get; }

        /// <summary>
        /// Gets the patches for all matching files containing limited nodes.
        /// </summary>
        public static IEnumerable<Patch> NodePatches
            => NodeFiles.Select(i => GetNodePatch(i)).Where(i => i.IsNotEmpty);

        /// <summary>
        /// Creates backup directory if needed.
        /// </summary>
        public static void CreateBackupDir() {
            if (!Directory.Exists(BackupPath)) Directory.CreateDirectory(BackupPath);
        }

        /// <summary>
        /// Applies a specified patch.
        /// </summary>
        /// <param name="patch"><see cref="Patch"/> structure containing path and a list of replacements to make.</param>
        public static void ApplyPatch(Patch patch) {
            var file = new Text8();
            var backupPath = patch.Path.Replace(TargetPath, BackupPath);
            var backupDir = Path.GetDirectoryName(backupPath);
            if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
            file.Load(patch.Path);
            file.Save(backupPath);
            file.Replace(patch.Replacements);
            file.Save(patch.Path);
        }

        #region Non-public

        #region Configuration

        private const string DefaultPath = @"D:\MaSzyna";
        private const string SceneryDir = @"scenery";
        private const string BackupDir = @"MaxDistancePatch.Backup";
        private const string TestFile = @"eu07.ini";

        private static readonly string TargetPath;
        private static readonly string BackupPath;
        private static readonly Encoding Encoding = Encoding.GetEncoding(1250);
        private static readonly byte[] SeqNode = new Text8("node");
        private static readonly byte[] SeqNone = new Text8("-1");
        private static readonly byte[] SeqSound = new Text8("sound");
        private static readonly byte[] SeqEnd = new Text8("end");
        private static readonly int NodeSequenceLength = SeqNode.Length;
        private static readonly Regex RxNodeFiles = new Regex(@"\.(scn|scm|inc)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RxT3DFiles = new Regex(@"\.(t3d)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RxE3DFiles = new Regex(@"\.(e3d)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Properties and methods

        /// <summary>
        /// Gets a collection of paths to the files containing nodes.
        /// </summary>
        private static IEnumerable<string> NodeFiles
            => new Node(Path.Combine(TargetPath, SceneryDir))
                .DescendantFiles.Select(i => i.Path)
                .Where(i => RxNodeFiles.IsMatch(i));

        /// <summary>
        /// Static constructor testing validity of the working directory.
        /// </summary>
        static MaxDistance() {
            TargetPath = Directory.GetCurrentDirectory();
            if (!File.Exists(Path.Combine(TargetPath, TestFile))) TargetPath = DefaultPath;
            IsValidPath = File.Exists(Path.Combine(TargetPath, TestFile));
            if (IsValidPath) BackupPath = Path.Combine(TargetPath, BackupDir);
        }

        /// <summary>
        /// Gets the patch for limited visibility nodes for a file with specified path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Patch structure.</returns>
        private static Patch GetNodePatch(string path) {
            if (!File.Exists(path)) return new Patch();
            var t = new Text8(Encoding);
            t.Load(path);
            int i, offset = 0, n = t.Length;
            Text8.Match m;
            var replacements = new List<Text8.Replacement>();
            bool isIdentity = false, isSound = false;
            int soundOffset, endOffset;
            while (offset < n) {
                i = t.FindSequence(SeqNode, offset);
                if (i < 0) break;
                offset = i + NodeSequenceLength;
                m = t.FindSeparator(offset);
                if (m.Offset != offset) break;
                offset += m.Length;
                m = t.FindNumber(offset);
                if (m.Offset != offset) break;
                isIdentity = new ArraySegment<byte>(t, m.Offset, m.Length).SequenceEqual(SeqNone);
                endOffset = t.FindSequence(SeqEnd, offset, 255);
                if (endOffset > 0) {
                    soundOffset = t.FindSequence(SeqSound, offset, endOffset - offset);
                    isSound = soundOffset > 0;
                }
                if (!isIdentity && !isSound) replacements.Add(new Text8.Replacement { Offset = m.Offset, Length = m.Length, Data = SeqNone });
                offset += m.Length;
            }
            if (replacements.Count < 1) return new Patch();
            return new Patch { IsNotEmpty = true, Path = path, Replacements = replacements };
        }

        #endregion

        #endregion

        /// <summary>
        /// Patch structure.
        /// </summary>
        public struct Patch {

            /// <summary>
            /// True if the structure contains meaningful data.
            /// </summary>
            public bool IsNotEmpty;

            /// <summary>
            /// A path to the file to be replaced.
            /// </summary>
            public string Path;

            /// <summary>
            /// Replacements list.
            /// </summary>
            public List<Text8.Replacement> Replacements;

        }

    }

}