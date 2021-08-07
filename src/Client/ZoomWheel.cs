using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace spyglass.src.Client
{
    class ZoomWheel : GameWheel
	{
		public ZoomWheel(ICoreClientAPI capi) : base(capi)
		{
		}

		public override void OnMouseWheel(MouseWheelEventArgs args)
		{
			if (SpyglassMod.zoomed)
			{
				args.SetHandled(true);
				SpyglassMod.zoomRatio += args.delta / 40.0f;
				if (SpyglassMod.zoomRatio < 0.8f) SpyglassMod.zoomRatio = 0.8f;
				if (SpyglassMod.zoomRatio > 1f) SpyglassMod.zoomRatio = 1f;
			}
		}
	}
}
