using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NetCoreVolumeService
{
    class Program
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32")]
        static extern IntPtr GetConsoleWindow();

        const UInt32 WM_APPCOMMAND = 0x0319;
        const UInt32 APPCOMMAND_VOLUME_DOWN = 9;
        const UInt32 APPCOMMAND_VOLUME_UP = 10;
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;

        static void IncreaseVolume()
        {
            var cw = GetConsoleWindow();
            SendMessage(cw, WM_APPCOMMAND, cw, new IntPtr(APPCOMMAND_VOLUME_UP << 16));
            Console.WriteLine("Increase Volume");
            //ShowBalloon("Remote Control", "Increase Volume");
        }

        static void DecreaseVolume()
        {
            var cw = GetConsoleWindow();
            SendMessage(cw, WM_APPCOMMAND, cw, new IntPtr(APPCOMMAND_VOLUME_DOWN << 16));
            Console.WriteLine("Decrease Volume");
            //ShowBalloon("Remote Control", "Decrease Volume");
        }
        static void MuteVolume()
        {
            var cw = GetConsoleWindow();
            SendMessage(cw, WM_APPCOMMAND, cw, (IntPtr)APPCOMMAND_VOLUME_MUTE);
            Console.WriteLine("Mute");
            //ShowBalloon("Remote Control", "Mute");
        }
        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    // ShowBalloon("Remote Control", "Waiting for a connection...");
                    //NotifyIcon ballon = new NotifyIcon();
                    //ballon.Icon = SystemIcons.Application;//or any icon you like
                    //ballon.ShowBalloonTip(1000, "Balloon title", "Balloon text", ToolTipIcon.None);


                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    //Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);

                        switch (data)
                        {
                            case "1":
                                DecreaseVolume();
                                break;
                            case "2":
                                IncreaseVolume();
                                break;
                            case "3":
                                MuteVolume();
                                break;
                                //Add computer shutdown
                        }
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
