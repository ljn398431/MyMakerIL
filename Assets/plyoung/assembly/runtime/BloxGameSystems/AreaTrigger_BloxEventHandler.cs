using BloxEngine;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[AddComponentMenu("")]
	public class AreaTrigger_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events0 = new List<BloxEvent>(1);

		private List<BloxEvent> events1 = new List<BloxEvent>(1);

		private List<BloxEvent> events2 = new List<BloxEvent>(1);

		[BloxEvent("Game Systems/AreaTrigger/OnAreaTriggerEnter", IconName = "bgs")]
		public void OnAreaTriggerEnter(Collider otherCollider, string triggerIdent)
		{
			base.RunEvents(this.events0, new BloxEventArg("otherCollider", otherCollider), new BloxEventArg("triggerIdent", triggerIdent));
		}

		[BloxEvent("Game Systems/AreaTrigger/OnAreaTriggerExit", IconName = "bgs")]
		public void OnAreaTriggerExit(Collider otherCollider, string triggerIdent)
		{
			base.RunEvents(this.events1, new BloxEventArg("otherCollider", otherCollider), new BloxEventArg("triggerIdent", triggerIdent));
		}

		[BloxEvent("Game Systems/AreaTrigger/OnAreaTriggerStay", IconName = "bgs")]
		public void OnAreaTriggerStay(Collider otherCollider, string triggerIdent)
		{
			base.RunEvents(this.events2, new BloxEventArg("otherCollider", otherCollider), new BloxEventArg("triggerIdent", triggerIdent));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Game Systems/AreaTrigger/OnAreaTriggerEnter".Equals(ev.ident))
			{
				this.events0.Add(ev);
			}
			else if ("Game Systems/AreaTrigger/OnAreaTriggerExit".Equals(ev.ident))
			{
				this.events1.Add(ev);
			}
			else if ("Game Systems/AreaTrigger/OnAreaTriggerStay".Equals(ev.ident))
			{
				this.events2.Add(ev);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
