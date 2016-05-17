using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json.Linq;
 
namespace WindowsFormsApplication7
{
    public partial class Form3 : FormUtil
    {

          private List<String> messages = new List<String>();
          Boolean isclosed = true;
          String serno;
          String device_ip;

          
        public Form3(String device_ip, String serno)
        {
            
            this.serno = serno;
            this.device_ip = device_ip;
            Thread ask = new Thread(this.StartListener);
            ask.Start();
            InitializeComponent();
            Setup();

        }

        private void Sender()
        {
            try {
            while (true) {
                SendBroadcast("{" + "\"serno\":" + '"' + serno + '"'  +"," + "\"dhcp\":" + "\"\"" + "," + "\"ipaddress\":" + '"' + '"' + "," + "\"gateway\":" + '"' + '"' + ","
        + "\"mask\"" + ":" + '"'+ '"' + "}");
            Thread.Sleep(delayTime);
            }
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
                return;
            }
            

        }

        private void SetParams()
        {
            if (messages.Count > 0)
            {
                JObject recievedMessage = JObject.Parse(messages[0]);
                textBox1.Text = (string)recievedMessage["ipaddress"];
                textBox2.Text = (string)recievedMessage["gateway"];
                textBox3.Text = (string)recievedMessage["mask"];

            }
        }
        
        private void StartListener()
            
        {
            SenderStop = false;
            Thread sender = new Thread(this.Sender);
            sender.Start();
            try
            {
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, UDP_RX_PORT);
            listener = new UdpClient(UDP_RX_PORT);
            isclosed = false;
           
                DateTime startTime = DateTime.Now;

                
                while (true)
                {
                    
                            
                    if (listener.Available > 0)
                    {
                        byte[] bytes = listener.Receive(ref groupEP);

                        if ((!MyIpList.Contains(groupEP.Address))
                            && (Encoding.UTF8.GetString(bytes, 0, bytes.Length).StartsWith("{" + "\"serno\":" + '"' + serno + '"')))
                        {
                            messages.Add(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                            BeginInvoke(new MethodInvoker(delegate
                            {
                                SetParams();
                            }));
                            break;
                        }
                    }
                    
                    if (DateTime.Now.Subtract(startTime) >= new TimeSpan(0, 0, 0, 0, 3000))// функция выполняется в течении трех секунд
                    {
                        MessageBox.Show("Сетевые параметры этого устройства не были получены, попробуйте еще раз", "Настройки не получены",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                sender.Abort();
                listener.Close();
                isclosed = true;


            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;


               
            }
            else
            {
                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;

            }
           
            
                
         }

        //Кнопка "Применить"
        private void apply_button_click(object sender, EventArgs e)
        {
            if (!textBox1.ForeColor.Equals(Color.Green) || !textBox2.ForeColor.Equals(Color.Green) || !textBox3.ForeColor.Equals(Color.Green))
            {
                MessageBox.Show("Проверьте правильность настроек", "Неправильный формат данных",
                       MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }

            SendBroadcast("{" + "\"serno\":" + '"' + serno + '"' + "," + "\"dhcp\":" + "\"false\"" + "," + "\"ipaddress\":" + '"' + textBox1.Text + '"' + "," + "\"gateway\":" + '"' + textBox2.Text + '"' + ","
    + "\"mask\"" + ":" + '"' + textBox3.Text + '"' + "}");
            messages.Clear();
            base.Close();

        }
        //Кнопка "Сброс"
        private void reset_button_click(object sender, EventArgs e)
        {
            SendBroadcast("{" + "\"serno\":" + '"' + serno + '"' + "," + "\"dhcp\":" + "\"true\"" + "," + "\"ipaddress\":" + '"' + "192.168.0.254" + '"' + "," + "\"gateway\":" + '"' + "192.168.0.1" + '"' + ","
    + "\"mask\"" + ":" + '"' + "255.255.255.0" + '"' + "}");
            messages.Clear();
            base.Close();
        }
      
        
        private void Setup()
        {
            this.textBox1.TextChanged += textBox1_TextChanged;
            this.textBox2.TextChanged += textBox2_TextChanged;
            this.textBox3.TextChanged += textBox3_TextChanged;
            this.FormClosing += Form3_FormClosing;
            this.textBox1.Text = device_ip;
            
            
            checkBox1.Checked=true;


        }

        

        void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isclosed)
                listener.Close();
            messages.Clear();
            this.Dispose();
            
            
        }
        // Верификация ввода трех полей
        void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Update();
            if (IsIpAddress(textBox1.Text))
            {
                textBox1.ForeColor = Color.Green;
            }
            else
            {
                textBox1.ForeColor = Color.Red;
            }   
        }


        void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.Update();
            if (IsIpAddress(textBox2.Text))
            {
                textBox2.ForeColor = Color.Green;
            }
            else
            {
                textBox2.ForeColor = Color.Red;
            } 
        }


        void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.Update();
            if (IsIpAddress(textBox3.Text))
            {
                textBox3.ForeColor = Color.Green;
            }
            else
            {
                textBox3.ForeColor = Color.Red;
            }
        }

       
        static bool IsIpAddress(string Address)
        {
            System.Text.RegularExpressions.Regex IpMatch =
                new System.Text.RegularExpressions.Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])(\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])){3}$");
            
            return IpMatch.IsMatch(Address);
        }

        private void Form3_Load(System.Object sender, System.EventArgs e)
        {
            
        }
    }
}
