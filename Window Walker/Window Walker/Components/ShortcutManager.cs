using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Class for managing shortcuts
    /// Example: When you type "i" we actually search for "internet"
    /// </summary>
    class ShortcutManager
    {
        private static readonly string ShortcutsFile = Path.GetTempPath() + "WindowWalkerShortcuts.txt";

        private static ShortcutManager instance;

        public static ShortcutManager Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new ShortcutManager();
                }

                return instance;
            }
        }

        private ShortcutManager()
        {
            this.shortcuts = new Dictionary<string, string>();

            if (File.Exists(ShortcutsFile))
            {
                using (StreamReader reader = new StreamReader(ShortcutsFile))
                {
                    string currentLine;
                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        string[] parts = currentLine.Split('|');
                        if (parts.Length == 2 && !this.shortcuts.ContainsKey(parts[0]))
                        {
                            this.shortcuts.Add(parts[0], parts[1]);
                        }
                    }
                }
            }
        }

        Dictionary<string, string> shortcuts;

        public Dictionary<string, string> Shortcuts
        {
            get { return shortcuts; }
        }

        public bool AddShortcut(string before, string after)
        {
            if (shortcuts.ContainsKey(before))
            {
                return false;
            }

            shortcuts.Add(before, after);
            
            // Write the updated shortcuts list to a file
            WriteShortcutsToFile();
            
            return true;
        }

        public bool RemoveShortcut(string input)
        {
            if (!shortcuts.ContainsKey(input))
            {
                return false;
            }

            shortcuts.Remove(input);

            // Write the updated shortcuts list to a file
            WriteShortcutsToFile();

            return true;
        }

        public string GetShortcut(string input)
        {
            return (shortcuts.ContainsKey(input) ? shortcuts[input] : null);
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
                foreach (var shortcut in this.shortcuts)
                {
                    writer.WriteLine(shortcut.Key + "|" + shortcut.Value);
                }
            }
        }
    }
}
