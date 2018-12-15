using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        int stop = 0;
        public static IPAddress ipAd;
        public static TcpListener server;
        public static int count = 0;
        public static Socket[] socket;
        public static string[] userlogin;
        //2 mảng lưu trữ username và pass word, mặc định 2 user và 2 pasword
        public static string[] user = { "admin", "hung" };
        public static string[] password = { "admin", "hung" };
        int room = 6;
        public static List<List<int>> lsroom = new List<List<int>>();


        public Server()
        {

            InitializeComponent();

        }
        #region Các Hàm check
        int check3(int[,] arr, int x, int y)
        {
            for (int i = 0; i < 9; i++)
                if (arr[x, i] == arr[x, y] && i != y)
                {
                    return 0;
                }
            for (int j = 0; j < 9; j++)
                if (arr[j, y] == arr[x, y] && j != x)
                {
                    return 0;
                }
            int a = x / 3;
            int b = y / 3;
            for (int i = 3 * a; i < 3 * a + 3; i++)
                for (int j = 3 * b; j < 3 * b + 3; j++)
                    if (arr[i, j] == arr[x, y] && i != x && j != y)
                    {
                        return 0;
                    }
            return 1;
        }
        int check4(int[,] arr, int x, int y)
        {
            for (int i = 0; i < 16; i++)
                if (arr[x, i] == arr[x, y] && i != y)
                    return 0;
            for (int j = 0; j < 16; j++)
                if (arr[j, y] == arr[x, y] && j != x)
                    return 0;
            int a = x / 4;
            int b = y / 4;
            for (int i = 4 * a; i < 4 * a + 4; i++)
                for (int j = 4 * b; j < 4 * b + 4; j++)
                    if (arr[i, j] == arr[x, y] && i != x && j != y)
                        return 0;
            return 1;
        }
        #endregion
        //Lắng nghe và chấp nhận kết nối
        private void listen()
        {
            while (stop == 0)
            {
                try
                {
                    //Chấp nhận kết nối và bật thread kiểm tra login(Nhanuser)
                    socket[count] = server.AcceptSocket();
                    count++;
                    CheckForIllegalCrossThreadCalls = false;
                    Thread t = new Thread(Nhanuser);
                    t.IsBackground = true;
                    t.Start(count - 1);
                }
                catch (SocketException)
                {
                }
            }
        }
        //Kiểm tra thông tin login
        private void Nhanuser(object x)
        {
            if (stop == 0)
            {
                try
                {
                    //Nhận thông tin username
                    int y = (Int32)x;
                    byte[] bytereceive = new byte[100];
                    int k = socket[y].Receive(bytereceive);
                    string checkuser = "";
                    for (int i = 0; i < k; i++)
                    {
                        checkuser += Convert.ToChar(bytereceive[i]);
                    }
                    //Nhận thông tin password
                    bytereceive = new byte[100];
                    k = socket[y].Receive(bytereceive);
                    string checkpass = "";
                    for (int i = 0; i < k; i++)
                    {
                        checkpass += Convert.ToChar(bytereceive[i]);
                    }
                    //Kiểm tra khớp username, password
                    int resultcheck = -1;
                    for (int i = 0; i < 2; i++)
                    {
                        if (user[i] == checkuser && password[i] == checkpass)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                if (checkuser == userlogin[j])
                                {
                                    resultcheck = 0;
                                    break;
                                }
                                resultcheck = 1;
                            }
                            break;
                        }
                        else
                        {
                            resultcheck = -1;
                        }
                    }
                    //Thông báo cho Client kết quả kiểm tra
                    ASCIIEncoding encode = new ASCIIEncoding();
                    socket[y].Send(encode.GetBytes(Convert.ToString(resultcheck)));
                    //Nếu username, password khớp thì bật thread Communicate, ngược lại ngắt kết nối
                    if (resultcheck == 1)
                    {
                        DateTime now = DateTime.Now;
                        lsvLog.Items.Add(now + " User " + checkuser + " has logon" + "\n");
                        userlogin[y] = checkuser;
                        Thread t2 = new Thread(communicate);
                        t2.IsBackground = true;
                        t2.Start(y);
                    }
                    else
                    {
                        socket[y].Close();
                    }
                }
                catch
                {
                }
            }
        }
        //Server giao tiếp với Client
        private void communicate(object x)
        {
            int[,] arr = new int[16, 16];
            int y = (Int32)x;
            while (stop == 0)
            {
                try
                {
                    //Nhận thông tin từ Client                    
                    byte[] bytereceive = new byte[100];
                    int k = socket[y].Receive(bytereceive);
                    string checkrequest = "";
                    for (int i = 0; i < k; i++)
                    {
                        checkrequest += Convert.ToChar(bytereceive[i]);
                    }
                    #region Kiểm tra yêu cầu của Client (!: khởi tạo mảng ?:Check kiểm tra x:Ngắt kết nối)
                    if (checkrequest[0] == '!')
                    {
                        #region Truòng hợp yêu cầu là !(phát sinh mảng)
                        //Check size của mảng
                        if (checkrequest[1] == '4')
                        {
                            arr = new int[16, 16];
                            //Phat sinh mảng
                            phatsinh16 phatsinh = new phatsinh16();
                            arr = phatsinh.phatsinh();
                            //Gửi mảng về cho Client
                            ASCIIEncoding encode = new ASCIIEncoding();
                            for (int i = 0; i < 16; i++)
                                for (int j = 0; j < 16; j++)
                                {
                                    if (arr[i, j] < 10)
                                        socket[y].Send(encode.GetBytes("o" + Convert.ToString(arr[i, j])));
                                    else
                                        socket[y].Send(encode.GetBytes("t" + Convert.ToString(arr[i, j])));
                                }
                        }
                        else
                        {
                            if (checkrequest[1] == '3')
                            {
                                arr = new int[9, 9];
                                //Phat sinh mảng
                                Phatsinh9 phatsinh = new Phatsinh9();
                                arr = phatsinh.phatsinh();
                                //Gửi mảng về cho Client
                                ASCIIEncoding encode = new ASCIIEncoding();
                                for (int i = 0; i < 9; i++)
                                    for (int j = 0; j < 9; j++)
                                        socket[y].Send(encode.GetBytes(Convert.ToString(arr[i, j])));
                            }
                        }
                    }

                    #endregion
                    else
                    {
                        if (checkrequest[0] == '?')
                        {
                            #region Trường hợp yêu cầu là ?(Kiểm tra)
                            if (checkrequest[1] == '3')
                            {
                                #region Xử lý yêu cầu kiểm tra của Game3x3
                                if (Convert.ToChar(bytereceive[4]) != '0')
                                {
                                    //Xac định vị trí phần tử kiểm tra
                                    int toadox, toadoy;
                                    string temp = "";
                                    temp += Convert.ToChar(bytereceive[2]);
                                    toadox = Convert.ToInt32(temp);
                                    temp = "";
                                    temp += Convert.ToChar(bytereceive[3]);
                                    toadoy = Convert.ToInt32(temp);
                                    temp = "";
                                    temp += Convert.ToChar(bytereceive[4]);
                                    arr[toadox, toadoy] = Convert.ToInt32(temp);
                                    //Check
                                    ASCIIEncoding encode = new ASCIIEncoding();
                                    socket[y].Send(encode.GetBytes(Convert.ToString(check3(arr, toadox, toadoy))));
                                }
                                //Trường hợp là nút xóa, cập nhật giá trị 0
                                else
                                {
                                    int toadox, toadoy;
                                    string temp = "";
                                    temp += Convert.ToChar(bytereceive[2]);
                                    toadox = Convert.ToInt32(temp);
                                    temp = "";
                                    temp += Convert.ToChar(bytereceive[3]);
                                    toadoy = Convert.ToInt32(temp);
                                    arr[toadox, toadoy] = 0;
                                }
                                #endregion
                            }

                            else
                            {
                                #region Xử lý yêu cầu kiểm tra của Game4x4
                                //Xac định vị trí phần tử kiểm tra, cấu trúc request ?4<Toadohang>|<toadocot>|<giatri>x
                                int toadox, toadoy;
                                string temp = "";
                                int i = 2;
                                //Xác định tọa độ x
                                for (; ; )
                                {
                                    if (bytereceive[i] != '|')
                                    {
                                        temp += Convert.ToChar(bytereceive[i]);
                                        i++;
                                    }
                                    else
                                    {
                                        toadox = Convert.ToInt32(temp);
                                        i++;
                                        break;
                                    }
                                }
                                //Xác định tọa độ y
                                temp = "";
                                for (; ; )
                                {
                                    if (bytereceive[i] != '|')
                                    {
                                        temp += Convert.ToChar(bytereceive[i]);
                                        i++;
                                    }
                                    else
                                    {
                                        toadoy = Convert.ToInt32(temp);
                                        i++;
                                        break;
                                    }
                                }
                                //Xác định giá trị
                                temp = "";
                                for (; ; )
                                {
                                    if (bytereceive[i] != 'x')
                                    {
                                        temp += Convert.ToChar(bytereceive[i]);
                                        i++;
                                    }
                                    else
                                    {
                                        arr[toadox, toadoy] = Convert.ToInt32(temp);
                                        break;
                                    }
                                }
                                //Check và gửi kết quả cho Client
                                if (arr[toadox, toadoy] != 0)
                                {
                                    ASCIIEncoding encode = new ASCIIEncoding();
                                    socket[y].Send(encode.GetBytes(Convert.ToString(check4(arr, toadox, toadoy))));
                                }
                                #endregion
                            }
                        }
                        #endregion
                        else
                        {
                            if (checkrequest[0] == 'x')
                            {
                                #region Th yêu cầu là x(ngắt kết nối)

                                DateTime now = DateTime.Now;
                                socket[y].Close();
                                tbInfor.Text += now + "User " + userlogin[y] + " has logout" + Environment.NewLine;
                                userlogin[y] = "";
                                break;
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                catch
                {
                }
            }
        }
        //Khởi tạo server, và chạy listen
        private void btStart_Click(object sender, EventArgs e)
        {
            if (btListen.Text == "Start Server")
            {
                #region Nút listen
                //Các khởi tạo cơ bản
                stop = 0;
                socket = new Socket[10];
                userlogin = new string[10];
                ipAd = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(ipAd, 13000);
                server.Start();
                //Chạy thread listen và nhảy thông báo textbox
                Thread t = new Thread(listen);
                t.Start();
                btListen.Text = "Shutdown Server";
                btListen.BackColor = Color.Red;
                DateTime now = DateTime.Now;
                tbInfor.Text += now + " Server has Started!" + Environment.NewLine;
                #endregion
            }
            else
            {
                #region Nút Stop
                stop = 1;
                for (int i = 0; i < count; i++)
                {
                    if (socket[i].Connected)
                        socket[i].Close();
                }
                server.Stop();
                count = 0;
                btStart.Text = "Start Server";
                btStart.BackColor = Color.Green;
                DateTime now = DateTime.Now;
                lsvLog.Items.Add(now + "Server has Stoped!\n");
                #endregion
            }
        }
        //Tắt Server
        private void ServerGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (socket[i].Connected)
                        socket[i].Close();
                }
                server.Stop();
                stop = 1;
            }
            catch (Exception)
            {

            }
        }

        private void fServerGame_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < room; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    lsroom.Add(new List<int>());
                    lsroom[i].Add(0);
                }
            }
        }

        private string createPacketRoom()
        {
            string packet = null;
            packet += "r";
            for (int i = 0; i < room; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    packet += lsroom[i][j].ToString();
                }
            }
            return packet;
        }
    }
}
