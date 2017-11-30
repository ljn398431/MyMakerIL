using BloxEngine;
using BloxEngine.Databinding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	public class AttributeDataProvider : DataProvider
	{
		public enum ValueSource : byte
		{
			Caller = 0,
			Target = 1
		}

		public enum ValueType : byte
		{
			Value = 0,
			Max = 1
		}

		public int attId = -1;

		public ValueSource attSource;

		public ValueType attValType;

		public DataBinding attOwnerBind = new DataBinding();

		public DataBindingValueSource valSetterSource;

		private CharacterAttribute att;

		public override Type DataType()
		{
			return typeof(float);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			AttributeDataProvider attributeDataProvider = ScriptableObject.CreateInstance<AttributeDataProvider>();
			attributeDataProvider.hideFlags = HideFlags.HideAndDontSave;
			attributeDataProvider.attId = this.attId;
			attributeDataProvider.attSource = this.attSource;
			attributeDataProvider.attValType = this.attValType;
			attributeDataProvider.attOwnerBind = this.attOwnerBind.Copy();
			attributeDataProvider.valSetterSource = this.valSetterSource.Copy();
			return attributeDataProvider;
		}

		public override object GetValue()
		{
			this.CacheAttribute();
			if (this.att == null)
			{
				return 0f;
			}
			return (this.attValType == ValueType.Value) ? this.att.Value : this.att.MaxValue;
		}

		public override void Execute(object blackboardValue)
		{
			this.CacheAttribute();
			if (this.valSetterSource != null && this.att != null)
			{
				float num = BloxUtil.AsFloat(this.valSetterSource.GetValue(base.owner, blackboardValue));
				if (this.attValType == ValueType.Value)
				{
					this.att.Value = num;
				}
				else
				{
					this.att.MaxValue = num;
				}
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
			if (this.attSource == ValueSource.Caller)
			{
				this.CacheAttribute();
			}
			if (this.attSource == ValueSource.Target)
			{
				base.RegisterBindWithCallbacks(this.attOwnerBind);
			}
		}

		private void CacheAttribute()
		{
			if (this.att == null)
			{
				Component component = (this.attSource == ValueSource.Caller) ? base.owner : (this.attOwnerBind.GetValue() as Component);
				ICharacterAttributesOwner characterAttributesOwner = component as ICharacterAttributesOwner;
				if (characterAttributesOwner == null)
				{
					Debug.LogError("[Attribute Data Provider] The target object is not Attributes owner: " + (((object)component != null) ? component.gameObject.name : null), ((object)component != null) ? component.gameObject : null);
				}
				else
				{
					this.att = characterAttributesOwner.GetAttribute(this.attId);
					if (this.att == null)
					{
						string text = CharacterAttributeDefsAsset.Instance.IdToIdent(this.attId);
						Debug.LogErrorFormat("[Attribute Data Provider] Could not find Attribute [{0}] on [{1}]", text, ((object)component != null) ? component.gameObject.name : null, ((object)component != null) ? component.gameObject : null);
					}
					if (!base.isSetter)
					{
						if (this.attValType == ValueType.Value)
						{
							this.att.onValueChanged += this.BoundValueChanged;
						}
						else
						{
							this.att.onMaxValChanged += this.BoundValueChanged;
						}
					}
				}
			}
		}

		public override List<DataProvider> GetDataProvidersForDestruction()
		{
			List<DataProvider> list = new List<DataProvider>();
			if (this.attOwnerBind != null && base.IsValidForAutoDestroy(this.attOwnerBind.dataprovider))
			{
				list.Add(this.attOwnerBind.dataprovider);
				list.AddRange(this.attOwnerBind.dataprovider.GetDataProvidersForDestruction());
			}
			if (this.valSetterSource != null && this.valSetterSource.databind != null && base.IsValidForAutoDestroy(this.valSetterSource.databind.dataprovider))
			{
				list.Add(this.valSetterSource.databind.dataprovider);
				list.AddRange(this.valSetterSource.databind.dataprovider.GetDataProvidersForDestruction());
			}
			return list;
		}
	}
}
