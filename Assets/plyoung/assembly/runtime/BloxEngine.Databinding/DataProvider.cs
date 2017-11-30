using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Databinding
{
	/// <summary> The DataProvider can perform actions on data and return a value </summary>
	[ExcludeFromBlox]
	[HelpURL("https://plyoung.github.io/blox-databinding.html")]
	public class DataProvider : ScriptableObject, ISerializationCallbackReceiver
	{
		public string ident = "";

		public bool isSetter;

		[NonSerialized]
		protected Component owner;

		[NonSerialized]
		private List<DataBinding> databinds = new List<DataBinding>();

		[NonSerialized]
		private Action onValueChanged;

		protected bool _isDirty;

		public virtual Type DataType()
		{
			throw new Exception("[DataType] not implemented for: " + this);
		}

		public virtual void OnCreated()
		{
		}

		public virtual DataProvider CreateRuntimeCopy()
		{
			throw new Exception("[RuntimeCopyTo] not implemented for: " + this);
		}

		public virtual object GetValue()
		{
			throw new Exception("[GetValue] not implemented for: " + this);
		}

		public virtual void Execute(object blackboardValue)
		{
			throw new Exception("[Execute] not implemented for: " + this);
		}

		public virtual bool RequireDataPoller()
		{
			return false;
		}

		public void AddValueChangeListener(Action callback)
		{
			this.onValueChanged = (Action)Delegate.Combine(this.onValueChanged, callback);
		}

		public void RemoveValueChangeListener(Action callback)
		{
			this.onValueChanged = (Action)Delegate.Remove(this.onValueChanged, callback);
		}

		protected void RegisterBindWithCallbacks(DataBinding bind)
		{
			if (bind != null)
			{
				this.databinds.Add(bind);
				bind.Initialize(this.BoundValueChanged, this.owner);
			}
		}

		protected void RegisterBind(DataBinding bind)
		{
			if (bind != null)
			{
				bind.Initialize(null, this.owner);
			}
		}

		protected virtual void BoundValueChanged()
		{
			Action obj = this.onValueChanged;
			if (obj != null)
			{
				obj();
			}
		}

		public virtual void _SetDirty()
		{
			if (!Application.isPlaying)
			{
				this._isDirty = true;
			}
		}

		public virtual List<DataProvider> GetDataProvidersForDestruction()
		{
			return new List<DataProvider>();
		}

		protected bool IsValidForAutoDestroy(DataProvider p)
		{
			if ((UnityEngine.Object)p != (UnityEngine.Object)null)
			{
				return string.IsNullOrEmpty(p.ident);
			}
			return false;
		}

		public virtual void Initialize(Component owner)
		{
			this.owner = owner;
		}

		public virtual void Deserialize()
		{
		}

		public virtual void Serialize()
		{
			this._isDirty = false;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.Deserialize();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this._isDirty)
			{
				this._isDirty = false;
				this.Serialize();
			}
		}
	}
}
