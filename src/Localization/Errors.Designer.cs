﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Localization {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Errors {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Localization.Errors", typeof(Errors).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static string NeedRootAccess {
            get {
                return ResourceManager.GetString("NeedRootAccess", resourceCulture);
            }
        }
        
        public static string InvalidDevice {
            get {
                return ResourceManager.GetString("InvalidDevice", resourceCulture);
            }
        }
        
        public static string UserNameIsEmpty {
            get {
                return ResourceManager.GetString("UserNameIsEmpty", resourceCulture);
            }
        }
        
        public static string UserNoInfoFound {
            get {
                return ResourceManager.GetString("UserNoInfoFound", resourceCulture);
            }
        }
        
        public static string UserNameHasSpaces {
            get {
                return ResourceManager.GetString("UserNameHasSpaces", resourceCulture);
            }
        }
        
        public static string SnapshotDeviceHasSpaces {
            get {
                return ResourceManager.GetString("SnapshotDeviceHasSpaces", resourceCulture);
            }
        }
        
        public static string SnapshotDeviceIsEmpty {
            get {
                return ResourceManager.GetString("SnapshotDeviceIsEmpty", resourceCulture);
            }
        }
    }
}
