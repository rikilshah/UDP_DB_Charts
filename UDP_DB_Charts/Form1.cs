using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace UDP_DB_Charts
{
    public partial class Form1 : Form
    {
        delegate void StringArgReturningVoidDelegate(string text);
        
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread theUdpServer = new Thread(new ThreadStart(serverThread));
            try
            {
                theUdpServer.Start();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                theUdpServer.Abort();
            }
        }

        public void serverThread()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8090);
            UdpClient client = new UdpClient(ipep);
            string passText;
            while(true)
            {
                IPEndPoint RemoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = client.Receive(ref RemoteIPEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                //Gui Element for Text Output
                //SetText(RemoteIPEndPoint.Address.ToString() + ":" + returnData.ToString());
                passText = RemoteIPEndPoint.Address.ToString() + ":" + returnData.ToString();
                this.backgroundWorker1.RunWorkerAsync(passText);
                
            }
        }

        private void SetText(string text)
        {
            if(this.listBox1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox1.Items.Add(text);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //this.listBox1.Items.Add(e.Argument);
            string text = (string)e.Argument;
            e.Result = text;
            Console.WriteLine(e.Result);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // The user canceled the operation.
                MessageBox.Show("Operation was canceled");
            }
            else if (e.Error != null)
            {
                // There was an error during the operation.
                string msg = String.Format("An error occurred: {0}", e.Error.Message);
                MessageBox.Show(msg);
            }
            else
            {
                // The operation completed normally.
                string msg = String.Format("Result = {0}", e.Result);
                MessageBox.Show(msg);
                Console.WriteLine("Result is {0} ",e.Result);
            }
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            backgroundWorker1.CancelAsync();
        }
    }
}

