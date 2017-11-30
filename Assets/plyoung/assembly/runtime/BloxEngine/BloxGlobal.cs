using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[ExcludeFromBlox]
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	[HelpURL("https://plyoung.github.io/blox-global.html")]
	public class BloxGlobal : MonoBehaviour
	{
		private class DelayedEvent
		{
			public BloxContainer container;

			public string eventName;

			public BloxEventArg[] args;

			public float timer;
		}

		public List<Blox> bloxDefs = new List<Blox>();

		public int deadlockDetect = 999;

		private static Dictionary<string, Type> eventHandlerTypes = new Dictionary<string, Type>();

		private static Dictionary<string, Blox> bloxDefCache = new Dictionary<string, Blox>();

		private List<DelayedEvent> delayedEvents = new List<DelayedEvent>();

		private int delayedEventIdx;

		public static BloxGlobal Instance
		{
			get;
			private set;
		}

		public static void Create(GameObject bloxGlobalPrefab)
		{
			if ((UnityEngine.Object)BloxGlobal.Instance == (UnityEngine.Object)null && (UnityEngine.Object)bloxGlobalPrefab != (UnityEngine.Object)null)
			{
				UnityEngine.Object.Instantiate(bloxGlobalPrefab);
			}
		}

		protected void Awake()
		{
			BloxGlobal.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			base.gameObject.name = "BloxGlobal";
			this.CleanupBloxDefsListAndBuildCache(false);
		}

		protected void Update()
		{
			if (this.delayedEvents.Count != 0)
			{
				this.delayedEventIdx = this.delayedEvents.Count - 1;
				while (this.delayedEventIdx >= 0)
				{
					if ((UnityEngine.Object)this.delayedEvents[this.delayedEventIdx].container == (UnityEngine.Object)null || (UnityEngine.Object)this.delayedEvents[this.delayedEventIdx].container.gameObject == (UnityEngine.Object)null)
					{
						this.delayedEvents.RemoveAt(this.delayedEventIdx);
					}
					else
					{
						this.delayedEvents[this.delayedEventIdx].timer -= Time.deltaTime;
						if (this.delayedEvents[this.delayedEventIdx].timer <= 0.0)
						{
							DelayedEvent delayedEvent = this.delayedEvents[this.delayedEventIdx];
							delayedEvent.container.TriggerEvent(delayedEvent.eventName, delayedEvent.args);
							this.delayedEvents.RemoveAt(this.delayedEventIdx);
						}
					}
					this.delayedEventIdx--;
				}
			}
		}

		public void AddDelayedEvent(BloxContainer container, string eventName, float afterSeconds, params BloxEventArg[] args)
		{
			if (!((UnityEngine.Object)container == (UnityEngine.Object)null) && !string.IsNullOrEmpty(eventName))
			{
				this.delayedEvents.Add(new DelayedEvent
				{
					container = container,
					eventName = eventName,
					timer = afterSeconds,
					args = args
				});
			}
		}

		public bool CleanupBloxDefsListAndBuildCache(bool forceRefresh)
		{
			if (this.bloxDefs.Count == 0)
			{
				return false;
			}
			bool result = false;
			for (int num = this.bloxDefs.Count - 1; num >= 0; num--)
			{
				if ((UnityEngine.Object)this.bloxDefs[num] == (UnityEngine.Object)null)
				{
					result = true;
					this.bloxDefs.RemoveAt(num);
				}
			}
			if (BloxGlobal.bloxDefCache.Count == 0 | forceRefresh)
			{
				BloxGlobal.bloxDefCache.Clear();
				for (int i = 0; i < this.bloxDefs.Count; i++)
				{
					BloxGlobal.bloxDefCache.Add(this.bloxDefs[i].ident, this.bloxDefs[i]);
				}
			}
			return result;
		}

		public Blox FindBloxDef(string ident)
		{
			Blox result = null;
			if (BloxGlobal.bloxDefCache.TryGetValue(ident, out result))
			{
				return result;
			}
			return null;
		}

		public Blox FindBloxDefByName(string bloxName)
		{
			for (int i = 0; i < this.bloxDefs.Count; i++)
			{
				if (this.bloxDefs[i].screenName.Equals(bloxName))
				{
					return this.bloxDefs[i];
				}
			}
			return null;
		}

		public string FindBloxIdentByName(string bloxName)
		{
			Blox blox = this.FindBloxDefByName(bloxName);
			if ((object)blox == null)
			{
				return null;
			}
			return blox.ident;
		}

		public static void RegisterEventHandlerType(string ident, Type type)
		{
			if (!BloxGlobal.eventHandlerTypes.ContainsKey(ident))
			{
				BloxGlobal.eventHandlerTypes.Add(ident, type);
			}
		}

		public static Type FindEventHandlerType(string ident)
		{
			Type result = null;
			BloxGlobal.eventHandlerTypes.TryGetValue(ident, out result);
			return result;
		}
	}
}
