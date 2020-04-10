using JMDMGameUDPInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static JMDMGameUDPInterface.GameUDPInterface;

namespace JMDMParaGameUDPInterfaceNS
{
    public class JMDMParaGameUDPInterface : IDisposable
    {

        /// <summary>
        /// A event delegate to handle the event
        /// </summary>
        /// <param name="Sender">a generalized object to hold the sender (rarely actually used except for loging)</param>
        /// <param name="e">the event arg with the Data for the event</param>
        public delegate void MoveCommandReceivedEventHandler(object Sender, MoveCommandReceivedEventArgs e);

        /// <summary>
        /// An event to handle the Received data
        /// </summary>
        public event MoveCommandReceivedEventHandler MoveCommandReceivedEvent;

        /// <summary>
        /// An event to handle the Received data
        /// </summary>
        public event DataReceiveEventHandler DataReceiveEvent
        {
            add => UDPGameConnections.DataReceiveEvent += value;
            remove => UDPGameConnections.DataReceiveEvent += value;
        }

        /// <summary>
        /// the Port that the game will send info to this.
        /// </summary>
        public int? ReceiveFromGamesInputPort
        {
            get
            {
                return UDPGameConnections?.ReceiveFromGamesInputPort;
            }
        }

        public int? SendToGamesInputPort
        {
            get
            {
                return UDPGameConnections?.SendToGamesInputPort;
            }
        }

        public IPAddress ReceiveFromGamesInputAddress
        {
            get
            {
                return UDPGameConnections?.ReceiveFromGamesInputAddress;
            }
        }

        public IPAddress SendToGamesInputAddress
        {
            get
            {
                return UDPGameConnections?.SendToGamesInputAddress;
            }
        }

        GameUDPInterface UDPGameConnections;

        public bool IsDisposed { get; private set; } = false;


        public JMDMParaGameUDPInterface(int ReceiveFromGamesInputPort = 5000, int SendToGamesInputPort = 1200, string ReceiveFromGamesInputPortIP = "127.0.0.1", string SendToGamesInputPortInputPortIP = "127.0.0.1")
        {
            UDPGameConnections = new GameUDPInterface(ReceiveFromGamesInputPort, SendToGamesInputPort, ReceiveFromGamesInputPortIP, SendToGamesInputPortInputPortIP);
            DataReceiveEvent += SelfhandleDataReceiveEvent;
        }

        public void StartListenLoop()
        {
            UDPGameConnections.StartListenLoop();
        }

        private void SelfhandleDataReceiveEvent(object Sender, DataReceiveEventArgs e)
        {
            const string MoveOrderConst = "OUT(";
            string Message = ASCIIEncoding.ASCII.GetString(e.Data);

            if (Message.Contains(MoveOrderConst))
            {
                MoveCommandReceivedEvent.Invoke(this, new MoveCommandReceivedEventArgs(byte.Parse(Message.Substring(8, 1)), byte.Parse(Message.Substring(4, 3))));
            }
        }



        void SendRawPacket(ref byte[] Data)
        {
            UDPGameConnections.SendRawPacket(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RopeNumber">rope number 1-2</param>
        /// <param name="TriggerLevel">the value inputed</param>
        public void SendRope(byte RopeNumber, byte TriggerLevel)
        {
            const string Message = "IN({0:000},{1})";
            byte[] Data = ASCIIEncoding.ASCII.GetBytes(string.Format(Message, TriggerLevel, RopeNumber));
            SendRawPacket(ref Data);
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                UDPGameConnections?.Dispose();
            }
        }

        ~JMDMParaGameUDPInterface()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                UDPGameConnections?.Dispose();
            }
        }
    }
}
