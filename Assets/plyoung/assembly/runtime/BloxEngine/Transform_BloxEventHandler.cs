using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Transform_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events1 = new List<BloxEvent>(1);

		private List<BloxEvent> events2 = new List<BloxEvent>(1);

		[BloxEvent("Misc/OnTransformParentChanged")]
		public void OnTransformParentChanged()
		{
			base.RunEvents(this.events1);
		}

		[BloxEvent("Misc/OnTransformChildrenChanged")]
		public void OnTransformChildrenChanged()
		{
			base.RunEvents(this.events2);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Misc/OnTransformParentChanged".Equals(ev.ident))
			{
				this.events1.Add(ev);
			}
			else if ("Misc/OnTransformChildrenChanged".Equals(ev.ident))
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
