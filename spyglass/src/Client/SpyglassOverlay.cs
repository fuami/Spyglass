using Vintagestory.API.Client;
using Cairo;

namespace spyglass.src.Client
{
    internal class SpyglassOverlay : GuiElement // basically GuiElementInset, with a customizeable background color.
    {
        private readonly int edgeSize;
        private readonly float glassBrightness;
        private readonly float glassColor;

        public SpyglassOverlay(ICoreClientAPI capi, ElementBounds bounds, int edgeSize, float glassColor, float glassBrightness) : base(capi, bounds)
        {
            this.edgeSize = edgeSize;
            this.glassColor = glassColor;
            this.glassBrightness = glassBrightness;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            if (glassBrightness < 1)
            {
                ctx.SetSourceRGBA(glassColor, glassColor, glassColor, 1 - glassBrightness);
                Rectangle(ctx, Bounds);
                ctx.Fill();
            }

            EmbossRoundRectangleElement(ctx, Bounds, true, edgeSize);
        }
    }
}