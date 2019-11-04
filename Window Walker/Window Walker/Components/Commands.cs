using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    class Commands
    {
        private static string QuitCommand = ":quit";
        private static string[] TerminalLaunchCommands = new string[]
        {
            ":lterminal",
            ":lcmd",
            ":lterm",
            ":lt"
        };

        public static void ProcessCommand(string commandText)
        {
            if (commandText == QuitCommand)
            {
                System.Windows.Application.Current.Shutdown();
            }
            else if (TerminalLaunchCommands.Contains(commandText))
            {
                Components.LivePreview.DeactivateLivePreview();

                Process.Start(new ProcessStartInfo("cmd.exe")
                { WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) });
            }
        }
    }
}
