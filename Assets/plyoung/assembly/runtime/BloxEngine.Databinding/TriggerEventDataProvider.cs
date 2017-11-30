using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	public class TriggerEventDataProvider : DataProvider
	{
		public enum DataSourceOject : byte
		{
			WithName = 0,
			WithTag = 1,
			Owner = 2
		}

		public DataSourceOject sourceObjType;

		public string objNameOrTag;

		public string bloxIdent;

		public string triggerBloxEvent;

		public string varName;

		public DataBindingValueSource[] valSetterSources = new DataBindingValueSource[0];

		[NonSerialized]
		private plyVar v;

		[NonSerialized]
		private BloxContainer bloxContainer;

		public override Type DataType()
		{
			return typeof(object);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			TriggerEventDataProvider triggerEventDataProvider = ScriptableObject.CreateInstance<TriggerEventDataProvider>();
			triggerEventDataProvider.hideFlags = HideFlags.HideAndDontSave;
			triggerEventDataProvider.sourceObjType = this.sourceObjType;
			triggerEventDataProvider.objNameOrTag = this.objNameOrTag;
			triggerEventDataProvider.bloxIdent = this.bloxIdent;
			triggerEventDataProvider.triggerBloxEvent = this.triggerBloxEvent;
			triggerEventDataProvider.varName = this.varName;
			triggerEventDataProvider.valSetterSources = new DataBindingValueSource[this.valSetterSources.Length];
			for (int i = 0; i < this.valSetterSources.Length; i++)
			{
				triggerEventDataProvider.valSetterSources[i] = this.valSetterSources[i].Copy();
			}
			return triggerEventDataProvider;
		}

		public override object GetValue()
		{
			this.UpdateLocalValue(null);
			plyVar obj = this.v;
			if (obj == null)
			{
				return null;
			}
			return obj.GetValue();
		}

		public override void Execute(object blackboardValue)
		{
			this.UpdateLocalValue(blackboardValue);
		}

		public override void Initialize(Component owner)
		{
			base.Initialize(owner);
			if (base.isSetter)
			{
				for (int i = 0; i < this.valSetterSources.Length; i++)
				{
					base.RegisterBind(this.valSetterSources[i].databind);
				}
			}
			else
			{
				for (int j = 0; j < this.valSetterSources.Length; j++)
				{
					base.RegisterBindWithCallbacks(this.valSetterSources[j].databind);
				}
			}
		}

		protected override void BoundValueChanged()
		{
			this.UpdateLocalValue(null);
			base.BoundValueChanged();
		}

		private void UpdateLocalValue(object blackboardValue)
		{
			this.CacheBloxContainer();
			if (!((UnityEngine.Object)this.bloxContainer == (UnityEngine.Object)null) && !string.IsNullOrEmpty(this.triggerBloxEvent))
			{
				BloxEventArg[] array = new BloxEventArg[this.valSetterSources.Length];
				for (int i = 0; i < this.valSetterSources.Length; i++)
				{
					array[i] = new BloxEventArg("param" + i, this.valSetterSources[i].GetValue(base.owner, blackboardValue));
				}
				this.bloxContainer.TriggerEvent(this.triggerBloxEvent, array);
				this.CacheVariable();
			}
		}

		private void CacheVariable()
		{
			if (this.v == null && !((UnityEngine.Object)this.bloxContainer == (UnityEngine.Object)null))
			{
				this.v = this.bloxContainer.FindVariable(this.bloxIdent, this.varName);
				if (this.v == null)
				{
					Blox blox = BloxGlobal.Instance.FindBloxDef(this.bloxIdent);
					Debug.LogErrorFormat("The Blox Variable [{0}] does not exist in Blox [{1}] on GameObject [{2}]", this.varName, ((object)blox != null) ? blox.screenName : null, this.bloxContainer.name);
				}
			}
		}

		private void CacheBloxContainer()
		{
			if (!((UnityEngine.Object)this.bloxContainer != (UnityEngine.Object)null))
			{
				if (this.sourceObjType == DataSourceOject.WithName)
				{
					GameObject gameObject = GameObject.Find(this.objNameOrTag);
					this.bloxContainer = (((object)gameObject != null) ? gameObject.GetComponent<BloxContainer>() : null);
					if ((UnityEngine.Object)this.bloxContainer == (UnityEngine.Object)null)
					{
						Debug.LogError("[TriggerEvent DataProvider] The Blox Container could not be found on: " + this.objNameOrTag);
					}
				}
				else if (this.sourceObjType == DataSourceOject.WithTag)
				{
					GameObject gameObject2 = GameObject.FindGameObjectWithTag(this.objNameOrTag);
					this.bloxContainer = (((object)gameObject2 != null) ? gameObject2.GetComponent<BloxContainer>() : null);
					if ((UnityEngine.Object)this.bloxContainer == (UnityEngine.Object)null)
					{
						Debug.LogError("[TriggerEvent DataProvider] The Blox Container could not be found on GameObject with tag: " + this.objNameOrTag);
					}
				}
				else if (this.sourceObjType == DataSourceOject.Owner)
				{
					Component owner = base.owner;
					this.bloxContainer = (((object)owner != null) ? owner.GetComponent<BloxContainer>() : null);
					if ((UnityEngine.Object)this.bloxContainer == (UnityEngine.Object)null)
					{
						Debug.LogError("[TriggerEvent DataProvider] The Blox Container could not be found on GameObject or 'owner' was not a valid context for the object source.");
					}
				}
				else
				{
					Debug.LogErrorFormat("[TriggerEvent DataProvider] The Blox Container could not be found.");
				}
			}
		}

		public override List<DataProvider> GetDataProvidersForDestruction()
		{
			List<DataProvider> list = new List<DataProvider>();
			for (int i = 0; i < this.valSetterSources.Length; i++)
			{
				if (this.valSetterSources[i] != null && this.valSetterSources[i].databind != null && base.IsValidForAutoDestroy(this.valSetterSources[i].databind.dataprovider))
				{
					list.Add(this.valSetterSources[i].databind.dataprovider);
					list.AddRange(this.valSetterSources[i].databind.dataprovider.GetDataProvidersForDestruction());
				}
			}
			return list;
		}
	}
}
