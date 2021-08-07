using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace spyglass.src.Client
{
	// base class that allows using OnMouseWheel without breaking things.
	class GameWheel : HudElement
	{
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
			SingleComposer = capi.Gui.CreateCompo("gamewheel", ElementBounds.Fill).Compose(true);
		}

		public override bool TryClose()
		{
			return false;
		}

		public override void OnOwnPlayerDataReceived()
		{
			TryOpen();
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
