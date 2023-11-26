using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace spyglass.src.Client
{
	// base class that allows using OnMouseWheel without breaking things.
	abstract class GameWheel : HudElement
	{
		private bool wasEnabled = false;

		public override bool Focusable
		{
			get => false;
		}

		public override double InputOrder
		{
			get => 1.1;
		}

		public GameWheel(ICoreClientAPI capi) : base(capi)
		{
			SingleComposer = capi.Gui.CreateCompo("gamewheel", ElementBounds.Empty).Compose(true);
		}

		public override bool TryClose()
		{
			return false;
		}

		public override void OnOwnPlayerDataReceived()
		{
			TryOpen();
		}
		public override void OnRenderGUI(float deltaTime)
		{
			if ( wasEnabled != IsEnabled() )
			{
				wasEnabled = !wasEnabled;
				SingleComposer = adjustCompose( capi.Gui.CreateCompo("gamewheel", wasEnabled ? ElementBounds.Fill : ElementBounds.Empty) ).Compose(true);
			}

			if (wasEnabled)
			{
				BeforeRenderGUI(deltaTime);
				base.OnRenderGUI(deltaTime);
			}
		}

		// when enabled some mouse actions are blocked, so best only enable when in use.
		public abstract bool IsEnabled();

		public virtual void BeforeRenderGUI(float deltaTime)
		{
		}

		public virtual GuiComposer adjustCompose(GuiComposer composer)
		{
			return composer;
		}

		public override void OnMouseDown(MouseEvent args)
		{
		}

		public override void OnMouseUp(MouseEvent args)
		{
		}

		public override void OnMouseMove(MouseEvent args)
		{
		}

	}
}
