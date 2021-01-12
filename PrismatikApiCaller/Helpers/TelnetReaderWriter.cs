using System;
using System.Net.Sockets;
using System.Text;

namespace PrismatikApiCaller
{
    public class TelnetReaderWriter
    {
        public string Read(NetworkStream ns)
        {
            StringBuilder sb = new StringBuilder();
            if (ns.CanRead)
            {
                byte[] readBuffer = new byte[1024];
                do
                {
                    int numBytesRead = ns.Read(readBuffer, 0, readBuffer.Length);
                    sb.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, numBytesRead));
                    sb.Replace(Convert.ToChar(24), ' ');
                    sb.Replace(Convert.ToChar(255), ' ');
                    sb.Replace('?', ' ');
                }

                while (ns.DataAvailable);
            }

            return sb.ToString();
        }

        public void Write(NetworkStream ns, string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message + Environment.NewLine);
            ns.Write(msg, 0, msg.Length);
        }
    }
}
