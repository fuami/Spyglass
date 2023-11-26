using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;

namespace spyglass.src.Client
{
    class ZoomWheel : GameWheel
	{
		public ZoomWheel(ICoreClientAPI capi) : base(capi)
		{
		}

		public override bool IsEnabled()
		{
			return ClientManipulation.getPercentZoomed() > 0.01;
		}

		public override void BeforeRenderGUI(float deltaTime)
		{
			SingleComposer.Color.W = ClientManipulation.getPercentZoomed() * SpyglassMod.config.edgeOpacity;
		}

		public override GuiComposer adjustCompose(GuiComposer composer)
		{
			composer.Color = new Vec4f(1f, 1f, 1f, 0f);
			return composer.AddInteractiveElement(new SpyglassOverlay(capi,ElementBounds.Fill, SpyglassMod.config.edgeSize, SpyglassMod.config.glassColor, SpyglassMod.config.glassBrightness));
		}

		public override void OnMouseWheel(MouseWheelEventArgs args)
		{
			if (SpyglassMod.zoomed && SpyglassMod.config.enableMouseWheelAdjustment)
			{
				args.SetHandled(true);
				SpyglassMod.zoomRatio += args.delta / 40.0f;
				if (SpyglassMod.zoomRatio < 0.8f) SpyglassMod.zoomRatio = 0.8f;
				if (SpyglassMod.zoomRatio > 1f) SpyglassMod.zoomRatio = 1f;
			}
		}
	}
}
