﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MaxDistancePatch.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MaxDistancePatch.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All done..
        /// </summary>
        public static string AllDone {
            get {
                return ResourceManager.GetString("AllDone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Already patched. Nothing to do..
        /// </summary>
        public static string AlreadyPatched {
            get {
                return ResourceManager.GetString("AlreadyPatched", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applying the patch.
        /// </summary>
        public static string ApplyingPatch {
            get {
                return ResourceManager.GetString("ApplyingPatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When the patch is applied you can restore previous files from an automatically created backup..
        /// </summary>
        public static string BackupMessage {
            get {
                return ResourceManager.GetString("BackupMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Access denied while writing to the file. Try to run this program as Administrator..
        /// </summary>
        public static string CannotWrite {
            get {
                return ResourceManager.GetString("CannotWrite", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removes historic object visibility limits in the simulator..
        /// </summary>
        public static string Description {
            get {
                return ResourceManager.GetString("Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The distance the objects are visible from was set up in scenery files. This allowed the simulator to display only those object which are not too far from the camera. The authors who made sceneries many years ago on legacy computers set those values so low to cause visible and disturbing visual artifact of objects jumping suddelny into view. On some sceneries, like Drawinowo - the lanscape beyond the windshield looked a little desert-like. The current version of the simulator calculates objects visibility au [rest of string was truncated]&quot;;.
        /// </summary>
        public static string DescriptionDetailed {
            get {
                return ResourceManager.GetString("DescriptionDetailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initializing.
        /// </summary>
        public static string Initializing {
            get {
                return ResourceManager.GetString("Initializing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Move this file to main MaSzyna directory and try again..
        /// </summary>
        public static string InvalidDirectory {
            get {
                return ResourceManager.GetString("InvalidDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PATCH.
        /// </summary>
        public static string PatchCommand {
            get {
                return ResourceManager.GetString("PatchCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ready. {0} limited visibility nodes found..
        /// </summary>
        public static string ReadyNodesFound {
            get {
                return ResourceManager.GetString("ReadyNodesFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Searching for limited visibility nodes.
        /// </summary>
        public static string SearchingForLimitedNodes {
            get {
                return ResourceManager.GetString("SearchingForLimitedNodes", resourceCulture);
            }
        }
    }
}
