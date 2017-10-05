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
using System.Data.Sql;
using System.Data.SqlClient;

namespace UDP_DB_Charts
{
    
    public partial class Form1 : Form
    {
        private UdpClient client;
        public string conString = "user id=admin;" +
           "password=123;server=roarbit-pc\\sqlexpress;" +
           "database=udpdb;" +
           "connection timeout=30;" +
           "Trusted_Connection=yes;";

        public Form1()
        {
            InitializeComponent();
            //Create UDPClient and Start Listening
            client = new UdpClient(9090);
            client.BeginReceive(DataReceived, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            
        }

        private void DataReceived(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 8090);
            byte[] data;

            try
            {
                data = client.EndReceive(ar, ref ip);

                if(data.Length == 0)
                {
                    return;
                }
                client.BeginReceive(DataReceived, null);
            }
            catch(ObjectDisposedException)
            {
                return;
            }
            this.BeginInvoke((Action<IPEndPoint, string>)DataReceivedUI, ip, Encoding.ASCII.GetString(data));
        }

        private void DataReceivedUI(IPEndPoint ipendpoint, string data)
        {
            this.listBox1.Items.Add(ipendpoint.ToString() + ":" + data + Environment.NewLine);

            using (SqlConnection connection = new SqlConnection(conString))
            {
                string commandString = "INSERT INTO baseTable(Date,Status) Values(@Date,@Status)";
                SqlCommand myCommand = new SqlCommand(commandString,connection);

                myCommand.Parameters.Clear();
                myCommand.Connection = connection;
                myCommand.Parameters.Add(new SqlParameter("Date", data.ToString()));
                myCommand.Parameters.Add(new SqlParameter("Status", 1));

                try
                {
                    connection.Open();
                    int rowAffec = myCommand.ExecuteNonQuery();
                    Console.WriteLine("RowsAffected: {0}", rowAffec);
                }
                catch(Exception error)
                {
                    Console.WriteLine(error.Message);
                }

                try
                {
                    connection.Close();
                }
                catch(Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }
    }
}

