using MjpegProcessor;
using System;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using DeviceManagement;

namespace KvantoRover
{
    public partial class Form1 : Form
    {
        MjpegDecoder mjpeg;
        DeviceInfo xbox;
        TcpClient tcpClient;
        NetworkStream tcpStream;

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
            String t = "";
            if(Keyboard.IsKeyDown(Key.Up))
            {
                t += "X:255;";
            } else if(Keyboard.IsKeyDown(Key.Down)) {
                t += "X:-255;";
            } else
            {
                t += "X:0;";
            }
            if (Keyboard.IsKeyDown(Key.Left))
            {
                t += "Y:255;";
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {
                t += "Y:-255;";
            } else
            {
                t += "Y:0;";
            }
            if(tbCommand.Text != t)
            {
                tbCommand.Text = t;
                if(connected)
                {
                    try
                    {
                        sendCmd(t);
                    } catch(Exception err)
                    {
                        tbResponse.Text = err.Message;
                    }
                }
            }
            //Keyboard.IsKeyDown(Key.Return)
            //DisplayControllerInformation();
        }

        private void sendCmd(String cmd)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd + "\n");
            tcpStream.Write(data, 0, data.Length);
            data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes = tcpStream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            tbResponse.Text = responseData;
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
        }

        private void disconnect()
        {
            if(connected)
            {
                mjpeg.StopStream();
                tcpStream.Close();
                tcpClient.Close();
                btnConnect.Text = "Connect";
                tbRobotAddr.Enabled = true;
                connected = false;
            }
        }

        private void connect()
        {
            if(!connected)
            {
                try
                {
                    mjpeg.ParseStream(new Uri("http://" + tbRobotAddr.Text + ":8099"));
                    tcpClient = new TcpClient(tbRobotAddr.Text, 9999);
                    tcpStream = tcpClient.GetStream();
                    sendCmd("RESET");
                    btnConnect.Text = "Disconnect";
                    tbRobotAddr.Enabled = false;
                    connected = true;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (connected)
                disconnect();
            else
                connect();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            disconnect();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
