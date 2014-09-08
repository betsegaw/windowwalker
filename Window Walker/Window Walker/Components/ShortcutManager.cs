using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WindowWalker.Components
{
    /// <summary>
    /// Class for managing shortcuts
    /// Example: When you type "i" we actually search for "internet"
    /// </summary>
    class SettingsManager
    {
        private static readonly string ShortcutsFile = Path.GetTempPath() + "WindowWalkerShortcuts.ini";

        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public static Settings SettingsInstance = new Settings();

        private static SettingsManager instance;

        public static SettingsManager Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new SettingsManager();
                }

                return instance;
            }
        }

        private SettingsManager()
        {
            if (File.Exists(ShortcutsFile))
            {
                using (StreamReader reader = new StreamReader(ShortcutsFile))
                {
                    string jsonString = reader.ReadToEnd();
                    SettingsManager.SettingsInstance = (Settings)SettingsManager.Serializer.Deserialize(jsonString, typeof(Settings));
                }
            }
        }

        public bool AddShortcut(string before, string after)
        {
            if (!SettingsManager.SettingsInstance.Shortcuts.ContainsKey(before))
            {
                SettingsManager.SettingsInstance.Shortcuts.Add(before, new List<string>());
            }

            SettingsManager.SettingsInstance.Shortcuts[before].Add(after);
            
            // Write the updated shortcuts list to a file
            WriteShortcutsToFile();
            
            return true;
        }

        public bool RemoveShortcut(string input)
        {
            if (!SettingsManager.SettingsInstance.Shortcuts.ContainsKey(input))
            {
                return false;
            }

            SettingsManager.SettingsInstance.Shortcuts.Remove(input);

            // Write the updated shortcuts list to a file
            WriteShortcutsToFile();

            return true;
        }

        public List<string> GetShortcut(string input)
        {
            return (SettingsManager.SettingsInstance.Shortcuts.ContainsKey(input) ? SettingsManager.SettingsInstance.Shortcuts[input] : new List<string>());
        }
       
        /// <summary>
        /// Writes the current shortcuts to the shortcuts file.
        /// Note: We are writing the file even if there are no shortcuts. This handles
        /// the case where the user deletes their last shortcut.
        /// </summary>
        private void WriteShortcutsToFile()
        {
            using (StreamWriter writer = new StreamWriter(ShortcutsFile, false))
            {
                writer.Write(SettingsManager.Serializer.Serialize(SettingsManager.SettingsInstance));
            }
        }
    }
}
