using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Renderer_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> onBecameInvisibleEvents = new List<BloxEvent>(1);

		private List<BloxEvent> onBecameVisible = new List<BloxEvent>(1);

		[BloxEvent("Misc/OnBecameInvisible")]
		public void OnBecameInvisible()
		{
			base.RunEvents(this.onBecameInvisibleEvents);
		}

		[BloxEvent("Misc/OnBecameVisible")]
		public void OnBecameVisible()
		{
			base.RunEvents(this.onBecameVisible);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Misc/OnBecameInvisible".Equals(ev.ident))
			{
				this.onBecameInvisibleEvents.Add(ev);
			}
			else if ("Misc/OnBecameVisible".Equals(ev.ident))
			{
				this.onBecameVisible.Add(ev);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
