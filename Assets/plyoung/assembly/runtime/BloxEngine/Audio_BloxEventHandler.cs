using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Audio_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Misc/OnAudioFilterRead", Order = 5000)]
		public void OnAudioFilterRead(float[] data, int channels)
		{
			base.RunEvents(this.events, new BloxEventArg("data", data), new BloxEventArg("channels", channels));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Misc/OnAudioFilterRead".Equals(ev.ident))
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
