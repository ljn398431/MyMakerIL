using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	public class MemberBindProvider : DataProvider
	{
		public enum DataSourceOject : byte
		{
			None = 0,
			WithName = 1,
			WithTag = 2,
			OfType = 3,
			Instance = 4,
			Owner = 5
		}

		public DataSourceOject sourceObjType;

		public string memberStoredData;

		public string sourceObjData;

		public DataBindingValueSource[] valSetterSources = new DataBindingValueSource[0];

		[NonSerialized]
		public BloxMemberInfo member;

		[NonSerialized]
		public BloxMemberInfo instanceMember;

		[NonSerialized]
		private Property property;

		[NonSerialized]
		private bool inited;

		[NonSerialized]
		private object memberContext;

		[NonSerialized]
		private EventInfo _boundMemberValueChangedEvent;

		public override Type DataType()
		{
			BloxMemberInfo obj = this.member;
			if (obj == null)
			{
				return null;
			}
			return obj.ReturnType;
		}

		public override object GetValue()
		{
			if (!this.inited)
			{
				this.HookWithContext(this.MemberContext());
			}
			if (this.property != null)
			{
				return this.property.Value;
			}
			return this.member.GetValue(this.MemberContext());
		}

		public override void Execute(object blackboardValue)
		{
			if (this.member != null)
			{
				if (!this.inited)
				{
					this.HookWithContext(this.MemberContext());
				}
				if (this.property != null)
				{
					this.property.Value = this.valSetterSources[0].GetValue(base.owner, blackboardValue);
				}
				else if (this.member.IsFieldOrProperty)
				{
					this.member.SetValue(this.MemberContext(), this.valSetterSources[0].GetValue(base.owner, blackboardValue));
				}
				else
				{
					object[] array = new object[this.valSetterSources.Length];
					for (int i = 0; i < this.valSetterSources.Length; i++)
					{
						array[i] = this.valSetterSources[i].GetValue(base.owner, blackboardValue);
					}
					this.member.Invoke(this.MemberContext(), array);
				}
			}
		}

		public override DataProvider CreateRuntimeCopy()
		{
			MemberBindProvider memberBindProvider = ScriptableObject.CreateInstance<MemberBindProvider>();
			memberBindProvider.hideFlags = HideFlags.HideAndDontSave;
			memberBindProvider.sourceObjType = this.sourceObjType;
			memberBindProvider.memberStoredData = this.memberStoredData;
			memberBindProvider.sourceObjData = this.sourceObjData;
			memberBindProvider.valSetterSources = new DataBindingValueSource[this.valSetterSources.Length];
			for (int i = 0; i < this.valSetterSources.Length; i++)
			{
				memberBindProvider.valSetterSources[i] = this.valSetterSources[i].Copy();
			}
			memberBindProvider.member = this.member;
			memberBindProvider.instanceMember = this.instanceMember;
			memberBindProvider.property = this.property;
			memberBindProvider.inited = false;
			memberBindProvider.memberContext = null;
			memberBindProvider._boundMemberValueChangedEvent = null;
			return memberBindProvider;
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

		public override void Deserialize()
		{
			this.member = BloxMemberInfo.DecodeMember(this.memberStoredData);
			if (this.sourceObjType == DataSourceOject.Instance)
			{
				this.instanceMember = BloxMemberInfo.DecodeMember(this.sourceObjData);
			}
		}

		public override void Serialize()
		{
			base.Serialize();
			this.memberStoredData = BloxMemberInfo.EncodeMember(this.member);
			if (this.sourceObjType == DataSourceOject.Instance)
			{
				this.sourceObjData = BloxMemberInfo.EncodeMember(this.instanceMember);
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

		private object MemberContext()
		{
			if (this.member != null && !this.member.IsStatic)
			{
				if (this.memberContext != null)
				{
					return this.memberContext;
				}
				Type reflectedType = this.member.ReflectedType;
				if (this.sourceObjType == DataSourceOject.WithName)
				{
					GameObject gameObject = GameObject.Find(this.sourceObjData);
					this.memberContext = (((object)gameObject != null) ? gameObject.GetComponent(reflectedType) : null);
					if (this.memberContext == null)
					{
						Debug.LogErrorFormat("[MemberBind] The context for member [{0}] could not be resolved. This will lead to further errors. Object with Name [{1}] and Component [{2}] was expected in scene.", BloxMemberInfo.SimpleMemberPath(this.member), this.sourceObjData, reflectedType.Name);
					}
				}
				else if (this.sourceObjType == DataSourceOject.WithTag)
				{
					GameObject gameObject2 = GameObject.FindGameObjectWithTag(this.sourceObjData);
					this.memberContext = (((object)gameObject2 != null) ? gameObject2.GetComponent(reflectedType) : null);
					if (this.memberContext == null)
					{
						Debug.LogErrorFormat("[MemberBind] The context for member [{0}] could not be resolved. This will lead to further errors. Object with Tag [{1}] and Component [{2}] was expected in scene.", BloxMemberInfo.SimpleMemberPath(this.member), this.sourceObjData, reflectedType.Name);
					}
				}
				else if (this.sourceObjType == DataSourceOject.OfType)
				{
					this.memberContext = UnityEngine.Object.FindObjectOfType(reflectedType);
					if (this.memberContext == null)
					{
						Debug.LogErrorFormat("[MemberBind] The context for member [{0}] could not be resolved. This will lead to further errors. Object with Component [{1}] was expected in scene.", BloxMemberInfo.SimpleMemberPath(this.member), reflectedType.Name);
					}
				}
				else if (this.sourceObjType == DataSourceOject.Instance)
				{
					if (this.instanceMember != null)
					{
						this.memberContext = this.instanceMember.GetValue(null);
					}
					if (this.memberContext == null)
					{
						Debug.LogErrorFormat("[MemberBind] The context for member [{0}] could not be resolved via singleton instance data. This will lead to further errors.", BloxMemberInfo.SimpleMemberPath(this.member));
					}
				}
				else if (this.sourceObjType == DataSourceOject.Owner)
				{
					Component owner = base.owner;
					this.memberContext = (((object)owner != null) ? owner.GetComponent(reflectedType) : null);
					if (this.memberContext == null)
					{
						Debug.LogErrorFormat("[MemberBind] The context for member [{0}] could not be resolved via 'owner'. This will lead to further errors.", BloxMemberInfo.SimpleMemberPath(this.member));
					}
				}
				else
				{
					Debug.LogErrorFormat("[MemberBind] The context for member [{0}] could not be resolved. This will lead to further errors. No source object was specified.", BloxMemberInfo.SimpleMemberPath(this.member));
				}
				return this.memberContext;
			}
			return null;
		}

		private void HookWithContext(object context)
		{
			this.inited = true;
			this.property = this.member.GetPropertyT(context);
			if (!base.isSetter)
			{
				if (this.property != null)
				{
					this.property.onValueChanged += this.BoundPropertyValueChanged;
				}
				else if (this.member.IsFieldOrProperty)
				{
					EventInfo eventInfo = null;
					EventInfo[] events = this.member.ReflectedType.GetEvents();
					for (int i = 0; i < events.Length; i++)
					{
						object[] customAttributes = events[i].GetCustomAttributes(typeof(PropertyChangedEventAttribute), true);
						for (int j = 0; j < customAttributes.Length; j++)
						{
							PropertyChangedEventAttribute propertyChangedEventAttribute = (PropertyChangedEventAttribute)customAttributes[j];
							int num = 0;
							while (num < propertyChangedEventAttribute.Properties.Length)
							{
								if (!(propertyChangedEventAttribute.Properties[num] == this.member.Name))
								{
									num++;
									continue;
								}
								eventInfo = events[i];
								break;
							}
							if (eventInfo != null)
								break;
						}
						if (eventInfo != null)
							break;
					}
					if (eventInfo != null)
					{
						this._boundMemberValueChangedEvent = eventInfo;
						eventInfo.AddEventHandler(context, Delegate.CreateDelegate(eventInfo.EventHandlerType, this, "BoundValueChanged"));
					}
				}
			}
		}

		public override bool RequireDataPoller()
		{
			this.HookWithContext(this.MemberContext());
			if (this.property == null)
			{
				return this._boundMemberValueChangedEvent == null;
			}
			return false;
		}

		private void BoundPropertyValueChanged(object newVal)
		{
			base.BoundValueChanged();
		}

		private void BoundValueChanged(string propertyName)
		{
			if (this.member != null && this.member.Name.Equals(propertyName))
			{
				base.BoundValueChanged();
			}
		}
	}
}
