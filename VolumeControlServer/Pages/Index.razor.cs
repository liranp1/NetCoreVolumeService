using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace VolumeControlServer.Pages
{
    public partial class Index
    {
        string messageOnWeb = "";
        private void IncreaseVolume()
        {
            Connect("127.0.0.1", "2");
        }
        private void DecreaseVolume()
        {
            Connect("127.0.0.1", "1");
        }
        private void MuteVolume()
        {
            Connect("127.0.0.1", "3");
        }

        private void Connect(String server, String message)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            try
            {
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                messageOnWeb = "Sent: " + message;

                data = new Byte[256];
                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                messageOnWeb = "ArgumentNullException: " + e;
            }
            catch (SocketException e)
            {
                messageOnWeb = "SocketException: " + e;
            }
#pragma warning restore CA1416 // Validate platform compatibility

        }
    }
}
