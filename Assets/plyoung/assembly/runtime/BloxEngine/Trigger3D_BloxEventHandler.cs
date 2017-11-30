using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Trigger3D_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onStayEvents = new List<BloxEvent>(1);

		[BloxEvent("Trigger/OnTriggerEnter", Order = 100)]
		public void OnTriggerEnter()
		{
			base.RunEvents(this.onEnterEvents);
		}

		[BloxEvent("Trigger/OnTriggerExit", Order = 100)]
		public void OnTriggerExit()
		{
			base.RunEvents(this.onExitEvents);
		}

		[BloxEvent("Trigger/OnTriggerStay", Order = 100)]
		public void OnTriggerStay()
		{
			base.RunEvents(this.onStayEvents);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Trigger/OnTriggerEnter".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Trigger/OnTriggerExit".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Trigger/OnTriggerStay".Equals(ev.ident))
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
