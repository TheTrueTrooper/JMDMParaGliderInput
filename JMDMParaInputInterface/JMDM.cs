using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO.Ports;

namespace JMDMParaInputInterface
{

    public class JMDM_ServoPortControl : IDisposable
    {
        bool IsDisposed = false;

        private string send_port;
        private SerialPort sp_send = null;

        public JMDM_ServoPortControl(string sendport)
        {
            send_port = sendport;
            sp_send = new SerialPort(send_port, 9600, Parity.None, 8, StopBits.One);
            sp_send.ReadTimeout = 400;
        }

        public void Open_Port()
        {
            //try
            //{
                if (!sp_send.IsOpen)
                {
                    sp_send.Open();
                }
            //}
            //catch (Exception ex)
            //{
            //    //Debug.Log("Port_Error:" + ex.Message);
            //}
        }

        //num  1-6   height  10-250    3个缸的行程控制
        public void Send_Data(int num, int height, bool limit = true)
        {
            if (num < 0 || num > 7)
            {
                return;
            }
            if (limit)
            {
                if (height < 10)
                {
                    height = 10;
                }
                else if (height > 250)
                {
                    height = 250;
                }
            }

            string str = string.Format("OC(0{0},{1:D3})", num, height);
            if (sp_send.IsOpen)
            {
                sp_send.Write(str);
            }
        }

        public void Close_Port()
        {
            if (sp_send.IsOpen)
            {
                sp_send.Close();
            }
        }

        public void Dispose()
        {
            if(!IsDisposed)
            {
                IsDisposed = true;
                sp_send.Dispose();
            }
        }

        ~JMDM_ServoPortControl()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                sp_send.Dispose();
            }
        }
    }
}
