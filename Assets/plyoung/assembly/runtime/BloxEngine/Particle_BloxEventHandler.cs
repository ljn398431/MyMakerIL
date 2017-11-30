using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Particle_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> events = new List<BloxEvent>(1);

		[BloxEvent("Collision/OnParticleCollision", Order = 100)]
		public void OnParticleCollision(GameObject otherObj)
		{
			base.RunEvents(this.events, new BloxEventArg("otherObj", otherObj));
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Collision/OnParticleCollision".Equals(ev.ident))
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
