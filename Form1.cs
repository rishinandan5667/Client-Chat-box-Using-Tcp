using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tcpclient
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader read;
        public StreamWriter write;
        public string texttosend;
        public string texttorecieve;
        public Form1()
        {
            InitializeComponent();
        }

        //client side connection with sever
        private void button1_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint ipconnection = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));

            try
            {
                client.Connect(ipconnection);

                if (client.Connected)
                {
                    ChatDisplay.AppendText("Connected to Server" + "\n");
                    read = new StreamReader(client.GetStream());
                    write = new StreamWriter(client.GetStream());
                    write.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                }
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message.ToString());
            }
        }

        //background work of receiving message
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while(client.Connected)
            {
                try
                {
                    texttorecieve = read.ReadLine();
                    this.ChatDisplay.Invoke(new MethodInvoker(delegate ()
                    {
                        ChatDisplay.AppendText("Server: " + texttorecieve +"\n");
                    }));

                    texttorecieve = "";
                }
                catch(Exception er)
                {
                    MessageBox.Show(er.Message.ToString());
                }

            }
        }

        //background work of sending message
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                write.WriteLine(texttosend);
                this.ChatDisplay.Invoke(new MethodInvoker(delegate ()
                {
                    ChatDisplay.AppendText("Me: " + texttosend + "\n");
                }));
            }
            else
            {
                MessageBox.Show("Sending Failed");
            }

            backgroundWorker2.CancelAsync();
        }

        //after typing the message in textbox to send from client
        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox3.Text != "")
            {
                texttosend = textBox3.Text;
                backgroundWorker2.RunWorkerAsync();
            }

            textBox3.Clear();
        }
    }
}