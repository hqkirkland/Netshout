using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netshout
{
    /*
     * Consider making this an object interface.
     * That way, instantiating it will limit MP3
     * users to MP3 message classes/types.
     */
    
    public enum MessageFlag
    {
        Operations = 0x0000,

        Authenticate = 0x1001,
        Setup = 0x1002,
        NegotiateBufferSize = 0x1003,
        Standby = 0x1004,
        Terminate = 0x1005,
        FlushMetadata = 0x1006,
        RequireListenerAuth = 0x1007,
        NegotiateMaxPayloadSize = 0x1008,
        RequestCipher = 0x1009,
        SetMimeType = 0x1040,
        TransferBegin = 0x1050,
        TransferData = 0x1051,
        IcyName = 0x1100,
        IcyGenre = 0x1101,
        IcyUrl = 0x1102,
        IcyPub = 0x1103,

        /*
        Interruption = 0x2001,
        Termination = 0x2002,
        */

        // InfoMetadata = 0x3000,
        UrlMetadata = 0x3001,
        AolXmlMetadata = 0x3901,
        ShoutcastXmlMetadata = 0x3902,

        JpgLogo = 0x4000,
        PngLogo = 0x4001,
        BmpLogo = 0x4002,
        GifLogo = 0x4003,
        JpgAlbum = 0x4100,
        PngAlbum = 0x4101,
        BmpAlbum = 0x4102,
        GifAlbum = 0x4103,
        
        TimeRemaining = 0x5001,

        Mp3Data = 0x7000,

        VlbData = 0x8000,
        AacLcData = 0x8001,
        AacpData = 0x8003,
        VorbisData = 0x8004,

        HeaderlessAacp = 0x9000,
        HeaderlessVorbis = 0x9001,

        HeaderlessBinaryMetadata = 0xA
    }
}
