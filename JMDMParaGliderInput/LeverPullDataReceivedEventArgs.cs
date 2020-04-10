using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMParaGliderInput
{
    public class LeverPullDataReceivedEventArgs : EventArgs
    {
        public byte Level { get; private set; }

        internal LeverPullDataReceivedEventArgs(byte Level)
        {
            this.Level = Level;
        }
    }
}
