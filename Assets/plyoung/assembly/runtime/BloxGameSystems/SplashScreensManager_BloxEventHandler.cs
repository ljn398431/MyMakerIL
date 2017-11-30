using BloxEngine;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[AddComponentMenu("")]
	public class SplashScreensManager_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events0 = new List<BloxEvent>(1);

		private List<BloxEvent> events1 = new List<BloxEvent>(1);

		private List<BloxEvent> events2 = new List<BloxEvent>(1);

		[BloxEvent("Game Systems/Startup/OnSpashScreenShown", IconName = "bgs", Order = 1002)]
		public void bgsOnSpashScreenShown(int screenIndex)
		{
			base.RunEvents(this.events0, new BloxEventArg("screenIndex", screenIndex));
		}

		[BloxEvent("Game Systems/Startup/OnSpashScreenHidden", IconName = "bgs", Order = 1003)]
		public void bgsOnSpashScreenHidden(int screenIndex)
		{
			base.RunEvents(this.events1, new BloxEventArg("screenIndex", screenIndex));
		}

		[BloxEvent("Game Systems/Startup/OnSpashScreensDone", IconName = "bgs", Order = 1005)]
		public void bgsOnSpashScreensDone()
		{
			base.RunEvents(this.events2);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Game Systems/Startup/OnSpashScreenShown".Equals(ev.ident))
			{
				this.events0.Add(ev);
			}
			else if ("Game Systems/Startup/OnSpashScreenHidden".Equals(ev.ident))
			{
				this.events1.Add(ev);
			}
			else if ("Game Systems/Startup/OnSpashScreensDone".Equals(ev.ident))
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
