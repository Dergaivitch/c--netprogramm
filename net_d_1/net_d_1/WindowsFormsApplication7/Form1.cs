//#define debugg
#define releasee
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;





namespace WindowsFormsApplication7
{
    public partial class Form1 : FormUtil 
    {

        private Panel buttonPanel = new Panel();
        private DataGridView mainDataGridView = new DataGridView();
        private Button outputButton = new Button();
        private Button about_progButton = new Button();
        private System.Windows.Forms.Label label_find;
        private List<String> messages = new List<String>();
        Form3 ipconfig_form;


        // для проверки максимальной ширины полей
#if (debugg)
        private List<String> SendMessages = new List<String> {"B1-6037566767(v2);KN-03-00072;AA-B4-CC-EE-DD-FF;1.1;;;2a5d1dd9baae3c122b3bee491cf7ad2c36337795dbefb687a618c9048e7423ba;;;T2OK;;",
            "B1-6035(v2);KN-03-00045;AA-B7-CC-EE-DD-FF;1.1;;;2a5d1dd9baae3c122b3bee491cf7ad2c36337795dbefb687a618c9048e7423ba;;;T2OK;;",
            "B1-6036(v2);KN-03-00006;A7-B4-CC-EE-DD-FF;1.1;;;2a5d1dd9baae3c122b3bee491cf7ad2c36337795dbefb687a618c9048e7423ba;;;T2OK;;" };
#endif
#if(releasee)
        private List<String> SendMessages = new List<String> { "partno;serno;MAC;sw version;;;stm32id;;prodate;status;;" };
#endif       

        private void Sender()
        {
            try
            {
                while (true)
                {
                    for (int i = 0; i < SendMessages.Count; i++)
                    {

                        SendBroadcast(SendMessages[i]);

                    }
                    Thread.Sleep(delayTime);

                }
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
                return;
            }

        }

        /* Функция StsrtListener() для прослушивания датаграмм UDP, отправляемых непосредственно широковещательному адресу на порту 49049
         Клиент получает строку сообщения, которые создают список текстовых данных */
        protected void StartListener(List<String> SendMessages)
        {
            SenderStop = false;
            Thread sender = new Thread(this.Sender);
            sender.Start();
            try
            {
                if (CheckPortFree(UDP_RX_PORT))
                    listener = new UdpClient(UDP_RX_PORT);
                else return;
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, UDP_RX_PORT);
                MyIpList = GetMyIpList();


                DateTime startTime = DateTime.Now;


                while (true)
                {


                    if (listener.Available > 0)
                    {
                        byte[] bytes = listener.Receive(ref groupEP);
#if(debugg)
                        if ((!messages.Contains(Encoding.UTF8.GetString(bytes, 0, bytes.Length) + groupEP.Address.ToString())) && (true))
#endif
#if(releasee)
                            if ((!messages.Contains(Encoding.UTF8.GetString(bytes, 0, bytes.Length) + groupEP.Address.ToString())) && (!MyIpList.Contains(groupEP.Address)))
#endif
                        {
                            messages.Add(Encoding.UTF8.GetString(bytes, 0, bytes.Length) + groupEP.Address.ToString());

                        }
                    }

                    if (DateTime.Now.Subtract(startTime) >= new TimeSpan(0, 0, 0, 0, 3000)) // функция выполняется в течении трех секунд
                        break;
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Неудачная попытка использования порта: закройте другие экземляры программы и попробуйте заново", "Порт Занят",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                sender.Abort();
                listener.Close();
                
            }
                
        }

        public Form1()
        {
            this.Load += new EventHandler(Form1_Load);

        }

        private void Form1_Load(System.Object sender, System.EventArgs e)
        {
            SetupLayout();                           // настраивает панель с кнопкой и кнопки
            SetupDataGridView();                     // настраивает таблицу

        }

        // Событие по нажатию кнопки "Пуск" 
        private void outputButton_Click(object sender, EventArgs e)
        {
            mainDataGridView.Rows.Clear();          //очищает строки таблицы перед выводом новых данных
            this.StartListener(SendMessages);
            outputDataGridView();    

        }

        // Событие по клику кнопки "О программе". Вызывает вторую форму.
        private void about_progButton_Click(object sender, EventArgs e)
        {
            Form2 inf_form = new Form2();
            inf_form.Show();
            inf_form.MaximizeBox = false;
            inf_form.FormBorderStyle = FormBorderStyle.Fixed3D;
            inf_form.Size = new Size(500, 500);
            inf_form.Text = "Network Discovery (ROTEK)";
            inf_form.StartPosition = FormStartPosition.CenterScreen;

        }


