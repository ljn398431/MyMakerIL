using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Collision2D_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onStayEvents = new List<BloxEvent>(1);

		[BloxEvent("Collision/OnCollisionEnter2D", Order = 100)]
		public void OnCollisionEnter2D()
		{
			base.RunEvents(this.onEnterEvents);
		}

		[BloxEvent("Collision/OnCollisionExit2D", Order = 100)]
		public void OnCollisionExit2D()
		{
			base.RunEvents(this.onExitEvents);
		}

		[BloxEvent("Collision/OnCollisionStay2D", Order = 100)]
		public void OnCollisionStay2D()
		{
			base.RunEvents(this.onStayEvents);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Collision/OnCollisionEnter2D".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Collision/OnCollisionExit2D".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Collision/OnCollisionStay2D".Equals(ev.ident))
			{
				this.onStayEvents.Add(ev);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
