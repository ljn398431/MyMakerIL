using BloxEngine.Variables;
using System;
using UnityEngine;

namespace BloxEngine
{
	[Serializable]
	[ExcludeFromBlox]
	public class Blox : ScriptableObject, ISerializationCallbackReceiver
	{
		public string ident;

		public string screenName;

		public BloxEvent[] events = new BloxEvent[0];

		public plyVariables variables;

		public bool scriptDirty = true;

		[NonSerialized]
		public bool bloxLoaded;

		[NonSerialized]
		public Type scriptType;

		private bool _isDirty;

		protected void OnEnable()
		{
			this.bloxLoaded = false;
			if (this.scriptDirty)
			{
				this.Deserialize();
			}
			else
			{
				plyVariables obj = this.variables;
				if (obj != null)
				{
					obj.BuildCache();
				}
				this.scriptType = BloxUtil.GetType("BloxGenerated." + this.ident, "Assembly-CSharp, Version=0.0.0.0, Culture=neutral");
			}
		}

		public void CopyTo(Blox def)
		{
			def.scriptDirty = true;
			def.variables = this.variables.Copy();
			def.events = new BloxEvent[this.events.Length];
			for (int i = 0; i < this.events.Length; i++)
			{
				def.events[i] = this.events[i].Copy();
			}
		}

		public override string ToString()
		{
			return this.screenName;
		}

		public void _SetDirty()
		{
			if (!Application.isPlaying)
			{
				this._isDirty = true;
			}
		}

		public void Deserialize()
		{
			this.bloxLoaded = true;
			plyVariables obj = this.variables;
			if (obj != null)
			{
				obj.BuildCache();
			}
			if (Application.isPlaying && !Application.isEditor)
				return;
			for (int i = 0; i < this.events.Length; i++)
			{
				this.events[i].Deserialize(this);
			}
		}

		public void Serialize()
		{
            Debug.Log("Serialize", "Blox", Color.yellow);
            plyVariables obj = this.variables;
			if (obj != null)
			{
				obj.Serialize();
			}
			if (this._isDirty)
			{
				this._isDirty = false;
				for (int i = 0; i < this.events.Length; i++)
				{
					this.events[i].Serialize();
				}
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
            Debug.Log("Blox OnAfterDeserialize", "Blox", Color.cyan);
            this.variables.Deserialize(false);
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
            Debug.Log("Blox OnBeforeSerialize", "Blox", Color.cyan);
			this.Serialize();
		}
	}
}
