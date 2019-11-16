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

        public static void InstallUpdateSyncWithInfo()
        {
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
                return;
            }

            System.Windows.Forms.Application.Restart();
        }
    }
}
