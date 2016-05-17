using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;
using System.Reflection;



namespace WindowsFormsApplication7
{
    public partial class Form2 : FormUtil
    {
        public System.Windows.Forms.LinkLabel linkLabel1;
        public System.Windows.Forms.Label label1;
        private Button picButton = new Button();


        public Form2()
        {
            //this.Load += new EventHandler(Form2_Load);
            InitializeComponent();
            
        }
  

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("http://rotek.ru/");
        }


        private void Form2_Load(object sender, EventArgs e)
        {

            picButton.Size = new Size(360, 200);
            picButton.Location = new Point(70, 40);
            picButton.Enabled = false;
            picButton.BackgroundImage = Properties.Resources.rotek_logo_eng;
            picButton.BackgroundImageLayout = ImageLayout.Center;
            picButton.FlatAppearance.BorderSize = 0;
            picButton.FlatStyle = FlatStyle.Flat;
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.picButton });

            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(160, 390);
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.linkLabel1.Text = "Перейти на официальный сайт";
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.linkLabel1 });

            this.label1 = new System.Windows.Forms.Label();
            this.label1.Location = new System.Drawing.Point(195, 420);
            this.label1.Text = "Версия ПО: " + fwver;
            this.label1.Size = new Size(label1.PreferredWidth, label1.PreferredHeight);
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.label1 });
       
          
        }
    }
 }
