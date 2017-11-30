using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Collision3D_nfo_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onStayEvents = new List<BloxEvent>(1);

		[BloxEvent("Collision/with Info/OnCollisionEnter(...)", Order = 100)]
		public void OnCollisionEnter(Collision collision)
		{
			base.RunEvents(this.onEnterEvents, new BloxEventArg("collision", collision));
		}

		[BloxEvent("Collision/with Info/OnCollisionExit(...)", Order = 100)]
		public void OnCollisionExit(Collision collision)
		{
			base.RunEvents(this.onExitEvents, new BloxEventArg("collision", collision));
		}

		[BloxEvent("Collision/with Info/OnCollisionStay(...)", Order = 100)]
		public void OnCollisionStay(Collision collision)
		{
			base.RunEvents(this.onStayEvents, new BloxEventArg("collision", collision));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Collision/with Info/OnCollisionEnter(...)".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Collision/with Info/OnCollisionExit(...)".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Collision/with Info/OnCollisionStay(...)".Equals(ev.ident))
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
