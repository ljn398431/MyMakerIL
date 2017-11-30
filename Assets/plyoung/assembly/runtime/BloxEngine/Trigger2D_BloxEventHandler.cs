using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Trigger2D_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onStayEvents = new List<BloxEvent>(1);

		[BloxEvent("Trigger/OnTriggerEnter2D", Order = 100)]
		public void OnTriggerEnter2D()
		{
			base.RunEvents(this.onEnterEvents);
		}

		[BloxEvent("Trigger/OnTriggerExit2D", Order = 100)]
		public void OnTriggerExit2D()
		{
			base.RunEvents(this.onExitEvents);
		}

		[BloxEvent("Trigger/OnTriggerStay2D", Order = 100)]
		public void OnTriggerStay2D()
		{
			base.RunEvents(this.onStayEvents);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Trigger/OnTriggerEnter2D".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Trigger/OnTriggerExit2D".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Trigger/OnTriggerStay2D".Equals(ev.ident))
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
