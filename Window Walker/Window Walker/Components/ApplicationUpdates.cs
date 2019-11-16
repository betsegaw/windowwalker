using System;
using System.ComponentModel;
using System.Deployment.Application;

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

                ad.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(CheckForUpdateCompleted);
                ad.CheckForUpdateAsync();

                _lastUpdateCheck = DateTime.Now;
            }
        }

        private static void CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null || !(e.UpdateAvailable))
            {
                alreadyCheckingForUpdate = false;
                return;
            }
            else
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
            System.Windows.Application.Current.Shutdown();
            System.Windows.Forms.Application.Restart();
        }
    }
}
