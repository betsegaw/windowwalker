using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// A class to handle the commands entered by the user, different 
    /// form the user being able to search through their windows
    /// </summary>
    class Commands
    {
        /// <summary>
        /// Constructor primarily used to enforce the creation of tips
        /// and populate the enabled commands list
        /// </summary>
        static Commands()
        {
            EnabledCommands = new Dictionary<string, Command>();

            EnabledCommands.Add( "quit",
                new Command()
                {
                    SearchTexts = new string[] {
                        ":quit",
                        ":q"
                    },
                    Tip = "type \":quit\" to exit"
                }
            );

            EnabledCommands.Add("launchTerminal",
                new Command()
                {
                    SearchTexts = new string[] {
                        ":lterminal",
                        ":lcmd",
                        ":lterm",
                        ":lt"
                    },
                    Tip = "type \":lt\" or \":lcmd\"to launch a new terminal window"
                }
            );

            EnabledCommands.Add("launchVSCode",
                new Command()
                {
                    SearchTexts = new string[] {
                        ":lvscode",
                        ":lcode"
                    },
                    Tip = "type \":lvscode\" or \":lcode\"to launch a new instance of VSCode"
                }
            );
        }

        /// <summary>
        /// Dictionary containing all the enabled commands
        /// </summary>
        private static Dictionary<string, Command> EnabledCommands;

        /// <summary>
        /// Primary method which executes on the commands that are passed to it
        /// </summary>
        /// <param name="commandText">The search text the user has entered</param>
        public static void ProcessCommand(string commandText)
        {
            Components.LivePreview.DeactivateLivePreview();

            if (EnabledCommands["quit"].SearchTexts.Contains(commandText))
            {
                System.Windows.Application.Current.Shutdown();
            }
            else if (EnabledCommands["launchTerminal"].SearchTexts.Contains(commandText))
            {
                Process.Start(new ProcessStartInfo("cmd.exe")
                { WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) });
            }
            else if (EnabledCommands["launchVSCode"].SearchTexts.Contains(commandText))
            {
                Process.Start("code");
            }
        }

        /// <summary>
        /// Gets the tips for all the enabled commands
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetTips()
        {
            return EnabledCommands.Select(x => x.Value.Tip);
        }

        /// <summary>
        /// Command class representing a single command
        /// </summary>
        public class Command
        {
            /// <summary>
            /// The set of substrings to search for in the search text to figure out if the user wants this command
            /// </summary>
            public string[] SearchTexts { get; set; }

            /// <summary>
            /// The help tip to get displayed in the cycling display
            /// </summary>
            public string Tip { get; set; }
        }
    }
}
