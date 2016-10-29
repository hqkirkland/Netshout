using Netshout.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Netshout
{
    public class Broadcast
    {
        #region Properties

        /// <summary>
        /// Returns the underlying server connection.
        /// </summary>
        public Socket ClientSocket
        {
            get
            {
                return SourceSocket;
            }

            set
            {
                Disconnect();
                SourceSocket = value;
            }
        }

        /// <summary>
        /// Indicates whether the connection is active as of the last Receive() operation. Setting false disconnects the session.
        /// </summary>
        public bool Connected
        {
            get
            {
                return ClientSocket.Connected;
            }

            set
            {
                if(!value)
                {
                    if (SourceSocket.Connected)
                    {
                        ClientSocket.Shutdown(SocketShutdown.Both);
                        ClientSocket.Disconnect(false);
                    }

                    Authenticated = false;
                    ClientSocket.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets or updates the broadcast name.
        /// </summary>
        public String IcyName
        {
            get
            {
                return _IcyName;
            }

            set
            {
                ClientSocket.Send(Packet.Serialize(MessageFlag.IcyName, Encoding.Default.GetBytes(value)));
                ReceiveAck();
                _IcyName = value;
            }
        }

        /// <summary>
        /// Gets or updates the broadcast genre.
        /// </summary>
        public String IcyGenre
        {
            get
            {
                return _IcyGenre;
            }

            set
            {
                ClientSocket.Send(Packet.Serialize(MessageFlag.IcyGenre, Encoding.Default.GetBytes(value)));
                ReceiveAck();
                _IcyGenre = value;
            }
        }

        /// <summary>
        /// Gets or updates the broadcast referral URL.
        /// </summary>
        public String IcyUrl
        {
            get
            {
                return _IcyUrl;
            }

            set
            {
                ClientSocket.Send(Packet.Serialize(MessageFlag.IcyUrl, Encoding.Default.GetBytes(value)));
                ReceiveAck();
                _IcyUrl = value;
            }
        }

        /// <summary>
        /// Gets or updates the broadcast's availability.
        /// </summary>
        public bool IcyPub
        {
            get
            {
                return _IcyPub;
            }

            set
            {
                int Public = (value) ? 1 : 0;
                ClientSocket.Send(Packet.Serialize(MessageFlag.IcyPub, Encoding.Default.GetBytes(Public.ToString())));
                ReceiveAck();
                _IcyPub = value;
            }
        }

        #endregion

        #region Fields

        // Just in case someone needs to change it.
        public String Version = "2.1";
        public String AudioMimeType;
        public bool Authenticated;
        public bool StreamReady;

        public int NegotiatedBufferSize;
        public int NegotiatedMaxPayloadSize;
        public int Bitrate;

        private IPEndPoint ShoutcastEndpoint;
        private Socket SourceSocket;

        private int StreamNum;
        private String XTEAKey;
        private String User;
        private String Pass;

        private String _IcyName;
        private String _IcyGenre;
        private String _IcyUrl;
        private bool _IcyPub;

        private byte[] IncomingBuffer = new byte[128];

        #endregion

        #region Ultravox Methods
        
        /// <summary>
        /// Opens a SHOUTcast connection to the DNAS. 
        /// </summary>
        /// <param name="Username">Username on the DNAS; arbitrary value.</param>
        /// <param name="Password">DNAS broadcast password.</param>
        /// <param name="ServerEndpoint">The IPEndPoint for the destination DNAS.</param>
        /// <param name="StreamId">Stream number on the DNAS.</param>
        public Broadcast(String Username, String Password, IPEndPoint ServerEndpoint, int StreamId)
        {
            if(Username.Length > 8)
            {
                Username.Substring(0, 8);
            }

            User = Username;
            Pass = Password;
            StreamNum = StreamId;

            ShoutcastEndpoint = ServerEndpoint;

            // By setting the Connected property, the server will initiate the connection process.

            SourceSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            SourceSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            SourceSocket.Connect(ShoutcastEndpoint);

            Connected = true;
        }

        /// <summary>
        /// Sends a cipher request to the server, receives the key, and authenticates the user.
        /// </summary>
        public void AuthenticateStream()
        {
            /* Cipher Request */
            byte[] RequestPayload = Encoding.Default.GetBytes("2.1");
            byte[] CipherPacket = Packet.Serialize(MessageFlag.RequestCipher, RequestPayload);

            SourceSocket.Send(CipherPacket);
            String[] Response = ReceiveAck();

            if(Response == new String[1])
            {
                Authenticated = false;
                Connected = false;

                return;
            }
            
            XTEAKey = Response[1].TrimEnd('\0');
            IncomingBuffer = new byte[128];

            /* Broadcast Authentication Request */
            
            /*
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Encrypting password...");
            Console.WriteLine();
            */

            String XteaEncryptedUser = XTEA.Encrypt(User, XTEAKey).ToLower();
            String XteaEncryptedPassword = XTEA.Encrypt(Pass, XTEAKey).ToLower();

            String AuthenticationString = Version + ":" + StreamNum.ToString() + ":" + XteaEncryptedUser + ":" + XteaEncryptedPassword;

            byte[] AuthBytes = Encoding.Default.GetBytes(AuthenticationString);
            SourceSocket.Send(Packet.Serialize(MessageFlag.Authenticate, AuthBytes));

            if (ReceiveAck()[2] == "Allow")
            {
                Authenticated = true;
            }
        }

        /// <summary>
        /// Sends the MIME type for the stream's audio to the server.
        /// </summary>
        /// <param name="MimeType">The MIME type for the audio data that will be sent to the server.</param>
        public void SetMimeType(String MimeType)
        {
            AudioMimeType = MimeType;
            SourceSocket.Send(Packet.Serialize(MessageFlag.SetMimeType, Encoding.Default.GetBytes(MimeType)));
            ReceiveAck();
        }

        /// <summary>
        /// Sends the broadcast's bitrate to the server. Raises exception of the server refuses the indication.
        /// </summary>
        /// <param name="AverageBitrate">Average bitrate of the stream, in kilobytes/s. Value will be equal to MaximumBitrate for fixed-bitrate stream.</param>
        /// <param name="MaximumBitrate">Maximum bitrate of the stream, in kilobytes/s. Value will be equal to Average for fixed-bitrate stream.</param>
        public void SetBitrate(int AverageBitrate, int MaximumBitrate)
        {
            String BitratePayload = (AverageBitrate * 1000).ToString() + ":" + (MaximumBitrate * 1000);
            SourceSocket.Send(Packet.Serialize(MessageFlag.Setup, Encoding.Default.GetBytes(BitratePayload)));

            try
            {
                ReceiveAck();
            }

            catch(BitrateDeniedException)
            {
                throw;
            }
        }

        /// <summary>
        /// Starts a negotiation procedure with the distribution point for its buffer. If the specified minimum size isn't supported, it will throw an exception.
        /// </summary>
        /// <param name="DesiredSize">The desired size for the distribution point's buffer, in kilobytes.</param>
        /// <param name="MinimumSize">The minimum size for the distribution point's buffer, in kilobytes.</param>
        /// <returns>The negotiated size for the buffer.</returns>
        public int NegotiateBufferSize(int DesiredSize, int MinimumSize)
        {
            String NegotiationPayload = DesiredSize.ToString() + ":" + MinimumSize.ToString();
            SourceSocket.Send(Packet.Serialize(MessageFlag.NegotiateBufferSize, Encoding.Default.GetBytes(NegotiationPayload)));

            String NegotiatedSize = ReceiveAck()[1];

            NegotiatedBufferSize = 0;
            int.TryParse(NegotiatedSize, out NegotiatedBufferSize);

            return NegotiatedBufferSize;
        }

        /// <summary>
        /// Switches to Data Transfer Mode; indicates that audio data and/or metadata is incoming.
        /// </summary>
        public bool Standby()
        {
            SourceSocket.Send(Packet.Serialize(MessageFlag.Standby, new byte[0]));

            String[] ResponseArray = ReceiveAck();

            if (ResponseArray[1] == "Data transfer mode")
            {
                StreamReady = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Requests for cached metadata to be flushed on the distribution point.
        /// </summary>
        public void FlushCachedMetadata()
        {
            SourceSocket.Send(Packet.Serialize(MessageFlag.FlushMetadata, new byte[0]));
            ReceiveAck();
        }

        /// <summary>
        /// Concise way of flushing and then updating the distribution point's metadata
        /// </summary>
        /// <param name="MetadataInstance">A ShoutcastMetadata instance with the new data to send to the server, with all desired fields filled out.</param>
        public void PerformMetadataUpdate(ShoutcastMetadata MetadataInstance)
        {
            FlushCachedMetadata();

            var XmlNamespaces = new XmlSerializerNamespaces();
            XmlNamespaces.Add("", "");

            XmlSerializer Serializer = new XmlSerializer(typeof(ShoutcastMetadata));
            StringBuilder Sb = new StringBuilder();

            int MetadataId = 1;
            int MetadataSpan = 1;
            int MetadataIndex = 1;

            byte[] XmlHead = new byte[] { 0x0, Convert.ToByte(MetadataId), 0x0, Convert.ToByte(MetadataSpan), 0x0, Convert.ToByte(MetadataIndex) };
            byte[] XmlBytes;

            using (StringWriter Writer = new StringWriter(Sb))
            {
                Serializer.Serialize(Writer, MetadataInstance, XmlNamespaces);
                XmlBytes = Encoding.UTF8.GetBytes(Writer.ToString().Replace("utf-16", "UTF-8"));
                Array.Resize<byte>(ref XmlBytes, XmlBytes.Length + 6);

                Buffer.BlockCopy(XmlBytes, 0, XmlBytes, 6, XmlBytes.Length - 6);
                Buffer.BlockCopy(XmlHead, 0, XmlBytes, 0, 6);
            }

            Console.WriteLine(Encoding.UTF8.GetString(XmlBytes));

            SourceSocket.Send(Packet.Serialize(MessageFlag.ShoutcastXmlMetadata, XmlBytes));
        }

        /// <summary>
        /// Not implemented yet. It is assumed that this specifies a password that needs to be used to access the stream.
        /// </summary>
        /// <param name="Password"></param>
        public void RequireListenerAuth(String Password)
        {
            throw new NotImplementedException("This feature hasn't been implemented yet.");
        }

        /// <summary>
        /// Negotiates the maximum payload size for data transfer mode.
        /// </summary>
        /// <param name="DesiredSize">Desired maximum payload size.</param>
        /// <param name="MinimumSize">Minimum payload size.</param>
        /// <returns></returns>
        public int NegotiateMaxPayloadSize(int DesiredSize, int MinimumSize)
        {
            String NegotiationPayload = DesiredSize.ToString() + ":" + MinimumSize.ToString();
            SourceSocket.Send(Packet.Serialize(MessageFlag.NegotiateMaxPayloadSize, Encoding.Default.GetBytes(NegotiationPayload)));

            String NegotiatedSize = ReceiveAck()[1];
            NegotiatedMaxPayloadSize = 0;
            int.TryParse(NegotiatedSize, out NegotiatedMaxPayloadSize);

            return NegotiatedMaxPayloadSize;
        }

        /// <summary>
        /// Encapsulates the audio data and sends it to the server using the message type that corresponds to your specified MimeType.
        /// </summary>
        /// <param name="AudioData">A byte array containing the payload for this packet.</param>
        /// <returns></returns>
        public byte[] SerializedAudioData(byte[] AudioData)
        {
            MessageFlag AudioFlag;

            switch (AudioMimeType)
            {
                case MimeType.Mp3:
                    AudioFlag = MessageFlag.Mp3Data;
                    break;

                case MimeType.Aacp:
                    AudioFlag = MessageFlag.AacpData;
                    break;

                case MimeType.Aac:
                    AudioFlag = MessageFlag.AacLcData;
                    break;

                case MimeType.Ogg:
                    AudioFlag = MessageFlag.VorbisData;
                    break;

                default:
                    throw new MimeTypeException();
            }

            return Packet.Serialize(AudioFlag, AudioData);
        }

        #endregion

        #region Communication Methods
        /// <summary>
        /// Receives an acknowledgement string from the distribution point. Raises the appropriate Exception if the response is not an acknowledgement but, rather, an error.
        /// </summary>
        /// <param name="IntendedFlag">The flag that the response will have. Typically, the response uses the same flag as the request (E.g. Responses for a packet with the Authentication Request type will use the Authentication Request type.)</param>
        /// <returns>Server's acknowledgement string response.</returns>
        public String[] ReceiveAck()
        {
            SourceSocket.Receive(IncomingBuffer);

            int Flags = BitConverter.ToInt16(IncomingBuffer, 2);
            int ResponseLength = BitConverter.ToInt16(IncomingBuffer, 4);

            if (BitConverter.IsLittleEndian)
            {
                Flags = BitConverter.ToInt16(BitConverter.GetBytes(BitConverter.ToInt16(IncomingBuffer, 2)).Reverse().ToArray(), 0);
                ResponseLength = BitConverter.ToInt16(BitConverter.GetBytes(BitConverter.ToInt16(IncomingBuffer, 4)).Reverse().ToArray(), 0);
            }

            String[] Response = Encoding.Default.GetString(IncomingBuffer, 6, ResponseLength).TrimEnd('\0').Split(':');

            /* NAK Handling */
            if (Response[0] == "NAK")
            {

                String Reason = (Response[1] == "2.1") ? Response[2] : Response[1];

                switch (Reason)
                {
                    case "Parse Error":
                        throw new ServerParseException();

                    case "Sequence Error":
                        throw new SequenceException(Flags);

                    case "Deny":

                        if (Flags != (int)(MessageFlag.SetMimeType))
                        {
                            Authenticated = false;
                            Disconnect();

                            throw new AuthDenyException(User, Pass);
                        }

                        break;

                    case "Version Error":
                        throw new AuthDenyException(Version);

                    case "Stream Moved":
                        throw new AuthDenyException(StreamNum);

                    case "Buffer Size Error":
                        throw new BufferNegotiationException();

                    case "Bit Rate Error":
                        throw new BitrateDeniedException();

                    case "Compatibility mode not enabled":
                        throw new CompatibilityModeDisabledException();

                    case "Configuration Error":
                        throw new ConfigurationIncompleteException();

                    case "Stream In Use":
                        throw new StreamInUseException(StreamNum);

                    default:
                        throw new Exception(Response[2]);
                }
            }

            // Clear the buffer.
            IncomingBuffer = new byte[128];

            return Response;
        }

        /// <summary>
        /// Performs a clean disconnect, notifying the distribution point of the disconnection before disposing the socket.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                ClientSocket.Send(Packet.Serialize(MessageFlag.Terminate, new byte[0]));
            }

            catch (SocketException) { }
            catch (NullReferenceException) { }

            Connected = false;
        }
        #endregion
    }
}
