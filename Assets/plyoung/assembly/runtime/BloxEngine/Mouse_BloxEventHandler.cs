using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Mouse_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onDragEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onOverEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onUpEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onDownEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onButtonEvents = new List<BloxEvent>(1);

		[BloxEvent("Input/OnMouseDrag", Order = 100)]
		public void OnMouseDrag()
		{
			base.RunEvents(this.onDragEvents);
		}

		[BloxEvent("Input/OnMouseEnter", Order = 100)]
		public void OnMouseEnter()
		{
			base.RunEvents(this.onEnterEvents);
		}

		[BloxEvent("Input/OnMouseExit", Order = 100, YieldAllowed = true)]
		public void OnMouseExit()
		{
			base.RunEvents(this.onExitEvents);
		}

		[BloxEvent("Input/OnMouseOver", Order = 100, YieldAllowed = true)]
		public void OnMouseOver()
		{
			base.RunEvents(this.onOverEvents);
		}

		[BloxEvent("Input/OnMouseUp", Order = 100)]
		public void OnMouseUp()
		{
			base.RunEvents(this.onUpEvents);
		}

		[BloxEvent("Input/OnMouseDown", Order = 100)]
		public void OnMouseDown()
		{
			base.RunEvents(this.onDownEvents);
		}

		[BloxEvent("Input/OnMouseUpAsButton", Order = 100)]
		public void OnMouseUpAsButton()
		{
			base.RunEvents(this.onButtonEvents);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Input/OnMouseDrag".Equals(ev.ident))
			{
				this.onDragEvents.Add(ev);
			}
			else if ("Input/OnMouseEnter".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Input/OnMouseExit".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Input/OnMouseOver".Equals(ev.ident))
			{
				this.onOverEvents.Add(ev);
			}
			else if ("Input/OnMouseUp".Equals(ev.ident))
			{
				this.onUpEvents.Add(ev);
			}
			else if ("Input/OnMouseDown".Equals(ev.ident))
			{
				this.onDownEvents.Add(ev);
			}
			else if ("Input/OnMouseUpAsButton".Equals(ev.ident))
			{
				this.onButtonEvents.Add(ev);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