        private void CellDoubleClick_row(object sender, EventArgs e)
        {
            if (!(ipconfig_form == null) && !(ipconfig_form.IsDisposed))
                ipconfig_form.Close();
            ipconfig_form = new Form3(mainDataGridView.Rows[mainDataGridView.CurrentCell.RowIndex].Cells["IP-адрес"].Value.ToString(), mainDataGridView.Rows[mainDataGridView.CurrentCell.RowIndex].Cells["Серийный номер"].Value.ToString());
                ipconfig_form.Show();
                ipconfig_form.MaximizeBox = false;
                ipconfig_form.FormBorderStyle = FormBorderStyle.Fixed3D;
                ipconfig_form.Size = new Size(380, 350);
                ipconfig_form.Text = "Сетевые параметры";
                ipconfig_form.StartPosition = FormStartPosition.CenterScreen;
               
        }
    
    

        private void SetupLayout()
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.Size = new Size(540, 500);
            this.Text = "Network Discovery (ROTEK)";
            this.StartPosition = FormStartPosition.CenterScreen;


            outputButton.Text = "Пуск";
            outputButton.Location = new Point(20, 15);
            outputButton.Click += new EventHandler(outputButton_Click);

            about_progButton.Text = "О программе";
            about_progButton.Location = new Point(400, 13);
            about_progButton.Size = new Size(100, 25);
            about_progButton.Click += new EventHandler(about_progButton_Click);

            label_find = new System.Windows.Forms.Label();
            label_find.Location = new System.Drawing.Point(120, 20);
            label_find.Size = new Size(label_find.PreferredWidth, label_find.PreferredHeight);
            buttonPanel.Controls.Add(label_find);
     
            buttonPanel.Controls.Add(outputButton);
            buttonPanel.Controls.Add(about_progButton);
            buttonPanel.Height = 50;
            buttonPanel.Dock = DockStyle.Bottom;

            this.Controls.Add(this.buttonPanel);
        }

        private void SetupDataGridView()
        {
            this.Controls.Add(mainDataGridView);

            mainDataGridView.BackgroundColor = Color.WhiteSmoke;
            mainDataGridView.ColumnCount = 5;
            mainDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            mainDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            mainDataGridView.ColumnHeadersDefaultCellStyle.Font =
                new Font(mainDataGridView.Font, FontStyle.Bold);
           
            mainDataGridView.MaximumSize = new Size (550, 430);

            mainDataGridView.Columns[0].Width = 120;
            mainDataGridView.Columns[1].Width = 84;
            mainDataGridView.Columns[2].Width = 110;
            mainDataGridView.Columns[3].Width = 124;
            mainDataGridView.Columns[4].Width = 80;


            mainDataGridView.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            mainDataGridView.AllowUserToResizeRows = false;
            mainDataGridView.RowHeadersWidthSizeMode =
                DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            mainDataGridView.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;
            mainDataGridView.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Raised;
            mainDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            mainDataGridView.GridColor = Color.Black;
            mainDataGridView.RowHeadersVisible = false;

            mainDataGridView.Columns[0].Name = "Серийный номер";
            mainDataGridView.Columns[1].Name = "IP-адрес";
            mainDataGridView.Columns[2].Name = "MAC-адрес";
            mainDataGridView.Columns[3].Name = "Модель";
            mainDataGridView.Columns[4].Name = "Версия ПО";

            mainDataGridView.Columns[0].ReadOnly = true;
            mainDataGridView.Columns[1].ReadOnly = true;
            mainDataGridView.Columns[2].ReadOnly = true;
            mainDataGridView.Columns[3].ReadOnly = true;
            mainDataGridView.Columns[4].ReadOnly = true;

            mainDataGridView.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            mainDataGridView.MultiSelect = false;
            mainDataGridView.Dock = DockStyle.Fill;

            if (fwver.Equals("1.1"))
            mainDataGridView.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(CellDoubleClick_row);
            

            mainDataGridView.AllowUserToAddRows = false;

        }
        //функция вывода udp сообщений в виде списка в таблицу и подсчет найденных устройств
        private void outputDataGridView() 
        {
                for (int i = 0; i < messages.Count; i++)
                {
                    String[] substring = Regex.Split(messages[i], ";");
                    if (substring.Length == 12)
                    {
                        string[] row = { substring[1], substring[11], substring[2], substring[0], substring[3] };
                        mainDataGridView.Rows.Add(row);
                    }
                    
                }
            
                buttonPanel.Controls.Remove(label_find);
                label_find = new System.Windows.Forms.Label();
                label_find.Location = new System.Drawing.Point(120, 20);
                
                label_find.Text = "Найдено " + (mainDataGridView.RowCount);
                label_find.Size = new Size(label_find.PreferredWidth, label_find.PreferredHeight);
                buttonPanel.Controls.Add(label_find);
                messages.Clear();
               
                
        }
    }
}