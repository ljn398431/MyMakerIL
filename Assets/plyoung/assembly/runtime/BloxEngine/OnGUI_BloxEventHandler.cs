using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class OnGUI_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Misc/OnGUI")]
		public void OnGUI()
		{
			base.RunEvents(this.events);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Misc/OnGUI".Equals(ev.ident))
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
