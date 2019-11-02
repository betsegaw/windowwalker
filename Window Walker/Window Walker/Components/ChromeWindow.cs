using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    class ChromeWindow : Window
    {
        string title;

        public override string Title
        {
            get
            {
                return title;
            } 
        }

       public ChromeWindow(IntPtr hwnd, string tabName): base(hwnd)
       {
            this.title = tabName;
       }
    }
}
