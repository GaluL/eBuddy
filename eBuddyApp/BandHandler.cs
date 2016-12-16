using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBuddy
{
    class BandHandler
    {
        private static BandHandler instance;
        public static BandHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BandHandler();
                }

                return instance;
            }
        }

        private bool isPaired;
        public bool IsPaired
        {
            get { return isPaired; }
        }

        private BandHandler()
        {

        }
    }
}
