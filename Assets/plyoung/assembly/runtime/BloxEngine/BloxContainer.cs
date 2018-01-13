using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BloxEngine
{
	[ExcludeFromBlox]
	[DisallowMultipleComponent]
	[AddComponentMenu("Blox/Blox Visual Script")]
	[HelpURL("https://plyoung.github.io/blox-container.html")]
	public class BloxContainer : MonoBehaviour, ISerializationCallbackReceiver
	{
		private class SendMessageInfo
		{
			public List<BloxMemberInfo> mi = new List<BloxMemberInfo>(0);

			public List<object> target = new List<object>(0);
		}

		[SerializeField]
		public GameObject bloxGlobalPrefab;

		[SerializeField]
		public List<string> bloxIdents = new List<string>();

		[SerializeField]
		private List<BloxVariables> bloxVars = new List<BloxVariables>();
        [SerializeField]
        private Dictionary<string, List<BloxEvent>> customEvents;
        [SerializeField]
        private Dictionary<string, BloxVariables> bloxVarCache;
        [SerializeField]
        private Dictionary<string, SendMessageInfo> sendMessageCache = new Dictionary<string, SendMessageInfo>(0);

		protected void Awake()
		{
			BloxGlobal.Create(this.bloxGlobalPrefab);
			this.customEvents = new Dictionary<string, List<BloxEvent>>();
			this.BuildVarCache();
			for (int i = 0; i < this.bloxIdents.Count; i++)
			{
				Blox blox = BloxGlobal.Instance.FindBloxDef(this.bloxIdents[i]);
				if (!((UnityEngine.Object)blox == (UnityEngine.Object)null))
				{
					if (!this.bloxVarCache.ContainsKey(this.bloxIdents[i]))
					{
						BloxVariables bloxVariables = new BloxVariables();
						bloxVariables.bloxIdent = this.bloxIdents[i];
						this.bloxVars.Add(bloxVariables);
						this.bloxVarCache.Add(bloxVariables.bloxIdent, bloxVariables);
						this._CheckVariables(bloxVariables, blox);
						bloxVariables.BuildCache();
					}
					this.AttachComponents(blox);
				}
			}
		}

		private void AttachComponents(Blox b)
		{
			if (b.scriptDirty || b.scriptType == null)
			{
				this.AttachEventHandlers(b);
			}
			else
			{
				base.gameObject.AddComponent(b.scriptType);
			}
		}

		private void AttachEventHandlers(Blox b)
		{
			if (!b.bloxLoaded)
			{
                Debug.Log("AttachEventHandlers", "BloxContainer", Color.yellow);
                b.Deserialize();
			}
			Common_BloxEventHandler common_BloxEventHandler = null;
			for (int i = 0; i < b.events.Length; i++)
			{
				BloxEvent bloxEvent = b.events[i].Init(this, b);
				if (bloxEvent != null)
				{
					if (bloxEvent.ident.Equals("Custom"))
					{
						if (!this.customEvents.ContainsKey(bloxEvent.screenName))
						{
							this.customEvents.Add(bloxEvent.screenName, new List<BloxEvent>());
						}
						this.customEvents[bloxEvent.screenName].Add(bloxEvent);
					}
					else
					{
						Type type = BloxGlobal.FindEventHandlerType(bloxEvent.ident);
						if (type == null)
						{
							Debug.LogWarning("Could not find a handler for: " + bloxEvent.ident, base.gameObject);
						}
						else
						{
							BloxEventHandler bloxEventHandler = (BloxEventHandler)base.gameObject.GetComponent(type);
							if ((UnityEngine.Object)bloxEventHandler == (UnityEngine.Object)null)
							{
								bloxEventHandler = (BloxEventHandler)base.gameObject.AddComponent(type);
								if (bloxEventHandler.GetType() == typeof(Common_BloxEventHandler))
								{
									common_BloxEventHandler = (Common_BloxEventHandler)bloxEventHandler;
								}
							}
							bloxEventHandler.AddEvent(bloxEvent);
						}
					}
				}
			}
			if ((UnityEngine.Object)common_BloxEventHandler != (UnityEngine.Object)null)
			{
				common_BloxEventHandler.Awake();
				common_BloxEventHandler.OnEnable();
			}
		}

		/// <summary> This is used to trigger Custom Events; not any of the System or other Defined Events</summary>
		public void TriggerEvent(string eventName)
		{
			base.gameObject.SendMessage(eventName, new BloxEventArg[0], SendMessageOptions.DontRequireReceiver);
			List<BloxEvent> list = default(List<BloxEvent>);
			if (this.customEvents == null)
			{
				Debug.LogError("The Event Cache is not yet initialized.", base.gameObject);
			}
			else if (this.customEvents.TryGetValue(eventName, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.RunEvent(list[i]);
				}
			}
		}

		/// <summary> This is used to trigger Custom Events; not any of the System or other Defined Events</summary>
		public void TriggerEvent(string eventName, BloxEventArg[] args)
		{
			if (args == null || args.Length == 0)
			{
				this.TriggerEvent(eventName);
			}
			else
			{
				base.gameObject.SendMessage(eventName, args, SendMessageOptions.DontRequireReceiver);
				List<BloxEvent> list = default(List<BloxEvent>);
				if (this.customEvents == null)
				{
					Debug.LogError("The Event Cache is not yet initialized.", base.gameObject);
				}
				else if (this.customEvents.TryGetValue(eventName, out list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						this.RunEvent(list[i], args);
					}
				}
			}
		}

		/// <summary> This is used to trigger Custom Events; not any of the System or other Defined Events</summary>
		public void TriggerEvent(string eventName, float afterSeconds)
		{
			if (afterSeconds > 0.0)
			{
				BloxGlobal.Instance.AddDelayedEvent(this, eventName, afterSeconds, (BloxEventArg[])null);
			}
			else
			{
				this.TriggerEvent(eventName);
			}
		}

		/// <summary> This is used to trigger Custom Events; not any of the System or other Defined Events</summary>
		public void TriggerEvent(string eventName, float afterSeconds, BloxEventArg[] args)
		{
			if (afterSeconds > 0.0)
			{
				BloxGlobal.Instance.AddDelayedEvent(this, eventName, afterSeconds, args);
			}
			else
			{
				this.TriggerEvent(eventName, args);
			}
		}

		/// <summary> A special version of SendMessage which can send multiple values to target function. You must send 
		/// exact value types in the order the target function expects them to be. This will grab the first function from
		/// each component with the given name so be sure to use unique names else use the 2nd variation of this function. </summary>
		public void SendMessage(string functionName, params object[] values)
		{
			SendMessageInfo sendMessageInfo = null;
			if (!this.sendMessageCache.TryGetValue(functionName, out sendMessageInfo))
			{
				sendMessageInfo = new SendMessageInfo();
				this.sendMessageCache.Add(functionName, sendMessageInfo);
				Component[] components = base.gameObject.GetComponents<Component>();
				Debug.Log(components.Length);
				for (int i = 0; i < components.Length; i++)
				{
					MethodInfo method = components[i].GetType().GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public);
					if (method != null)
					{
						sendMessageInfo.mi.Add(new BloxMemberInfo(method, null));
						sendMessageInfo.target.Add(components[i]);
					}
				}
			}
			for (int j = 0; j < sendMessageInfo.target.Count; j++)
			{
				sendMessageInfo.mi[j].Invoke(sendMessageInfo.target[j], values);
			}
		}

		/// <summary> A special version of SendMessage which can send multiple values to target function. You must send 
		/// exact value types in the order the target function expects them to be. This is a safer version of the function
		/// which requires you to specify the expected types on the target function so that the exact function can 
		/// be matched in components on the GameObject. </summary>
		public void SendMessage(string functionName, Type[] expectedTypes, params object[] values)
		{
			SendMessageInfo sendMessageInfo = null;
			if (!this.sendMessageCache.TryGetValue(functionName, out sendMessageInfo))
			{
				sendMessageInfo = new SendMessageInfo();
				this.sendMessageCache.Add(functionName, sendMessageInfo);
				Component[] components = base.gameObject.GetComponents<Component>();
				for (int i = 0; i < components.Length; i++)
				{
					MethodInfo method = components[i].GetType().GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public, null, expectedTypes, null);
					if (method != null)
					{
						sendMessageInfo.mi.Add(new BloxMemberInfo(method, null));
						sendMessageInfo.target.Add(components[i]);
					}
				}
			}
			for (int j = 0; j < sendMessageInfo.target.Count; j++)
			{
				sendMessageInfo.mi[j].Invoke(sendMessageInfo.target[j], values);
			}
		}

		private void RunEvent(BloxEvent ev, params BloxEventArg[] args)
		{
			ev.GetReadyToRun(args);
			if (ev.canYield)
			{
				BloxGlobal.Instance.StartCoroutine(ev.RunYield());
			}
			else
			{
				ev.Run();
			}
		}

		public plyVar FindVariable(string bloxIdent, string varName)
		{
			BloxVariables bloxVariables = this.GetBloxVariables(bloxIdent, null);
			if (bloxVariables == null)
			{
				return null;
			}
			return bloxVariables.FindVariable(varName);
		}

		public BloxVariables GetBloxVariables(string bloxIdent, Blox b = null)
		{
			BloxVariables bloxVariables = null;
			if ((UnityEngine.Object)b == (UnityEngine.Object)null)
			{
				if (this.bloxVarCache == null)
				{
					this.BuildVarCache();
				}
				if (this.bloxVarCache.TryGetValue(bloxIdent, out bloxVariables))
				{
					return bloxVariables;
				}
				Debug.LogError("This Blox Container does not contain variables for a Blox with ident: " + bloxIdent, base.gameObject);
			}
			else
			{
				if (this.bloxVars.Count > 0)
				{
					for (int num = this.bloxVars.Count - 1; num >= 0; num--)
					{
						if (!this.bloxIdents.Contains(this.bloxVars[num].bloxIdent))
						{
							this.bloxVars.RemoveAt(num);
						}
					}
				}
				if (this.bloxVarCache == null)
				{
					this.bloxVarCache = new Dictionary<string, BloxVariables>();
					for (int i = 0; i < this.bloxVars.Count; i++)
					{
						this.bloxVarCache.Add(this.bloxVars[i].bloxIdent, this.bloxVars[i]);
					}
				}
				this.bloxVarCache.TryGetValue(bloxIdent, out bloxVariables);
				if (bloxVariables == null)
				{
					bloxVariables = new BloxVariables();
					bloxVariables.bloxIdent = bloxIdent;
					this.bloxVars.Add(bloxVariables);
					this.bloxVarCache.Add(bloxIdent, bloxVariables);
				}
				else if (!Application.isPlaying)
				{
					bloxVariables.Deserialize(false);
				}
				this._CheckVariables(bloxVariables, b);
			}
			return bloxVariables;
		}

		private void BuildVarCache()
		{
			this.bloxVarCache = new Dictionary<string, BloxVariables>();
			for (int i = 0; i < this.bloxVars.Count; i++)
			{
				this.bloxVarCache.Add(this.bloxVars[i].bloxIdent, this.bloxVars[i]);
				this._CheckVariables(this.bloxVars[i], BloxGlobal.Instance.FindBloxDef(this.bloxVars[i].bloxIdent));
				this.bloxVars[i].BuildCache();
			}
		}

		public void _RemoveBloxVariables(string bloxIdent)
		{
			if (this.bloxVarCache != null && this.bloxVarCache.ContainsKey(bloxIdent))
			{
				this.bloxVarCache.Remove(bloxIdent);
			}
			int num = 0;
			while (true)
			{
				if (num < this.bloxVars.Count)
				{
					if (!this.bloxVars[num].bloxIdent.Equals(bloxIdent))
					{
						num++;
						continue;
					}
					break;
				}
				return;
			}
			this.bloxVars.RemoveAt(num);
		}

		public void _ClearBloxVarCache()
		{
			this.bloxVarCache = null;
		}

		private void _CheckVariables(BloxVariables v, Blox b)
		{
			if (v != null && !((UnityEngine.Object)b == (UnityEngine.Object)null))
			{
				if (v.varDefs.Count > 0)
				{
					for (int num = v.varDefs.Count - 1; num >= 0; num--)
					{
						plyVar plyVar = v.varDefs[num];
						bool flag = false;
						for (int i = 0; i < b.variables.varDefs.Count; i++)
						{
							plyVar plyVar2 = b.variables.varDefs[i];
							if (plyVar.ident == plyVar2.ident)
							{
								plyVar.name = plyVar2.name;
								if (plyVar.variableType != plyVar2.variableType)
								{
									plyVar2.CopyTo(plyVar);
								}
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							v.varDefs.RemoveAt(num);
						}
					}
				}
				for (int j = 0; j < b.variables.varDefs.Count; j++)
				{
					plyVar plyVar3 = b.variables.varDefs[j];
					bool flag2 = false;
					int num2 = 0;
					while (num2 < v.varDefs.Count)
					{
						if (plyVar3.ident != v.varDefs[num2].ident)
						{
							num2++;
							continue;
						}
						flag2 = true;
						break;
					}
					if (!flag2)
					{
						v.varDefs.Add(plyVar3.Copy());
					}
				}
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			for (int i = 0; i < this.bloxVars.Count; i++)
			{
				this.bloxVars[i].Deserialize(true);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			for (int i = 0; i < this.bloxVars.Count; i++)
			{
				this.bloxVars[i].Serialize();
			}
		}
	}
}
