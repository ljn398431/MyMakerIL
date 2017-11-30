using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Component : plyVarValueHandler
	{
		[SerializeField]
		private string storedTypeName;

		[NonSerialized]
		private Type componentType = typeof(Component);

		[NonSerialized]
		private plyVar cachedWrapper;

		public override Type variableType
		{
			get
			{
				return this.componentType;
			}
		}

		public override void SetStoredType(Type t)
		{
			this.componentType = t;
			this.storedTypeName = this.componentType.AssemblyQualifiedName;
		}

		public override object GetValue(plyVar wrapper)
		{
			this.cachedWrapper = wrapper;
			if (((wrapper.objRefs != null) ? wrapper.objRefs.Length : 0) != 0)
			{
				return wrapper.objRefs[0];
			}
			return null;
		}

		public override void SetValue(plyVar wrapper, object v)
		{
			this.cachedWrapper = wrapper;
			if (v == null)
			{
				wrapper.objRefs = null;
				return;
			}
			if (wrapper.objRefs == null || wrapper.objRefs.Length == 0)
			{
				wrapper.objRefs = new UnityEngine.Object[1];
			}
			if (v.GetType() == this.componentType)
			{
				wrapper.objRefs[0] = (v as Component);
				return;
			}
			GameObject gameObject = v as GameObject;
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				wrapper.objRefs[0] = gameObject.GetComponent(this.componentType);
				if (!(wrapper.objRefs[0] == (UnityEngine.Object)null))
					return;
				throw new Exception("The GameObject [" + gameObject.name + "] does not have the Component: " + this.componentType);
			}
			Component component = v as Component;
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				wrapper.objRefs[0] = component.GetComponent(this.componentType);
				if (!(wrapper.objRefs[0] == (UnityEngine.Object)null))
					return;
				throw new Exception("The GameObject [" + component.name + "] does not have the Component: " + this.componentType);
			}
			throw new Exception("Error: [" + v.GetType().Name + ": " + v.ToString() + "] => " + this.componentType);
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.cachedWrapper = wrapper;
			wrapper.objRefs = null;
		}

		public override string EncodeValues(plyVar wrapper)
		{
			this.cachedWrapper = wrapper;
			this.storedTypeName = this.componentType.AssemblyQualifiedName;
			return base.EncodeValues(wrapper);
		}

		public override void DecodeValues(plyVar wrapper, string data)
		{
			this.cachedWrapper = wrapper;
			JsonUtility.FromJsonOverwrite(data, this);
			if (string.IsNullOrEmpty(this.storedTypeName))
			{
				this.componentType = typeof(Component);
			}
			else
			{
				this.componentType = Type.GetType(this.storedTypeName);
			}
		}

		public Component GetValue()
		{
			if (this.cachedWrapper == null)
			{
				Debug.LogError("This should not happen!");
				return null;
			}
			if (((this.cachedWrapper.objRefs != null) ? this.cachedWrapper.objRefs.Length : 0) != 0)
			{
				return (Component)this.cachedWrapper.objRefs[0];
			}
			return null;
		}

		public void SetValue(object value)
		{
			this.SetValue(this.cachedWrapper, value);
		}

		public GameObject GetGameObject()
		{
			Component value = this.GetValue();
			if ((object)value == null)
			{
				return null;
			}
			return value.gameObject;
		}

		public T GetComponent<T>() where T : Component
		{
			Component value = this.GetValue();
			if ((UnityEngine.Object)value != (UnityEngine.Object)null && value.GetType() != typeof(T))
			{
				return value.GetComponent<T>();
			}
			return (T)value;
		}
	}
}
