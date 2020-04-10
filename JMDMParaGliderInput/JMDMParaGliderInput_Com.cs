using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JMDMParaGliderInput
{
    public class JMDMParaGliderInput_Com : IDisposable
    {
        public delegate void RightInputDataReceivedEventHandler(object sender, LeverPullDataReceivedEventArgs e);
        public delegate void LeftInputDataReceivedEventHandler(object sender, LeverPullDataReceivedEventArgs e);

        public event RightInputDataReceivedEventHandler RightInputDataReceivedEvent;
        public event LeftInputDataReceivedEventHandler LeftInputDataReceivedEvent;

        internal SerialPort InputListenCom { set; get; }

        internal string ListenPort { private set; get; }

        bool Disposed = false;

        Thread ListenerThread;

        internal bool IsOpen
        {
            get => InputListenCom.IsOpen;
        }

        public JMDMParaGliderInput_Com(string ListenPort)
        {
            this.ListenPort = ListenPort;
            InputListenCom = new SerialPort(ListenPort, 9600, Parity.None, 8, StopBits.One);
            InputListenCom.ReadTimeout = -1;

        }

        public void OpenPortAndListen()
        {
            if (!InputListenCom.IsOpen)
            {
                InputListenCom.Open();
                ListenerThread = new Thread(new ParameterizedThreadStart(Listening));
                ListenerThread.Start(this);
            }
        }

        void Listening(object ThisIn)
        {
            JMDMParaGliderInput_Com This = (JMDMParaGliderInput_Com)ThisIn;
            bool Disposed = false;
            lock (This)
                Disposed = This.Disposed;
            while (!Disposed)
            {
                //acuire a message
                //start the message
                char[] MessageType = new char[14];
                lock (This)
                    MessageType[0] = (char)This.InputListenCom.ReadByte();
                int Count = 1;
                //then read bytes until message done
                do
                {
                    lock (This)
                        MessageType[Count] = (char)This.InputListenCom.ReadByte();
                    Count++;
                }
                while (MessageType[Count - 1] != '\n' && Count < 14);
                //Dispach message
                switch (MessageType[0])
                {
                    case 'I':
                        {
                            //left I(2,250)
                            //right I(1,252)
                            byte InputLevel;
                            byte InputNumber;
                            string NumberString = "";
                            try
                            {
                                NumberString += MessageType[2];
                                InputNumber = byte.Parse(NumberString);
                                NumberString = "";
                                for(byte i = 4; i < 7; i++)
                                {
                                    NumberString += MessageType[i];
                                }
                                InputLevel = byte.Parse(NumberString);
                                if(InputNumber == 1)
                                    RightInputDataReceivedEvent.Invoke(this, new LeverPullDataReceivedEventArgs(InputLevel));
                                else if (InputNumber == 2)
                                    LeftInputDataReceivedEvent.Invoke(this, new LeverPullDataReceivedEventArgs(InputLevel));
                            }
                            //disgard any bad data
                            catch{}
                        }
                        break;
                }
                lock (This)
                    Disposed = This.Disposed;
            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                ListenerThread.Abort();
                InputListenCom.Dispose();
            }
        }

        ~JMDMParaGliderInput_Com()
        {
            if (!Disposed)
            {
                Disposed = true;
                ListenerThread?.Abort();
                InputListenCom?.Dispose();
            }
        }
    }
}
