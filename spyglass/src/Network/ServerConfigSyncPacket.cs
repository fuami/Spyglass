using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spyglass.src.Network
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ServerConfigSyncPacket
    {
        public VignetteStyle vignetteStyle;
        public int edgeSize;
        public float edgeOpacity;
    }
}
