﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Yaio.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FromFolderPath {
            get {
                return ((string)(this["FromFolderPath"]));
            }
            set {
                this["FromFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ToFolderPath {
            get {
                return ((string)(this["ToFolderPath"]));
            }
            set {
                this["ToFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".jpg,.mpg,.tiff,.jpeg")]
        public string FileExtensions {
            get {
                return ((string)(this["FileExtensions"]));
            }
            set {
                this["FileExtensions"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CreateYearDirectory {
            get {
                return ((bool)(this["CreateYearDirectory"]));
            }
            set {
                this["CreateYearDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CreateMonthDirectory {
            get {
                return ((bool)(this["CreateMonthDirectory"]));
            }
            set {
                this["CreateMonthDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Recursive {
            get {
                return ((bool)(this["Recursive"]));
            }
            set {
                this["Recursive"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<style>
img {
max-width:200px;
max-height:200px;
width: auto;      
height: auto;     
}
</style>

<script>
function myFunction() {
    var x = document.getElementById(""mySelect"").value;
	//alert(x);
		var img = document.getElementsByTagName('img');
        for(var i=0;i<img.length;i++) {
           img[i].style.maxWidth = x + 'px';
           img[i].style.maxHeight = x + 'px'; 
	}
}
</script>

<select id=""mySelect"" onchange=""myFunction()"">
  <option value=""500"">500
  <option value=""400"">400
  <option value=""300"">300
  <option value=""200"">200
</select>")]
        public string PrefixHtml {
            get {
                return ((string)(this["PrefixHtml"]));
            }
            set {
                this["PrefixHtml"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DeleteDuplicateFilesFromProcessFolder {
            get {
                return ((bool)(this["DeleteDuplicateFilesFromProcessFolder"]));
            }
            set {
                this["DeleteDuplicateFilesFromProcessFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SearchForDuplicatesRecursivelyInYearFolder {
            get {
                return ((bool)(this["SearchForDuplicatesRecursivelyInYearFolder"]));
            }
            set {
                this["SearchForDuplicatesRecursivelyInYearFolder"] = value;
            }
        }
    }
}
