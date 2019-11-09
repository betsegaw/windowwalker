using System;
using System.Deployment.Application;

namespace WindowWalker.Components
{

    public class ApplicationUpdates
    {
        static DateTime _lastUpdateCheck = DateTime.Now;
        static int numberOfDaysBetweenCheck = 1;

        public static void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info = null;

            var daysSinceLastUpdate = (DateTime.Now - _lastUpdateCheck).Days;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();
                }
                catch {
                    return;
                }
                finally
                {
                    _lastUpdateCheck = DateTime.Now;
                }

                if (info.UpdateAvailable || true)
                {
                    try
                    {
                        ad.Update();
                        System.Windows.Application.Current.Shutdown();
                        System.Windows.Forms.Application.Restart();
                    }
                    catch {
                        return;
                    }
                }
            }
        }
    }
}
