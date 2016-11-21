using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WindowWalker.Components
{
    /// <summary>
    /// Class that represents all the settings and
    /// can be serialized into JSON for easy saving
    /// </summary>
    class Settings
    {
        /// <summary>
        /// The version of the settings file
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// A list of all the shortcuts
        /// </summary>
        public Dictionary<string, List<string>> Shortcuts { get; set; }

        /// <summary>
        /// A list of saved window locations catagorized by number of screens
        /// </summary>
        public Dictionary<string, Point> WindowLocations { get; set; }

        /// <summary>
        /// The location of the search windows  (the top left point)
        /// </summary>
        [ScriptIgnore]
        public Point WindowLocation
        {
            get
            {
                if (WindowLocations.ContainsKey(System.Windows.Forms.Screen.AllScreens.Length.ToString()))
                {
                    return WindowLocations[System.Windows.Forms.Screen.AllScreens.Length.ToString()];
                }
                else
                {
                    return new Point() { X = 0, Y = 0 };
                }
            }

            set
            {
                if (WindowLocations == null)
                {
                    WindowLocations = new Dictionary<String, Point>();
                }
                WindowLocations[System.Windows.Forms.Screen.AllScreens.Length.ToString()] = value;
                
            }
        }

        /// <summary>
        /// Constructer to initialize some default values
        /// </summary>
        public Settings()
        {
            this.Version = string.Empty;
            this.Shortcuts = new Dictionary<string,List<string>>();
            this.WindowLocation = new Point() { X = 0, Y = 0 };
            this.WindowLocations = new Dictionary<String, Point>(); 
        }

        /// <summary>
        /// Custom point class to ease storing a point
        /// </summary>
        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }
    }
}
