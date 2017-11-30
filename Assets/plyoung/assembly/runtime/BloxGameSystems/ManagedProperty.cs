using BloxEngine;
using BloxEngine.Databinding;
using System;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class ManagedProperty
	{
		public string propertyName;

		public DataBinding getter = new DataBinding();

		public DataBinding setter = new DataBinding();

		public bool runDuringBoot;

		[NonSerialized]
		private bool inited;

		[NonSerialized]
		private Action onGetterValueChanged;

		public object Value
		{
			get
			{
				return this.getter.GetValue();
			}
		}

		public void Initialize()
		{
			if (!this.inited)
			{
				this.inited = true;
				this.setter.Initialize(null, null);
				this.getter.Initialize(this.OnGetterValueChanged, null);
			}
		}

		public override string ToString()
		{
			return this.propertyName;
		}

		public void AddValueChangeListener(Action callback)
		{
			this.onGetterValueChanged = (Action)Delegate.Remove(this.onGetterValueChanged, callback);
			this.onGetterValueChanged = (Action)Delegate.Combine(this.onGetterValueChanged, callback);
		}

		public void RemoveValueChangeListener(Action callback)
		{
			this.onGetterValueChanged = (Action)Delegate.Remove(this.onGetterValueChanged, callback);
		}

		private void OnGetterValueChanged()
		{
			Action obj = this.onGetterValueChanged;
			if (obj != null)
			{
				obj();
			}
		}
	}
}
