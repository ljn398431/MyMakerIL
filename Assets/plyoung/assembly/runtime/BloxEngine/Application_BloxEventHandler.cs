using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Application_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> focusEvents = new List<BloxEvent>(1);

		private List<BloxEvent> pauseEvents = new List<BloxEvent>(1);

		private List<BloxEvent> quitEvents = new List<BloxEvent>(1);

		private List<BloxEvent> lvLoadedEvents = new List<BloxEvent>(1);

		[BloxEvent("Misc/OnApplicationFocus", YieldAllowed = true)]
		public void OnApplicationFocus(bool focusStatus)
		{
			base.RunEvents(this.focusEvents, new BloxEventArg("focusStatus", focusStatus));
		}

		[BloxEvent("Misc/OnApplicationPause", YieldAllowed = true)]
		public void OnApplicationPause(bool pauseStatus)
		{
			base.RunEvents(this.pauseEvents, new BloxEventArg("pauseStatus", pauseStatus));
		}

		[BloxEvent("Misc/OnApplicationQuit")]
		public void OnApplicationQuit()
		{
			base.RunEvents(this.quitEvents);
		}

		[BloxEvent("Misc/OnLevelWasLoaded")]
		public void OnLevelWasLoaded_DEPRICATED()
		{
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Misc/OnApplicationFocus".Equals(ev.ident))
			{
				this.focusEvents.Add(ev);
			}
			else if ("Misc/OnApplicationPause".Equals(ev.ident))
			{
				this.pauseEvents.Add(ev);
			}
			else if ("Misc/OnApplicationQuit".Equals(ev.ident))
			{
				this.quitEvents.Add(ev);
			}
			else if ("Misc/OnLevelWasLoaded".Equals(ev.ident))
			{
				Debug.LogError("This event is no longer supported: " + ev.ident);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
