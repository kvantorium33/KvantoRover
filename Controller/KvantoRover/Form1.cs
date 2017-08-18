using MjpegProcessor;
using System;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using DeviceManagement;

namespace KvantoRover
{
    public partial class Form1 : Form
    {
        MjpegDecoder mjpeg;
        DeviceInfo xbox;
        TcpClient tcpClient;
        DispatcherTimer timer = new DispatcherTimer();
        bool connected = false;

        public Form1()
        {
            InitializeComponent();
            mjpeg = new MjpegDecoder();
            mjpeg.FrameReady += mjpeg_FrameReady;
            // GoFullscreen(true);
            var allClasses = DeviceInfoSet.GetAllClassesPresent();
            var devices = allClasses.GetDevices();
            xbox = (from device in devices where device.ClassName == "XboxComposite" select device).FirstOrDefault();
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += timerTick;
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {

            //DisplayControllerInformation();
        }

        private void GoFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }

        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            image.Image = e.Bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient("192.168.70.3", 9999);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(tbRobotAddr.Text);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            MessageBox.Show(responseData);
            stream.Close();
            client.Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(connected)
            {
                mjpeg.StopStream();
                tcpClient.Close();
                btnConnect.Text = "Connect";
                tbRobotAddr.Enabled = true;
                connected = false; 
            } else
            {
                try
                {
                    mjpeg.ParseStream(new Uri("http://" + tbRobotAddr.Text + ":8099"));
                    tcpClient = new TcpClient(tbRobotAddr.Text, 9999);
                    btnConnect.Text = "Disconnect";
                    tbRobotAddr.Enabled = false;
                    connected = true;
                } catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
    }
}
