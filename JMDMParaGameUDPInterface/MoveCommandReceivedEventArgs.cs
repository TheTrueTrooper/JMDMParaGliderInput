using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMDMParaGameUDPInterfaceNS
{
    /// <summary>
    /// An event argument to handle the Data received
    /// </summary>
    public class MoveCommandReceivedEventArgs : EventArgs
    {
        public byte MotorNumber { get; private set; }
        public byte Tilt { get; private set; }

        public DateTime DateTime = DateTime.UtcNow;

        internal MoveCommandReceivedEventArgs(byte MotorNumber, byte Tilt)
        {
            this.Tilt = Tilt;
            this.MotorNumber = MotorNumber;
        }
    }
}
