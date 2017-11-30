using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class UpdateLate_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Common/LateUpdate", Order = 3)]
		public void LateUpdate()
		{
			base.RunEvents(this.events);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Common/LateUpdate".Equals(ev.ident))
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
