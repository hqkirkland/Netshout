using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Netshout
{
    public class Comlink
    {
        public Socket ClientSocket;
        public byte[] IncomingBuffer;
        public bool Connecting;
        public bool Connected;
        public bool Ready;

        public String IpAddr;
        public String Username;
        public String Password;
        public String StreamId;
        public String Format;
        public int Bitrate;

        public int BufferSize;

        public Comlink(String ipAddr, int port, String user, String password, String sid, String format, int bitrate)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(ipAddr);

            // Server is listening.
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            this.Username = user;
            this.IpAddr = ipAddr;
            this.Password = password;
            this.StreamId = sid;
            this.Format = format;
            this.Bitrate = bitrate;

            // Connect to the server
            Connected = false;
            Ready = false;
            Connecting = true;

            ClientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            Connecting = false;
            Connected = true;
            IncomingBuffer = new byte[3];
            ClientSocket.EndConnect(ar);
            //Send the version/startup packet to the server
            byte[] VersionBytes = { 0x5A, 0x0, 0x10, 0x009, 0x0, 0x03, 0x32, 0x2e, 0x31, 0x0 };
            Send(VersionBytes);

            ClientSocket.Receive(IncomingBuffer);
            ClientSocket.Receive(IncomingBuffer);

            // Receiving the cipher, sending auth blob.
            IncomingBuffer = new byte[16];
            ClientSocket.Receive(IncomingBuffer);
            String CipherKey = Encoding.Default.GetString(IncomingBuffer).Trim(new char[] { (char)(0) });
            CipherKey = CipherKey.Replace("ACK:", "");
            String AuthBlob = "2.1:" + StreamId + ":" + XTEA.Encrypt(Username, CipherKey).ToLower() + ":" + XTEA.Encrypt(Password, CipherKey).ToLower();
            Packet AuthPacket = new Packet(0x10, 0x001, AuthBlob);
            Send(AuthPacket.Serialize());

            IncomingBuffer = new byte[48];
            ClientSocket.Receive(IncomingBuffer);
            String AuthResponse = Encoding.Default.GetString(IncomingBuffer, 6, 3).Trim();

            if (AuthResponse == "NAK")
            {
                throw new Exception(Encoding.Default.GetString(IncomingBuffer, 6, 24).Trim());
            }

            Ready = true;

            Packet MimePacket = new Packet(0x10, 0x040, Format);
            Send(MimePacket.Serialize());

            IncomingBuffer = new byte[32];
            ClientSocket.Receive(IncomingBuffer);
            String MimeResponse = Encoding.Default.GetString(IncomingBuffer, 6, 3);

            if (MimeResponse == "NAK")
            {
                throw new Exception(Encoding.Default.GetString(IncomingBuffer, 6, 25).Trim());
            }

            Packet BroadcastSetup = new Packet(0x10, 0x002, Bitrate + ":" + Bitrate);
            Send(BroadcastSetup.Serialize());

            IncomingBuffer = new byte[32];
            ClientSocket.Receive(IncomingBuffer);
            String BitrateResponse = Encoding.Default.GetString(IncomingBuffer, 6, 3);

            if (BitrateResponse == "NAK")
            {
                throw new Exception(Encoding.Default.GetString(IncomingBuffer, 6, 25).Trim());
            }

            Packet bufferSetup = new Packet(0x10, 0x003, BufferSize + ":0");
            Send(bufferSetup.Serialize());

            IncomingBuffer = new byte[32];
            ClientSocket.Receive(IncomingBuffer);
            String BufferResponse = Encoding.Default.GetString(IncomingBuffer, 6, 3);

            if (BitrateResponse == "ACK")
            {
                BufferResponse = Encoding.Default.GetString(IncomingBuffer, 10, 3);
                BufferSize = int.Parse(BufferResponse);
            }

            else
            {
                throw new Exception(Encoding.Default.GetString(IncomingBuffer, 6, 25).Trim());
            }

            Packet FrameSize = new Packet(0x10, 0x008, "16377:0");
            Send(FrameSize.Serialize());

            IncomingBuffer = new byte[32];
            ClientSocket.Receive(IncomingBuffer);
            String FrameResponse = Encoding.Default.GetString(IncomingBuffer, 6, 3);

            if (FrameResponse == "NAK")
            {
                throw new Exception(Encoding.Default.GetString(IncomingBuffer, 6, 25));
            }

            ClientSocket.BeginReceive(IncomingBuffer, 0, IncomingBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }

        public void Send(byte[] OutBuffer)
        {
            try
            {
                // Fill the info for the message to be sent.
                ClientSocket.BeginSend(OutBuffer, 0, OutBuffer.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }

            catch (Exception)
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            ClientSocket.EndSend(ar);
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                ClientSocket.EndReceive(ar);

                if (IncomingBuffer != null)
                {

                }

                IncomingBuffer = new byte[64];
                ClientSocket.BeginReceive(IncomingBuffer, 0, IncomingBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }

            catch (Exception)
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();
            }
        }
    }
}
