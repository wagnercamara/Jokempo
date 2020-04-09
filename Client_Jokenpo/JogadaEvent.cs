using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Jokenpo
{
    public class JogadaEvent : EventArgs
    {
        public Message jogada { get; set; }
    }
}
