using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spyglass.src.Client
{
    class ClientTime
    {
        private double startTime;

        public float ElapsedMilliseconds { get => (float)(SpyglassMod.gameRuntime - startTime) * 1000f; }

        internal static ClientTime StartNew()
        {
            return new ClientTime
            {
                startTime = SpyglassMod.gameRuntime
            };
        }

        internal static ClientTime Eariler()
        {
            return new ClientTime {
                startTime = SpyglassMod.gameRuntime - 30 // 30 seconds should do.
            };
        }
    }
}
