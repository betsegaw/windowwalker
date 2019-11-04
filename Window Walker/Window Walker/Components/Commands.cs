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
        static Commands()
        {
            EnabledCommands = new Dictionary<string, Command>();

            EnabledCommands.Add( "quit",
                new Command()
                {
                    SearchTexts = new string[] {
                        ":quit"
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
        }

        private static Dictionary<string, Command> EnabledCommands;

        public static void ProcessCommand(string commandText)
        {
            if (EnabledCommands["quit"].SearchTexts.Contains(commandText))
            {
                System.Windows.Application.Current.Shutdown();
            }
            else if (EnabledCommands["launchTerminal"].SearchTexts.Contains(commandText))
            {
                Components.LivePreview.DeactivateLivePreview();

                Process.Start(new ProcessStartInfo("cmd.exe")
                { WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) });
            }
        }

        public static IEnumerable<string> GetTips()
        {
            return EnabledCommands.Select(x => x.Value.Tip);
        }

        public class Command
        {
            public string[] SearchTexts { get; set; }
            public string Tip { get; set; }
        }
    }
}
