using plyLib;
using System;
using UnityEngine;

namespace BloxEngine.Databinding
{
	/// <summary> DataBinding is used by DataProvider to bind to data </summary>
	[Serializable]
	[ExcludeFromBlox]
	public class DataBinding
	{
		public DataProvider dataprovider;

		[NonSerialized]
		private int pollerRefs;

		public object Value
		{
			get;
			private set;
		}

		private event Action onValueChanged;

		public DataBinding Copy()
		{
			DataBinding dataBinding = new DataBinding();
			this.CopyTo(dataBinding);
			return dataBinding;
		}

		public void CopyTo(DataBinding d)
		{
			d.dataprovider = null;
			if ((UnityEngine.Object)this.dataprovider != (UnityEngine.Object)null)
			{
				if (Application.isPlaying)
				{
					d.dataprovider = this.dataprovider.CreateRuntimeCopy();
				}
				else
				{
					d.dataprovider = this.dataprovider;
					d.dataprovider._SetDirty();
				}
			}
		}

		public void Initialize(Action onValueChangedCallback, Component owner)
		{
			if ((UnityEngine.Object)this.dataprovider != (UnityEngine.Object)null)
			{
				this.dataprovider.Initialize(owner);
				this.Value = this.dataprovider.GetValue();
				if (onValueChangedCallback != null)
				{
					this.onValueChanged += onValueChangedCallback;
					this.dataprovider.AddValueChangeListener(this.BoundValueChanged);
					this.RegisterProviderUpdater();
				}
			}
		}

		private void BoundValueChanged()
		{
			this.UpdateValue();
			Action obj = this.onValueChanged;
			if (obj != null)
			{
				obj();
			}
		}

		public bool UpdateValue()
		{
			if ((UnityEngine.Object)this.dataprovider != (UnityEngine.Object)null)
			{
				object value = this.dataprovider.GetValue();
				if (this.Value != value)
				{
					this.Value = value;
					return true;
				}
			}
			return false;
		}

		public object GetValue()
		{
			this.UpdateValue();
			return this.Value;
		}

		public void Execute(object blackboardValue)
		{
			DataProvider obj = this.dataprovider;
			if ((object)obj != null)
			{
				obj.Execute(blackboardValue);
			}
		}

		public Type DataType(bool isGetter)
		{
			DataProvider obj = this.dataprovider;
			if ((object)obj == null)
			{
				return null;
			}
			return obj.DataType();
		}

		public void RegisterProviderUpdater()
		{
			if ((UnityEngine.Object)this.dataprovider != (UnityEngine.Object)null && this.dataprovider.RequireDataPoller())
			{
				if (this.pollerRefs == 0)
				{
					SingletonMonoBehaviour<DataProviderUpdater>.Instance.RegisterProvider(this);
				}
				this.pollerRefs++;
			}
		}

		public void RemoveProviderUpdater()
		{
			if (this.pollerRefs != 0)
			{
				this.pollerRefs--;
				if (this.pollerRefs == 0 && SingletonMonoBehaviour<DataProviderUpdater>.HasInstance)
				{
					SingletonMonoBehaviour<DataProviderUpdater>.Instance.RemoveProvider(this);
				}
			}
		}

		public void DoUpdate()
		{
			if (this.UpdateValue())
			{
				Action obj = this.onValueChanged;
				if (obj != null)
				{
					obj();
				}
			}
		}
	}
}
