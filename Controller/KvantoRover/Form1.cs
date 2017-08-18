using MjpegProcessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KvantoRover
{
    public partial class Form1 : Form
    {
        MjpegDecoder _mjpeg;

        public Form1()
        {
            InitializeComponent();
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            // GoFullscreen(true);
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

        private void button1_Click(object sender, EventArgs e)
        {
            _mjpeg.ParseStream(new Uri("http://192.168.70.3:8099"));
            // player.con
            //player.playlist.add("http://192.168.70.3:8099");
            //yer.playlist.play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient("192.168.70.3", 9999);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(textBox1.Text);
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
