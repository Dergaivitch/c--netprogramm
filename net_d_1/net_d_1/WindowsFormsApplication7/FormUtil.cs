using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication7
{
    public partial class FormUtil : Form
    {
        protected Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public const String fwver = "1.1";  // подключение версии 1.1 
        // public const String fwver = "1.0";  // подключение версии 1.0 
        public const int UDP_TX_PORT = 49049;   // порт передачи сообщений
        public const int UDP_RX_PORT = 49049;   // порт приема сообщений
        protected UdpClient listener;
        public static List<IPAddress> MyIpList;
        protected const int delayTime = 500;
        protected bool SenderStop = true;
        protected bool flag = true;
        public FormUtil()
        {
            InitializeComponent();
        }



        protected void SendBroadcast(String message)
        {
            UdpClient client = new UdpClient();
            try
            {


                IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, UDP_TX_PORT);
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                client.Send(bytes, bytes.Length, ip);


            }
            catch (ThreadAbortException e)
            {
                MessageBox.Show("Ничего серьезного, попробуйте еще раз, но сли вы увидели данное сообщение то расскажите о нем разработчику", "Debug",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                //MessageBox.Show("Проблемы с отправкой на порт: " + UDP_TX_PORT, "Порт отправки занят",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Порт занят, попробуйте еще раз", "Порт отправки занят",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                client.Close();
            }
        }


        protected static List<IPAddress> GetMyIpList () {
            List<IPAddress> MyIpList = new List<IPAddress>();
            String strHostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
            MyIpList.Add(ipaddress);
            }
            return MyIpList;
        }
       
 
 

        public static bool CheckPortFree(int nPortNum)
        {
            bool bCheckResult = true;
            Socket s = null;

            try
            {
                s = new Socket(AddressFamily.InterNetwork,
          SocketType.Stream,
          ProtocolType.Tcp);
                s.Bind(new IPEndPoint(IPAddress.Loopback, nPortNum));

            }
           
            
            catch (Exception e)
            {
                MessageBox.Show("Порт занят сторонним приложением", "Порт Занят",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                bCheckResult = false;


            }
            finally
            {
                s.Close();
            }
           

            return bCheckResult;
        }

        private void FormUtil_Load(object sender, EventArgs e)
        {

        }
    }
}
