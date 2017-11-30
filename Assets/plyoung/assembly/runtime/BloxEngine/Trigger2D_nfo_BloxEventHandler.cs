using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Trigger2D_nfo_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onStayEvents = new List<BloxEvent>(1);

		[BloxEvent("Trigger/with Info/OnTriggerEnter2D(...)", Order = 100)]
		public void OnTriggerEnter2D(Collider2D otherCollider)
		{
			base.RunEvents(this.onEnterEvents, new BloxEventArg("otherCollider", otherCollider));
		}

		[BloxEvent("Trigger/with Info/OnTriggerExit2D(...)", Order = 100)]
		public void OnTriggerExit2D(Collider2D otherCollider)
		{
			base.RunEvents(this.onExitEvents, new BloxEventArg("otherCollider", otherCollider));
		}

		[BloxEvent("Trigger/with Info/OnTriggerStay2D(...)", Order = 100)]
		public void OnTriggerStay2D(Collider2D otherCollider)
		{
			base.RunEvents(this.onStayEvents, new BloxEventArg("otherCollider", otherCollider));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Trigger/with Info/OnTriggerEnter2D(...)".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Trigger/with Info/OnTriggerExit2D(...)".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Trigger/with Info/OnTriggerStay2D(...)".Equals(ev.ident))
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
