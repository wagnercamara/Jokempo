using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class KillEvent : EventArgs
    {
        public ThreadClient threadClient { get; set; }
    }
}
