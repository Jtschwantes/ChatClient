using System;
using System.Windows.Forms;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        // Private fields
        private TcpClient server = new TcpClient();
        private NetworkStream stream = null;
        private string message = null;

        // Constructor (It's kinda like the main execution for forums projects)
        public Form1()
        {
            InitializeComponent();
        }

        // The connect button
        private void button1_Click(object sender, EventArgs e)
        {
            // Return if the client is already connected
            if (server.Connected) 
                return;

            // Try to connect
            try
            {
                server.Connect(IPAddress.Parse("192.168.0.202"), 100);
            }
            // Write a message if unable
            catch (Exception ex)
            {
                message = "Connection failed.";
                display();
                return;
            }
            // Write success message
            message = "Connected to Chat Server.";
            display();
            // Grab stream from socket
            stream = server.GetStream();

            // Grab text box info and send to server
            byte[] data = Encoding.ASCII.GetBytes(textBox1.Text + "%^&");
            stream.Write(data, 0, data.Length);
            stream.Flush();

            // Multithread to continually recieve messages
            Task ctThread = new Task(getMessage);
            ctThread.Start();
        }
        // The send button
        private void button2_Click(object sender, EventArgs e)
        {
            SendServerMessage(textBox2.Text);
        }
        // Disconnect
        private void button3_Click(object sender, EventArgs e)
        {
            server.Close();
        }

        // Send message to server
        private void SendServerMessage(string msg)
        {
            if(String.IsNullOrEmpty(msg))
            {
                message = "Please type something in before you send it";
                display();
                return;
            }
            byte[] data = Encoding.ASCII.GetBytes(msg + "%^&");
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }

        // Continually listen for messages
        private void getMessage()
        {
            while (true)
            {
                try
                {
                    stream = server.GetStream();
                    int buffSize = 2048;
                    byte[] inStream = new byte[2048];
                    stream.Read(inStream, 0, buffSize);
                    string returndata = Encoding.ASCII.GetString(inStream);
                    message = "" + returndata;
                    display();
                }
                catch
                {
                    server.Close();
                    return;
                }
            }
        }

        // Display messages to user
        private void display()
        {
            if (this.InvokeRequired) 
                this.Invoke(new MethodInvoker(display));
            else
                listBox1.Items.Add(message);
        }

    }
}