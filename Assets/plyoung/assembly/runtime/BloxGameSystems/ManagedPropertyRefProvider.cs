using BloxEngine;
using BloxEngine.Databinding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	public class ManagedPropertyRefProvider : DataProvider
	{
		public string propertyName;

		public DataBindingValueSource valSetterSource;

		[NonSerialized]
		private ManagedProperty p;

		public override Type DataType()
		{
			return typeof(object);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			ManagedPropertyRefProvider managedPropertyRefProvider = ScriptableObject.CreateInstance<ManagedPropertyRefProvider>();
			managedPropertyRefProvider.hideFlags = HideFlags.HideAndDontSave;
			managedPropertyRefProvider.propertyName = this.propertyName;
			managedPropertyRefProvider.valSetterSource = this.valSetterSource.Copy();
			return managedPropertyRefProvider;
		}

		public override object GetValue()
		{
			ManagedProperty obj = this.p;
			if (obj == null)
			{
				return null;
			}
			return obj.Value;
		}

		public override void Execute(object blackboardValue)
		{
			PropertiesManager.SetValue(this.propertyName, this.valSetterSource.GetValue(base.owner, blackboardValue));
		}

		public override void Initialize(Component owner)
		{
			base.Initialize(owner);
			if (base.isSetter)
			{
				base.RegisterBind(this.valSetterSource.databind);
			}
			else
			{
				base.RegisterBindWithCallbacks(this.valSetterSource.databind);
			}
			if (!base.isSetter)
			{
				this.p = PropertiesManager.GetProperty(this.propertyName);
				ManagedProperty obj = this.p;
				if (obj != null)
				{
					obj.AddValueChangeListener(this.OnPropertyValueChange);
				}
			}
		}

		public override void Deserialize()
		{
			base.Deserialize();
		}

		private void OnPropertyValueChange()
		{
			this.BoundValueChanged();
		}

		public override List<DataProvider> GetDataProvidersForDestruction()
		{
			List<DataProvider> list = new List<DataProvider>();
			if (this.valSetterSource != null && this.valSetterSource.databind != null && base.IsValidForAutoDestroy(this.valSetterSource.databind.dataprovider))
			{
				list.Add(this.valSetterSource.databind.dataprovider);
				list.AddRange(this.valSetterSource.databind.dataprovider.GetDataProvidersForDestruction());
			}
			return list;
		}
	}
}
