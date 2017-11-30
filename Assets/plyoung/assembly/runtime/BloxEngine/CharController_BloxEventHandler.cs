using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class CharController_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Collision/OnControllerColliderHit", Order = 100)]
		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			base.RunEvents(this.events, new BloxEventArg("hit", hit));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Collision/OnControllerColliderHit".Equals(ev.ident))
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
