using spyglass.src;
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
			VignetteStyle style = SpyglassMod.config.GetVinetteStyle();
            return composer.AddInteractiveElement(new SpyglassOverlay(capi,ElementBounds.Fill, SpyglassMod.config.edgeSize, SpyglassMod.config.glassColor, SpyglassMod.config.glassBrightness, style));
		}

		public override void OnMouseWheel(MouseWheelEventArgs args)
		{
			if (SpyglassMod.zoomed && SpyglassMod.config.enableMouseWheelAdjustment)
			{
				args.SetHandled(true);
				float fDelta = args.delta;
                SpyglassMod.setZoomRatio( SpyglassMod.getZoomRatio() + fDelta / ClientManipulation.GetWheelAdjustment() );
			}
		}
	}
}
