using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spyglass.src.Client
{
    class ClientTime
    {
        double startTime = 0;

        public float ElapsedMilliseconds { get => (float)(SpyglassMod.gameRuntime - startTime) * 1000f; }

        internal static ClientTime StartNew()
        {
            ClientTime v = new ClientTime();
            v.startTime = SpyglassMod.gameRuntime;
            return v;
        }
    }
}
