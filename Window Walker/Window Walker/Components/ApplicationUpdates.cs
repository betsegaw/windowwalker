using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Windows.Forms;

namespace WindowWalker.Components
{
    public class ApplicationUpdates
    {
        static DateTime _lastUpdateCheck = DateTime.Now;
        static int numberOfDaysBetweenCheck = 1;

        static bool alreadyCheckingForUpdate = false;

        public static void InstallUpdateSyncWithInfo()
        {
            if (alreadyCheckingForUpdate)
            {
                return;
            }
            else 
            {
                alreadyCheckingForUpdate = true;
            }

            var daysSinceLastUpdate = (DateTime.Now - _lastUpdateCheck).Days;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    ad.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(CheckForUpdateCompleted); 
                    ad.CheckForUpdateAsync();
                }
                catch {
                    alreadyCheckingForUpdate = false;
                    return;
                }
                finally
                {
                    _lastUpdateCheck = DateTime.Now;
                }
            }
        }

        private static void CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                alreadyCheckingForUpdate = false;
                return;
            }
            else if (e.UpdateAvailable)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                ad.UpdateCompleted += new AsyncCompletedEventHandler(UpdateCompleted);
                ad.UpdateAsync();
            }
        }

        private static void UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                alreadyCheckingForUpdate = false;
                return;
            }
            alreadyCheckingForUpdate = false;
            System.Windows.Forms.Application.Restart();
        }
    }
}
