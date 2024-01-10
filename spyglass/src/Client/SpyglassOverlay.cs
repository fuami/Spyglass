using Vintagestory.API.Client;
using Cairo;
using System;

namespace spyglass.src.Client
{
    internal class SpyglassOverlay : GuiElement // basically GuiElementInset, with a customizeable background color.
    {
        private readonly int edgeSize;
        private readonly float glassBrightness;
        private readonly float glassColor;
        private readonly VignetteStyle style;

        public SpyglassOverlay(ICoreClientAPI capi, ElementBounds bounds, int edgeSize, float glassColor, float glassBrightness, VignetteStyle style) : base(capi, bounds)
        {
            this.edgeSize = edgeSize;
            this.glassColor = glassColor;
            this.glassBrightness = glassBrightness;
            this.style = style;
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

            switch (style)
            {
                case VignetteStyle.circle:
                    {
                        ctx.SetSourceRGBA(0.0, 0.0, 0.0, 1.0);
                        ctx.NewPath();
                        double rad = Math.Min(Bounds.OuterHeight, Bounds.OuterWidth) * 0.5 - edgeSize;
                        ctx.ArcNegative(Bounds.drawX + Bounds.OuterWidth / 2.0, Bounds.drawY + Bounds.OuterHeight / 2.0, rad, 2.0 * Math.PI, 0);
                        ctx.Rectangle(Bounds.drawX, Bounds.drawY, Bounds.OuterWidth, Bounds.OuterHeight);
                        ctx.Fill();
                        break;
                    }

                case VignetteStyle.square:
                    {
                        ctx.SetSourceRGBA(0.0, 0.0, 0.0, 1.0);
                        ctx.NewPath();
                        double size = Math.Min(Bounds.OuterHeight, Bounds.OuterWidth) - edgeSize * 2;
                        double hsize = size * 0.5;
                        ctx.MoveTo(Bounds.drawX + Bounds.OuterWidth / 2.0 + hsize, Bounds.drawY + Bounds.OuterHeight / 2.0 + hsize);
                        ctx.RelLineTo(0, -size);
                        ctx.RelLineTo(-size, 0);
                        ctx.RelLineTo(0, size);
                        ctx.RelLineTo(size, 0);
                        ctx.Rectangle(Bounds.drawX, Bounds.drawY, Bounds.OuterWidth, Bounds.OuterHeight);
                        ctx.Fill();
                        break;
                    }

                case VignetteStyle.box:
                    {
                        ctx.SetSourceRGBA(0.0, 0.0, 0.0, 1.0);
                        ctx.NewPath();
                        double sizex = Bounds.OuterWidth - edgeSize * 2;
                        double sizey = Bounds.OuterHeight - edgeSize * 2;
                        ctx.MoveTo(Bounds.drawX + Bounds.OuterWidth / 2.0 + sizex / 2.0, Bounds.drawY + Bounds.OuterHeight / 2.0 + sizey / 2.0);
                        ctx.RelLineTo(0, -sizey);
                        ctx.RelLineTo(-sizex, 0);
                        ctx.RelLineTo(0, sizey);
                        ctx.RelLineTo(sizex, 0);
                        ctx.Rectangle(Bounds.drawX, Bounds.drawY, Bounds.OuterWidth, Bounds.OuterHeight);
                        ctx.Fill();
                        break;
                    }

                default:
                    {
                        EmbossRoundRectangleElement(ctx, Bounds, true, edgeSize);
                        break;
                    }
            }
        }
    }
}