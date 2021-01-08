using Login2.Auxiliary.WebAPIRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Login2.Auxiliary.Scanner
{
    class ScannerModule
    {
        CaptureScreen cpt;
        public ScannerModule()
        {
            cpt= new CaptureScreen(null, new FPTApiRequest());
        }
        public object getData()
        {
             return cpt.Fetch().Result;
        }
        public void Show() 
        {
            cpt.ShowDialog();
        }
    }
}
