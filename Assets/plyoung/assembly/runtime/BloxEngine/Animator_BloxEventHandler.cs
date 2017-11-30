using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Animator_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> moveEvents = new List<BloxEvent>(1);

		private List<BloxEvent> ikEvents = new List<BloxEvent>(1);

		[BloxEvent("Misc/OnAnimatorMove")]
		public void OnAnimatorMove()
		{
			base.RunEvents(this.moveEvents);
		}

		[BloxEvent("Misc/OnAnimatorIK")]
		public void OnAnimatorIK(int layerIndex)
		{
			base.RunEvents(this.ikEvents, new BloxEventArg("layerIndex", layerIndex));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Misc/OnAnimatorMove".Equals(ev.ident))
			{
				this.moveEvents.Add(ev);
			}
			else if ("Misc/OnAnimatorIK".Equals(ev.ident))
			{
				this.ikEvents.Add(ev);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
