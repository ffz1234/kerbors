using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace SC03
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket c;
        // des des1 = new des();
        Window a = new AS();
       
        public MainWindow()
        {
            a.Show();
            InitializeComponent();
        }
        private void button_click1(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(textbox2.Text);
            string host = textbox1.Text;
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例
            c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
            try
            {
                c.Connect(ipe);//连接到服务器
                System.Windows.MessageBox.Show("连接AS服务器成功！");
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show("连接超时！");
            }         
        }
        private void button_click2(object sender, RoutedEventArgs e)// 
        {
            textbox5.Text = "正在向AS服务器发送消息...";
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);            //获取时间戳
            string sendStr = textbox3.Text + "TGS" + Convert.ToInt64(ts.TotalSeconds).ToString();
            byte[] bs = Encoding.ASCII.GetBytes(sendStr);
            c.Send(bs, bs.Length, 0);
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            bytes = c.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息
            recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
        }
            /*textbox5.Text = recvStr;
            string recvstr1 = des1.DecryptString(recvStr,textbox4.Text);
            if (recvstr1.Substring(8, 3) == "TGS")
            {
                textbox5.Text = "登陆成功！";
               // Window f1 = new Window1();
               // f1.ShowDialog();
            }*/
            ///else
               // textbox5.Text = "登陆失败！";
        }
    }

