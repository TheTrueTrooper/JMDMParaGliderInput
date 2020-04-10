using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using JMDMParaGliderInput;
using JMDMParaGameUDPInterfaceNS;
using JMDMGameUDPInterface;
//using JMDM2PEggInput;
//using JMDMHanging2PFlight;
//using ButtonPressedDataReceivedEventArgs = JMDMHanging2PFlight.ButtonPressedDataReceivedEventArgs;

namespace JMDMParaInputInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Config Config = Config.LoadConfig();
            JMDMParaGliderInput_Com InputCom = new JMDMParaGliderInput_Com(Config.InputControlsComPort);
            JMDM_ServoPortControl ServoCom = new JMDM_ServoPortControl(Config.OutputServossComPort);
            JMDMParaGameUDPInterface ParaGame = new JMDMParaGameUDPInterface(ReceiveFromGamesInputPort: Config.ReceiveFromGameUDPPort, SendToGamesInputPort: Config.SendToGameUDPPort);
            
            ParaGame.DataReceiveEvent += (object sender, DataReceiveEventArgs e) =>
            {
                Console.WriteLine($"UDPDataRecived:{ASCIIEncoding.ASCII.GetString(e.Data)}");
            };
            ParaGame.MoveCommandReceivedEvent += (object sender, MoveCommandReceivedEventArgs e) =>
            {
                ServoCom.Send_Data(e.MotorNumber, e.Tilt);
            };
            InputCom.LeftInputDataReceivedEvent += (object sender, LeverPullDataReceivedEventArgs e) => { 
                Console.WriteLine($"LeftRecived:{e.Level}");
                ParaGame.SendRope(2, e.Level);
            };
            InputCom.RightInputDataReceivedEvent += (object sender, LeverPullDataReceivedEventArgs e) => { 
                Console.WriteLine($"RightRecived:{e.Level}");
                ParaGame.SendRope(1, e.Level);
            };
            ServoCom.Open_Port();
            InputCom.OpenPortAndListen();
            ParaGame.StartListenLoop();
            Console.ReadKey();

        }
    }
}
