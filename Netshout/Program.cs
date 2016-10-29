using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netshout;
using Netshout.Communication;
using System.IO;

namespace Netshout
{
    class Example
    {
	// Just a quick-and-dirty example. You don't have to wrap this in a class, obviously, the below code can be used anywhere that Netshout is accessible.
	// The intended user of this application should be able to recognize that, though 8)

        public Example()
        {
            Broadcast Bc = new Broadcast("UID", "Password", new System.Net.IPEndPoint(System.Net.IPAddress.Parse("EndpointIp"), 8000), 1);
            Console.WriteLine("Authenticating..");

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

            Console.WriteLine("Setting Bitrate..");
            Bc.SetBitrate(320, 320);
            
            Bc.NegotiateMaxPayloadSize((16 * 1024) - 6 - 1, 0);

            Console.Write("Negotiating buffer size..");
            Bc.NegotiatedBufferSize = Bc.NegotiateBufferSize(1024, 2048);
            Console.WriteLine();

            Bc.IcyName = "Netshout Test Broadcast";
            Bc.IcyGenre = "Various";
            Bc.IcyPub = false;
            Bc.IcyUrl = "http://dev.nodebay.com";
                      
            ShoutcastMetadata Scmd = new ShoutcastMetadata();
            Scmd.Track = "X";
            Scmd.Album = "Test Album";
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
