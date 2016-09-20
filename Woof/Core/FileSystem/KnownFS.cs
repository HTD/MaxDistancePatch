using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Woof.Core.FileSystem {

    /// <summary>
    /// Know file system class allows to enumerate known file systems.
    /// </summary>
    public sealed class KnownFS {

        #region Properties

        /// <summary>
        /// The type of IProvider implementation
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Short name of the file system.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets localized name to be displayed and recognized by localized applications.
        /// Application needs to set it first via <see cref="SetLocalizedName(Type, string)"/> method. 
        /// </summary>
        public string LocalizedName { get; private set; }

        /// <summary>
        /// Gets the flag indication if the file system is default root for all others.
        /// </summary>
        public bool IsRoot { get; private set; }

        /// <summary>
        /// Gets singleton instance or creates one if no singleton pattern available.
        /// </summary>
        public IProvider Instance =>
            Type.GetField("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as IProvider
            ?? Activator.CreateInstance(Type) as IProvider;

        #endregion

        #region Static methods

        /// <summary>
        /// Gets the default root file system definition.
        /// </summary>
        public static KnownFS Root => Collection.Single(i => i.IsRoot);

        /// <summary>
        /// Gets a known file system by name.
        /// </summary>
        /// <param name="name">Short file system name.</param>
        /// <returns></returns>
        public static KnownFS GetByName(string name) => Collection[name];

        /// <summary>
        /// Gets a know file system by instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static KnownFS GetByInstance(IProvider instance) => Collection[instance];

        /// <summary>
        /// Gets a known file system by localized name.
        /// </summary>
        /// <param name="name">Localized file system short name.</param>
        /// <returns></returns>
        public static KnownFS GetByLocalizedName(string name) => Collection.SingleOrDefault(i => i.LocalizedName.Equals(name, StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// Sets a localized name for the specified file system. It should be done on application start.
        /// </summary>
        /// <param name="type">File system class type.</param>
        /// <param name="name">Localized short name.</param>
        public static void SetLocalizedName(Type type, string name) {
            var fs = Collection.SingleOrDefault(i => i.Type == type);
            if (fs != null) fs.LocalizedName = name;
        }

        #endregion

        #region Non-public

        /// <summary>
        /// Static known file systems collection.
        /// </summary>
        private static readonly KnownFSCollection Collection = KnownFSCollection.Instance;

        /// <summary>
        /// Know file systems collection type.
        /// </summary>
        private sealed class KnownFSCollection : IEnumerable<KnownFS> {

            /// <summary>
            /// Known file systems definitions.
            /// </summary>
            private static KnownFS[] Items = new KnownFS[] {
                new KnownFS() { Type = typeof(LocalFS), Name = "LocalFS" },
                new KnownFS() { Type = typeof(DriveList), Name = "Drives", IsRoot = true },
                new KnownFS() { Type = typeof(PathList), Name = "PathList" }
            };

            /// <summary>
            /// <see cref="IEnumerable"/> implementation.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<KnownFS> GetEnumerator() {
                return ((IEnumerable<KnownFS>)Items).GetEnumerator();
            }

            /// <summary>
            /// <see cref="IEnumerable"/> implementation.
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator() {
                return ((IEnumerable<KnownFS>)Items).GetEnumerator();
            }

            /// <summary>
            /// By type indexer.
            /// </summary>
            /// <param name="type"><see cref="IProvider"/> type.</param>
            /// <returns>Known file system definition.</returns>
            public KnownFS this[Type type] => Items.SingleOrDefault(i => i.Type == type);

            /// <summary>
            /// By name indexer.
            /// </summary>
            /// <param name="name">Short name.</param>
            /// <returns>Known file system definition.</returns>
            public KnownFS this[string name] => Items.SingleOrDefault(i => i.Name == name);

            /// <summary>
            /// By instance indexer.
            /// </summary>
            /// <param name="instance"><see cref="IProvider"/> instance.</param>
            /// <returns></returns>
            public KnownFS this[IProvider instance] => Items.SingleOrDefault(i => i.Type == instance.GetType());

            /// <summary>
            /// Singleton instance.
            /// </summary>
            public static readonly KnownFSCollection Instance = new KnownFSCollection();

        }

        #endregion

    }

}