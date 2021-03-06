using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Update_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Common/Update", Order = 3)]
		public void Update()
		{
			base.RunEvents(this.events);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Common/Update".Equals(ev.ident))
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
