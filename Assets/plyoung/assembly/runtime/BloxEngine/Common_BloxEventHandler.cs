using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Common_BloxEventHandler : BloxEventHandler
	{
		private List<BloxEvent> awakeEvents = new List<BloxEvent>(1);

		private List<BloxEvent> startEvents = new List<BloxEvent>(1);

		private List<BloxEvent> destroyEvents = new List<BloxEvent>(1);

		private List<BloxEvent> enableEvents = new List<BloxEvent>(1);

		private List<BloxEvent> disableEvents = new List<BloxEvent>(1);

		[BloxEvent("Common/Awake", Order = 1)]
		public void Awake()
		{
			base.RunEvents(this.awakeEvents);
		}

		[BloxEvent("Common/Start", Order = 2, YieldAllowed = true)]
		public void Start()
		{
			base.RunEvents(this.startEvents);
		}

		[BloxEvent("Common/OnDestroy", Order = 5)]
		public void OnDestroy()
		{
			base.RunEvents(this.destroyEvents);
		}

		[BloxEvent("Common/OnEnable", Order = 5)]
		public void OnEnable()
		{
			base.RunEvents(this.enableEvents);
		}

		[BloxEvent("Common/OnDisable", Order = 5)]
		public void OnDisable()
		{
			base.RunEvents(this.disableEvents);
		}

		public override void AddEvent(BloxEvent ev)
		{
			if ("Common/Awake".Equals(ev.ident))
			{
				this.awakeEvents.Add(ev);
			}
			else if ("Common/Start".Equals(ev.ident))
			{
				this.startEvents.Add(ev);
			}
			else if ("Common/OnDestroy".Equals(ev.ident))
			{
				this.destroyEvents.Add(ev);
			}
			else if ("Common/OnEnable".Equals(ev.ident))
			{
				this.enableEvents.Add(ev);
			}
			else if ("Common/OnDisable".Equals(ev.ident))
			{
				this.disableEvents.Add(ev);
			}
			else
			{
				Debug.LogError("This event handler can't handle: " + ev.ident);
			}
		}
	}
}
