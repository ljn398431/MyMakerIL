using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Joint_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Misc/OnJointBreak")]
		public void OnJointBreak(float breakForce)
		{
			base.RunEvents(this.events, new BloxEventArg("breakForce", breakForce));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Misc/OnJointBreak".Equals(ev.ident))
			{
				this.events.Add(ev);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
