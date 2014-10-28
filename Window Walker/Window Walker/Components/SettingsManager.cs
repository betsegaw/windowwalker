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
        /// <summary>
        /// The path to the shortcut file
        /// </summary>
        private static readonly string ShortcutsFile = Path.GetTempPath() + "WindowWalkerShortcuts.ini";

        /// <summary>
        /// Reference to a serializer for saving the settings 
        /// </summary>
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        /// <summary>
        /// An instance of the settings class representing the current settings
        /// </summary>
        public static Settings SettingsInstance = new Settings();

        /// <summary>
        /// Instance of the manager itself
        /// </summary>
        private static SettingsManager instance;
        
        /// <summary>
        /// Implements Singlton pattern
        /// </summary>
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

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>Not sure why we have this AND a singlton pattern</remarks>
        static SettingsManager()
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

        /// <summary>
        /// Contructor that does nothing?
        /// </summary>
        private SettingsManager()
        {
            return;
        }

        /// <summary>
        /// Adds a shortcut to the settings
        /// </summary>
        /// <param name="before">what the user types</param>
        /// <param name="after">what the resulting search string is going to be</param>
        /// <returns>Returns true if it succeeds, false otherwise</returns>
        /// <remarks>Proably not usefull to actually do the true/false return since
        /// we can now have multiple shortcuts</remarks>
        public bool AddShortcut(string before, string after)
        {
            if (!SettingsManager.SettingsInstance.Shortcuts.ContainsKey(before))
            {
                SettingsManager.SettingsInstance.Shortcuts.Add(before, new List<string>());
            }

            SettingsManager.SettingsInstance.Shortcuts[before].Add(after);
            
            // Write the updated shortcuts list to a file
            SaveSettings();
            
            return true;
        }

        /// <summary>
        /// Removes a shortcut
        /// </summary>
        /// <param name="input">the input shortcut string</param>
        /// <returns>true if it succeeds, false otherwise</returns>
        /// <remarks>Probably has a bug since you can now a single input
        /// mapping to multiple outputs</remarks>
        public bool RemoveShortcut(string input)
        {
            if (!SettingsManager.SettingsInstance.Shortcuts.ContainsKey(input))
            {
                return false;
            }

            SettingsManager.SettingsInstance.Shortcuts.Remove(input);

            // Write the updated shortcuts list to a file
            SaveSettings();

            return true;
        }

        /// <summary>
        /// Retrieves a shortcut and returns all possible mappings
        /// </summary>
        /// <param name="input">the input string for the shortcuts</param>
        /// <returns>A list of all the shortcut strings that result from the user input</returns> 
        public List<string> GetShortcut(string input)
        {
            return (SettingsManager.SettingsInstance.Shortcuts.ContainsKey(input) ? SettingsManager.SettingsInstance.Shortcuts[input] : new List<string>());
        }
       
        /// <summary>
        /// Writes the current shortcuts to the shortcuts file.
        /// Note: We are writing the file even if there are no shortcuts. This handles
        /// the case where the user deletes their last shortcut.
        /// </summary>
        public void SaveSettings()
        {
            using (StreamWriter writer = new StreamWriter(ShortcutsFile, false))
            {
                writer.Write(SettingsManager.Serializer.Serialize(SettingsManager.SettingsInstance));
            }
        }
    }
}
