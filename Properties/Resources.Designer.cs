﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace zKinectV2OSC.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("zKinectV2OSC.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 127.0.0.1.
        /// </summary>
        internal static string DefaultIpAddressCsv {
            get {
                return ResourceManager.GetString("DefaultIpAddressCsv", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initializing.
        /// </summary>
        internal static string InitializingStatusTextFormat {
            get {
                return ResourceManager.GetString("InitializingStatusTextFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ip.txt.
        /// </summary>
        internal static string IpAddressFileName {
            get {
                return ResourceManager.GetString("IpAddressFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kinect not found....
        /// </summary>
        internal static string NoSensorFoundText {
            get {
                return ResourceManager.GetString("NoSensorFoundText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 50.
        /// </summary>
        internal static string oscUpdateRateMs {
            get {
                return ResourceManager.GetString("oscUpdateRateMs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 12345.
        /// </summary>
        internal static string PortNumber {
            get {
                return ResourceManager.GetString("PortNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:N1}fps.
        /// </summary>
        internal static string StandardFramesTextFormat {
            get {
                return ResourceManager.GetString("StandardFramesTextFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:hh\:mm\:ss}.
        /// </summary>
        internal static string StandardUptimeTextFormat {
            get {
                return ResourceManager.GetString("StandardUptimeTextFormat", resourceCulture);
            }
        }
    }
}
