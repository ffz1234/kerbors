using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace Vserver1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket connection;
        private TcpListener listener;
        private IPAddress ip;
        private Int32 port;
        public string Time = DateTime.Now.ToString("yyyy/MM/dd HH：mm：ss");
        private static byte[] result = new byte[1000];
        public MainWindow()
        {
            string host = GetLocalIP();
            IPAddress ip = IPAddress.Parse(host);
            InitializeComponent();
            this.ip = ip;
            this.port = 8000;
        }
        //侦听客户连接请求
        public void runAs()
        {

            while (true)
            {

                this.Dispatcher.Invoke(new Action(() => { T1.AppendText("Waiting for Connection\r\n"); }));
                connection = listener.AcceptSocket();
                //在新线程中启动新的socket连接，每个socket等待，并保持连接

                IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
                this.Dispatcher.Invoke(new Action(() => { T1.AppendText("远程主机:" + iprm.Address.ToString() + ":" + iprm.Port.ToString() + "连接上本机\r\n"); }));
                Thread thread = new Thread(new ThreadStart(dealClient));
                Thread myThread = new Thread(dealClient);
                thread.Start();

            }
        }

        //和客户端对话
        private void dealClient()
        {
            Socket connection = this.connection;
            IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
            this.Dispatcher.Invoke(new Action(() => { T2.AppendText("准备接受消息！\n"); }));

            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start(connection);
        }

        //处理客户端发过来的消息。
        /*   public Message DealMsg1(string msg5)
           {
               Message a = new Message();
               string type;
               string pwd;
               string tag;
               string ticket;
               string Authenticaorc;
               string Mticket;
               string MAuthenticaorc;
               string IDC;
               string ADC;
               string IDv;
               string TS4;
               string Lifetime4;
               string TS5;
               string IDC1;
               string ADC1;
               type = msg5.Substring(0, 2);
               pwd = msg5.Substring(2, 4);
               tag = msg5.Substring(6, 2);
               string tkA;
               tkA = msg5.Substring(8, msg5.Length - 8);
               //Console.WriteLine(tkA);
               string[] sArray = Regex.Split(tkA, "####", RegexOptions.IgnoreCase);
               Mticket = sArray[0];
               MAuthenticaorc = sArray[1];
               ticket = DesDecrypt(Mticket,"12345678");
               Authenticaorc = DesDecrypt(MAuthenticaorc, "12345678");
               string[] sArray1 = Regex.Split(ticket, "####", RegexOptions.IgnoreCase);
               IDC =sArray1[0].Substring(8,3);
               ADC = sArray1[1];
               IDv = sArray1[2].Substring(0, 3);
               TS4 = sArray1[2].Substring(3, Time.Length);
               Lifetime4 = sArray1[2].Substring(3 + Time.Length, 4);
               string[] sArray2 = Regex.Split(Authenticaorc, "####", RegexOptions.IgnoreCase);
               IDC1 = sArray2[0];
               ADC1 = sArray2[1];
               //int index2 = sArray2[2].IndexOf('\0');
               //int len2 = sArray2[2].Length;
               //sArray2[2].Remove(index2, len2 - index2);
               TS5 = sArray2[2];
               a = new Message(TS4,Lifetime4,IDC,ADC,IDv,TS5,IDC1,ADC1);
               return a;

           }*/
        private string StringToUnicode(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                // 取两个字符，每个字符都是右对齐。
                stringBuilder.AppendFormat("u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="unicode">Unicode.</param>
        private string UnicodeToString(string unicode)
        {
            string resultStr = "";
            string[] strList = unicode.Split('u');
            for (int i = 1; i < strList.Length; i++)
            {
                resultStr += (char)int.Parse(strList[i], System.Globalization.NumberStyles.HexNumber);
            }
            return resultStr;
        }
        public Message DealMsg7(string msg7)//处理信息7
        {
            Message a = new Message();
            string type;
            string pwd;
            string tag;
            string IDC;
            string TS;
            string operation;
            string bookname;
            type = msg7.Substring(0, 2);
            pwd = msg7.Substring(2, 4);
            if(type=="05")
            {
                string ticket;
                string Authenticaorc;
                string Mticket;
                string MAuthenticaorc;
                string ADC;
                string IDv;
                string TS4;
                string Lifetime4;
                string TS5;
                string IDC1;
                string ADC1;
                type = msg7.Substring(0, 2);
                pwd = msg7.Substring(2, 4);
                tag = msg7.Substring(6, 2);
                string tkA;
                tkA = msg7.Substring(8, msg7.Length - 8);
                //Console.WriteLine(tkA);
                string[] sArray = Regex.Split(tkA, "####", RegexOptions.IgnoreCase);
                Mticket = sArray[0];
                MAuthenticaorc = sArray[1];
                ticket = DecryptString(Mticket, "12345678");
                Authenticaorc = DecryptString(MAuthenticaorc, "12345678");
                string[] sArray1 = Regex.Split(ticket, "####", RegexOptions.IgnoreCase);
                IDC = sArray1[0].Substring(8, 3);
                ADC = sArray1[1];
                IDv = sArray1[2].Substring(0, 3);
                TS4 = sArray1[2].Substring(3, Time.Length);
                Lifetime4 = sArray1[2].Substring(3 + Time.Length, 4);
                string[] sArray2 = Regex.Split(Authenticaorc, "####", RegexOptions.IgnoreCase);
                IDC1 = sArray2[0];
                ADC1 = sArray2[1];
                //int index2 = sArray2[2].IndexOf('\0');
                //int len2 = sArray2[2].Length;
                //sArray2[2].Remove(index2, len2 - index2);
                TS5 = sArray2[2];
                a = new Message(TS4, Lifetime4, IDC, ADC, IDv, TS5, IDC1, ADC1);
            }
            if (type =="07")
            {
                string MM;
                string[] sArray1 = Regex.Split(msg7.Substring(6,msg7.Length-6), "####", RegexOptions.IgnoreCase);
                MM = sArray1[1];
                string CC;
                string rsa;
                CC = DecryptString(MM, "12345678");
                string[] sArray = Regex.Split(CC, "####", RegexOptions.IgnoreCase);
                tag = sArray[0].Substring(0, 2);
                IDC = sArray[0].Substring(2, 3);
                TS = sArray[0].Substring(5, Time.Length);
                operation = sArray[0].Substring(5 + Time.Length, 3);
                bookname = sArray[1];
                rsa = sArray[2];
               // MessageBox.Show(bookname);
               Console.WriteLine(bookname);
                a = new Message(operation, bookname, IDC,DateTime.Parse(TS),rsa);
            }
            if(type=="09")
            {
                string MM;
                string[] sArray1 = Regex.Split(msg7.Substring(6, msg7.Length - 6), "####", RegexOptions.IgnoreCase);
                MM = sArray1[0];
                string CC;
                string rsa;
                CC = DecryptString(MM, "12345678");
                string[] sArray = Regex.Split(CC, "####", RegexOptions.IgnoreCase);
                // tag = Int32.Parse(CC.Substring(0, 
                tag = sArray[0].Substring(0, 2);
                IDC = sArray[0].Substring(2, 3);
                operation = sArray[0].Substring(5, 3);
                rsa = sArray[1];
              // MessageBox.Show(operation);
               // MessageBox.Show(IDC);
               // MessageBox.Show(rsa);
               // MessageBox.Show(a.type);
                a = new Message(IDC,operation,rsa);

            }
            return a;
            }
        public static string DecryptString(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            // 如果两次密匙不一样，这一步可能会引发异常
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        public void ReceiveMessage(object ClientSocket)
        {
            Socket myClientSocket = (Socket)ClientSocket;
            IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
            while (true)
            {
                try
                {
                    //通过clientsocket接收数据
                    int num = myClientSocket.Receive(result);
                    //System.Windows.MessageBox.Show(Encoding.ASCII.GetString(result, 0, num));
                    //TB_recv_1.Text = "sss";
                    this.Dispatcher.Invoke(new Action(() => { T2.AppendText(Encoding.Unicode.GetString(result, 0, num)); }));

                    Thread sendThread = new Thread(SendMessage);
                    sendThread.Start(myClientSocket);
                    // Message c = DealMsg1(Encoding.ASCII.GetString(result));
                    //c.msg2_tkt_ADc = iprm.Address.ToString();
                    //this.Dispatcher.Invoke(new Action(() => { TextBox2.AppendText(c.MMessage(c)); }));
                    //string ssmg;
                    // ssmg=c.ssMessage(c);
                    //this.Dispatcher.Invoke(new Action(() => { TextBox4.AppendText(ssmg); }));
                    // Byte[] ssmg1 = new byte[1024];
                    // ssmg1=Encoding.ASCII.GetBytes(ssmg);
                    // int num1= myClientSocket.Send(ssmg1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //this.Dispatcher.Invoke(new Action(() => { TextBox3.AppendText("已断开连接"); }));
                    //myClientSocket.Shutdown(SocketShutdown.Both);
                    // myClientSocket.Close();
                   // break;
                }
            }
        }
        public void SendMessage(object clientSocket)
        {

           // while (true)
           // {
                Socket myClientSocket = (Socket)clientSocket;
                Message c = DealMsg7(Encoding.Unicode.GetString(result));
                //MessageBox.Show(c.type);
            if (c.type == "05")
                {
                    //Message b = new Message();
                    c.msg5_Au_TS5 = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    //this.Dispatcher.Invoke(new Action(() => { TextBox1.AppendText(c.msg2_TS2); }));
                    this.Dispatcher.Invoke(new Action(() => { T3.AppendText(c.MMessage5(c)); }));
                    DateTime ssmg = DateTime.Parse(c.msg5_Au_TS5).AddSeconds(1);
                    this.Dispatcher.Invoke(new Action(() => { T5.AppendText(ssmg.ToString("yyyy/MM/dd HH:mm:ss")); }));
                    // this.Dispatcher.Invoke(new Action(() => { TextBox1.AppendText(ssmg.Length.ToString()); }));
                    string Cssmg;
                    Cssmg = c.ssMessage(c);
                    this.Dispatcher.Invoke(new Action(() => { T4.AppendText(Cssmg); }));
                    Byte[] ssmg1 = new byte[1024];
                    ssmg1 = Encoding.ASCII.GetBytes(Cssmg);
                    //int num1 = myClientSocket.Send(ssmg1);
                    myClientSocket.Send(ssmg1, ssmg1.Length, 0);
               // Thread sendThread = new Thread(SendMessage);
               // sendThread.Start(myClientSocket);
                // myClientSocket.Close();
            }
                if (c.type == "09")
                {
                     c.dealmsg7(c);
               // MessageBox.Show(c.msg1_IDc);
                     string ssmg;
                    ssmg = c.ssMessage8(c);
                    //MessageBox.Show(ssmg);
                    Byte[] ssmg1 = new byte[1024];
                    ssmg1 = Encoding.Unicode.GetBytes(ssmg);
                    myClientSocket.Send(ssmg1, ssmg1.Length, 0);
                   // myClientSocket.Close();
            }
                if(c.type=="07")
            {
               // MessageBox.Show(c.msg7_operation);
                c.dealmsg7(c);
                string ssmg;
                ssmg = c.ssMessage8(c);
                //MessageBox.Show(ssmg);
                Byte[] ssmg1 = new byte[2048];
                ssmg1 = Encoding.Unicode.GetBytes(ssmg);
                myClientSocket.Send(ssmg1, ssmg1.Length, 0);
                //myClientSocket.Close();
            }
           // myClientSocket.Close();
            //}
        }
        public static TcpListener getListener(IPAddress address, Int32 port)
        {
            try
            {
                TcpListener listener = new TcpListener(address, port);
                listener.Start();
                return listener;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
            return null;
        }
        public static string GetLocalIP()
        {
            try
            {

                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ip = "";
                        ip = IpEntry.AddressList[i].ToString();
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            listener = getListener(ip, port);
            Thread thread = new Thread(new ThreadStart(runAs));
            thread.Start();
        }
    }

}

