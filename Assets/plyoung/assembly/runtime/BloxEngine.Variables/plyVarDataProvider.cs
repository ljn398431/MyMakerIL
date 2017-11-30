using BloxEngine.Databinding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Variables
{
	[ExcludeFromBlox]
	public class plyVarDataProvider : DataProvider
	{
		public enum DataSourceOject : byte
		{
			ObjWithName = 0,
			ObjWithTag = 1,
			OfOwner = 2
		}

		public enum VariableType : byte
		{
			Global = 0,
			Object = 1,
			Blox = 2
		}

		public string varName;

		public VariableType varType;

		public DataSourceOject sourceObjType;

		public string objNameOrTag;

		public string bloxIdent;

		public string triggerBloxEvent;

		public DataBindingValueSource valSetterSource;

		[NonSerialized]
		private plyVar v;

		[NonSerialized]
		private Component sourceObj;

		public override Type DataType()
		{
			this.CacheVariable();
			if (this.v != null)
			{
				return this.v.variableType;
			}
			return typeof(object);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			plyVarDataProvider plyVarDataProvider = ScriptableObject.CreateInstance<plyVarDataProvider>();
			plyVarDataProvider.hideFlags = HideFlags.HideAndDontSave;
			plyVarDataProvider.varName = this.varName;
			plyVarDataProvider.varType = this.varType;
			plyVarDataProvider.sourceObjType = this.sourceObjType;
			plyVarDataProvider.objNameOrTag = this.objNameOrTag;
			plyVarDataProvider.bloxIdent = this.bloxIdent;
			plyVarDataProvider.triggerBloxEvent = this.triggerBloxEvent;
			plyVarDataProvider.valSetterSource = this.valSetterSource.Copy();
			return plyVarDataProvider;
		}

		public override object GetValue()
		{
			this.CacheVariable();
			plyVar obj = this.v;
			if (obj == null)
			{
				return null;
			}
			return obj.GetValue();
		}

		public override void Execute(object blackboardValue)
		{
			this.CacheVariable();
			if (this.valSetterSource != null && this.v != null)
			{
				this.v.SetValue(this.valSetterSource.GetValue(base.owner, blackboardValue));
			}
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
		}

		public override void Deserialize()
		{
			base.Deserialize();
		}

		private void CacheVariable()
		{
			if (Application.isPlaying && (this.v == null || !((UnityEngine.Object)this.sourceObj != (UnityEngine.Object)null)))
			{
				if (this.varType == VariableType.Global)
				{
					this.sourceObj = GlobalVariables.Instance;
					this.v = GlobalVariables.Instance.variables.FindVariable(this.varName);
					if (this.v == null)
					{
						Debug.LogErrorFormat("The Global Variable [{0}] does not exist.", this.varName);
					}
					else if (!base.isSetter)
					{
						this.v.onValueChange += this.BoundValueChanged;
					}
				}
				else if (this.varType == VariableType.Blox)
				{
					BloxContainer bloxContainer = this.CacheSourceObject() as BloxContainer;
					if (!((UnityEngine.Object)bloxContainer == (UnityEngine.Object)null))
					{
						if (!string.IsNullOrEmpty(this.triggerBloxEvent))
						{
							bloxContainer.TriggerEvent(this.triggerBloxEvent);
						}
						this.v = bloxContainer.FindVariable(this.bloxIdent, this.varName);
						if (this.v == null)
						{
							Blox blox = BloxGlobal.Instance.FindBloxDef(this.bloxIdent);
							Debug.LogErrorFormat("The Blox Variable [{0}] does not exist in Blox [{1}] on GameObject [{2}]", this.varName, ((object)blox != null) ? blox.screenName : null, bloxContainer.name);
						}
						else if (!base.isSetter)
						{
							this.v.onValueChange += this.BoundValueChanged;
						}
					}
				}
				else if (this.varType == VariableType.Object)
				{
					ObjectVariables objectVariables = this.CacheSourceObject() as ObjectVariables;
					if (!((UnityEngine.Object)objectVariables == (UnityEngine.Object)null))
					{
						this.v = objectVariables.FindVariable(this.varName);
						if (this.v == null)
						{
							Debug.LogErrorFormat("The Object Variable [{0}] does not exist on GameObject [{1}]", this.varName, objectVariables.name);
						}
						else if (!base.isSetter)
						{
							this.v.onValueChange += this.BoundValueChanged;
						}
					}
				}
			}
		}

		private Component CacheSourceObject()
		{
			if ((UnityEngine.Object)this.sourceObj != (UnityEngine.Object)null)
			{
				return this.sourceObj;
			}
			Type type = (this.varType == VariableType.Blox) ? typeof(BloxContainer) : typeof(ObjectVariables);
			if (this.sourceObjType == DataSourceOject.ObjWithName)
			{
				GameObject gameObject = GameObject.Find(this.objNameOrTag);
				this.sourceObj = (((object)gameObject != null) ? gameObject.GetComponent(type) : null);
				if ((UnityEngine.Object)this.sourceObj == (UnityEngine.Object)null)
				{
					Debug.LogError("[Variables DataProvider] The Blox Container or Object Variables could not be found on: " + this.objNameOrTag);
				}
			}
			else if (this.sourceObjType == DataSourceOject.ObjWithTag)
			{
				GameObject gameObject2 = GameObject.FindGameObjectWithTag(this.objNameOrTag);
				this.sourceObj = (((object)gameObject2 != null) ? gameObject2.GetComponent(type) : null);
				if ((UnityEngine.Object)this.sourceObj == (UnityEngine.Object)null)
				{
					Debug.LogError("[Variables DataProvider] The Blox Container or Object Variables could not be found on GameObject with tag: " + this.objNameOrTag);
				}
			}
			else if (this.sourceObjType == DataSourceOject.OfOwner)
			{
				Component owner = base.owner;
				this.sourceObj = (((object)owner != null) ? owner.GetComponent(type) : null);
				if ((UnityEngine.Object)this.sourceObj == (UnityEngine.Object)null)
				{
					Debug.LogError("[Variables DataProvider] The Blox Container or Object Variables could not be found on GameObject or 'owner' was not a valid context for the object source.");
				}
			}
			else
			{
				Debug.LogErrorFormat("[Variables DataProvider] The Blox Container or Object Variables could not be found.");
			}
			return this.sourceObj;
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
