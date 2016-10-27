using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netshout;
using Netshout.Communication;

namespace NetshoutTestbench
{
    class Program
    {
        static void Main(string[] args)
        {
            Broadcast Bc = new Broadcast("Hunter", "x", new System.Net.IPEndPoint(System.Net.IPAddress.Parse("23.112.36.245"), 8000), 1);
            Console.WriteLine("Authenticating... ");

            try
            {
                Bc.AuthenticateStream();
            }

            catch (AuthDenyException E)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(E.Message);
                Console.ForegroundColor = ConsoleColor.White;

                while (true)
                {
                    Console.ReadLine();
                }
            }

            if (!Bc.Authenticated)
            {
                return;
            }

            Console.WriteLine("Setting MIME type..");
            Bc.SetMimeType(MimeType.Mp3);
            Console.WriteLine("Mime type: " + MimeType.Mp3);

            Console.Write("Setting Bitrate..");
            Bc.SetBitrate(320, 320);
            Console.WriteLine("Bitrate: 320kbps");

            Console.Write("Negotiating buffer size..");
            Bc.NegotiatedBufferSize = Bc.NegotiateBufferSize(1, 2);
            Console.WriteLine();

            Bc.IcyName = "Netshout Test Broadcast";
            Bc.IcyGenre = "Various";
            Bc.IcyPub = false;
            Bc.IcyUrl = "http://dev.nodebay.com";
            
            ShoutcastMetadata Scmd = new ShoutcastMetadata();
            Scmd.TrackTitle = "TestTrack";
            Scmd.Artist = "Artist";
            Scmd.Album = "Test Album";
            Scmd.Genre = "Various";
            Scmd.DjName = "DJ-Monk";
            Scmd.StreamTitle = "Netshout Library Test Session";
            Scmd.SourceIdentifier = "Netshout Source Library";
            
            Bc.Standby();
            Bc.PerformMetadataUpdate(Scmd);

            if (!Bc.StreamReady)
            {
                return;
            }

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
