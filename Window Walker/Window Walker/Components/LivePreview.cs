using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Class containg methods to control the live preview
    /// </summary>
    class LivePreview
    {
        /// <summary>
        /// Makes sure that a window is excluded from the live preview
        /// </summary>
        /// <param name="hwnd">handle to the window to exclude</param>
        public static void SetWindowExlusionFromLivePreview(IntPtr hwnd)
        {
            int renderPolicy = (int)InteropAndHelpers.DwmNCRenderingPolicy.Enabled;

            InteropAndHelpers.DwmSetWindowAttribute(
                hwnd,
                12,
                ref renderPolicy,
                sizeof(int)
                );
        }

        /// <summary>
        /// Activates the live preview
        /// </summary>
        /// <param name="targetWindow">the window to show by making all other windows transparent</param>
        /// <param name="windowToSpare">the window which should not be transparent but is not the target window</param>
        public static void ActivateLivePreview(IntPtr targetWindow, IntPtr windowToSpare)
        {
            InteropAndHelpers.DwmpActivateLivePreview(
                    true,
                    targetWindow,
                    windowToSpare,
                    InteropAndHelpers.LivePreviewTrigger.Superbar,
                    IntPtr.Zero);
        }

        /// <summary>
        /// Deactivates the live preview
        /// </summary>
        public static void DeactivateLivePreview()
        {
            InteropAndHelpers.DwmpActivateLivePreview(
                    false,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    InteropAndHelpers.LivePreviewTrigger.AltTab,
                    IntPtr.Zero);
        }
    }
}
