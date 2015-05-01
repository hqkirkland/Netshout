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
        public Socket _clientSocket;
        public byte[] byteData;
        public bool connecting;
        public bool connected;
        public bool ready;

        public String ipaddr;
        public String user;
        public String pass;
        public String sid;
        public String format;
        public int bitrate;

        public int BufferSize;

        public Comlink(String ipaddr, int port, String user, String password, String sid, String format, int bitrate)
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(ipaddr);

            //Server is listening.
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            this.user = user;
            this.ipaddr = ipaddr;
            this.pass = password;
            this.sid = sid;
            this.format = format;
            this.bitrate = bitrate;

            //Connect to the server
            connected = false;
            ready = false;
            connecting = true;

            _clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            connecting = false;
            connected = true;
            byteData = new byte[3];
            _clientSocket.EndConnect(ar);
            //Send the version/startup packet to the server
            byte[] bytesOut = { 0x5A, 0x0, 0x10, 0x009, 0x0, 0x03, 0x32, 0x2e, 0x31, 0x0 };
            Send(bytesOut);
            _clientSocket.Receive(byteData);
            _clientSocket.Receive(byteData);
            byteData = new byte[16];
            // Receive cipher
            _clientSocket.Receive(byteData);
            String cipherKey = Encoding.ASCII.GetString(byteData).Trim(new char[] { (char)(0) });
            cipherKey = cipherKey.Replace("ACK:", "");

            String authBlob = "2.1:" + sid + ":" + XTEA.Encrypt(user, cipherKey).ToLower() + ":" + XTEA.Encrypt(pass, cipherKey).ToLower();
            Packet authPkt = new Packet(0x10, 0x001, authBlob);

            Send(authPkt.Serialize());

            byteData = new byte[48];
            _clientSocket.Receive(byteData);

            String authResp = Encoding.ASCII.GetString(byteData, 6, 3).Trim();

            if (authResp == "NAK")
            {
                throw new Exception(Encoding.ASCII.GetString(byteData, 6, 24).Trim());
            }

            ready = true;

            Packet mimePacket = new Packet(0x10, 0x040, format);
            Send(mimePacket.Serialize());

            byteData = new byte[32];
            _clientSocket.Receive(byteData);

            String mimeResp = Encoding.ASCII.GetString(byteData, 6, 3);

            if (mimeResp == "NAK")
            {
                throw new Exception(Encoding.ASCII.GetString(byteData, 6, 25).Trim());
            }

            Packet broadcastSetup = new Packet(0x10, 0x002, bitrate + ":" + bitrate);
            Send(broadcastSetup.Serialize());

            byteData = new byte[32];
            _clientSocket.Receive(byteData);

            String bitResp = Encoding.ASCII.GetString(byteData, 6, 3);

            if (bitResp == "NAK")
            {
                throw new Exception(Encoding.ASCII.GetString(byteData, 6, 25).Trim());
            }

            Packet bufferSetup = new Packet(0x10, 0x003, BufferSize + ":0");
            Send(bufferSetup.Serialize());

            byteData = new byte[32];
            _clientSocket.Receive(byteData);

            String bufResp = Encoding.ASCII.GetString(byteData, 6, 3);

            if (bitResp == "ACK")
            {
                bufResp = Encoding.ASCII.GetString(byteData, 10, 3);
                BufferSize = int.Parse(bufResp);
            }

            else
            {
                throw new Exception(Encoding.ASCII.GetString(byteData, 6, 25).Trim());
            }

            Packet frameSize = new Packet(0x10, 0x008, "16377:0");
            Send(frameSize.Serialize());

            byteData = new byte[32];
            _clientSocket.Receive(byteData);

            String frameResp = Encoding.ASCII.GetString(byteData, 6, 3);

            if (frameResp == "NAK")
            {
                throw new Exception(Encoding.ASCII.GetString(byteData, 6, 25));
            }

            _clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }

        public void Send(byte[] dataOut)
        {
            try
            {
                //Fill the info for the message to be send;
                //Send it to the server
                _clientSocket.BeginSend(dataOut, 0, dataOut.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }

            catch (Exception ex)
            {

            }
        }

        private void OnSend(IAsyncResult ar)
        {
            _clientSocket.EndSend(ar);
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                _clientSocket.EndReceive(ar);

                if (byteData != null)
                {

                }

                byteData = new byte[64];
                _clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }

            catch (Exception e)
            {

            }
        }
    }
}
