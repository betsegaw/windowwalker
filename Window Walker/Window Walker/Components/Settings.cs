using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    class Settings
    {
        public string Version { get; set; }

        public Dictionary<string, string> Shortcuts { get; set; }

        public Settings()
        {
            this.Version = string.Empty;
            this.Shortcuts = new Dictionary<string,string>();
        }
    }
}
