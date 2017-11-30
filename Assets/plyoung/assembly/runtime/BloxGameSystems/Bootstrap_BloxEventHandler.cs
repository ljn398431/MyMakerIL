using BloxEngine;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[AddComponentMenu("")]
	public class Bootstrap_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Game Systems/Startup/OnBootstrapDone", IconName = "bgs", Order = 1001)]
		public void OnBootstrapDone()
		{
			base.RunEvents(this.events);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Game Systems/Startup/OnBootstrapDone".Equals(ev.ident))
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
