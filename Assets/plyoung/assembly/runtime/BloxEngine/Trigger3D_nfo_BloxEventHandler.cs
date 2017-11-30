using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Trigger3D_nfo_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onStayEvents = new List<BloxEvent>(1);

		[BloxEvent("Trigger/with Info/OnTriggerEnter(...)", Order = 100)]
		public void OnTriggerEnter(Collider otherCollider)
		{
			base.RunEvents(this.onEnterEvents, new BloxEventArg("otherCollider", otherCollider));
		}

		[BloxEvent("Trigger/with Info/OnTriggerExit(...)", Order = 100)]
		public void OnTriggerExit(Collider otherCollider)
		{
			base.RunEvents(this.onExitEvents, new BloxEventArg("otherCollider", otherCollider));
		}

		[BloxEvent("Trigger/with Info/OnTriggerStay(...)", Order = 100)]
		public void OnTriggerStay(Collider otherCollider)
		{
			base.RunEvents(this.onStayEvents, new BloxEventArg("otherCollider", otherCollider));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Trigger/with Info/OnTriggerEnter(...)".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Trigger/with Info/OnTriggerExit(...)".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Trigger/with Info/OnTriggerStay(...)".Equals(ev.ident))
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
