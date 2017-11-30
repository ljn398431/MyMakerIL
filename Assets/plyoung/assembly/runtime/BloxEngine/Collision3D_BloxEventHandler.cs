using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Collision3D_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onEnterEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onExitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onStayEvents = new List<BloxEvent>(1);

		[BloxEvent("Collision/OnCollisionEnter", Order = 100)]
		public void OnCollisionEnter()
		{
			base.RunEvents(this.onEnterEvents);
		}

		[BloxEvent("Collision/OnCollisionExit", Order = 100)]
		public void OnCollisionExit()
		{
			base.RunEvents(this.onExitEvents);
		}

		[BloxEvent("Collision/OnCollisionStay", Order = 100)]
		public void OnCollisionStay()
		{
			base.RunEvents(this.onStayEvents);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Collision/OnCollisionEnter".Equals(ev.ident))
			{
				this.onEnterEvents.Add(ev);
			}
			else if ("Collision/OnCollisionExit".Equals(ev.ident))
			{
				this.onExitEvents.Add(ev);
			}
			else if ("Collision/OnCollisionStay".Equals(ev.ident))
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
