using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_UnityObject : plyVarValueHandler
	{
		[SerializeField]
		private string storedTypeName;

		[NonSerialized]
		private Type objectType = typeof(UnityEngine.Object);

		[NonSerialized]
		private plyVar cachedWrapper;

		public override Type variableType
		{
			get
			{
				return this.objectType;
			}
		}

		public override void SetStoredType(Type t)
		{
			this.objectType = t;
			this.storedTypeName = this.objectType.AssemblyQualifiedName;
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
			if (this.objectType.IsAssignableFrom(v.GetType()))
			{
				wrapper.objRefs[0] = (v as UnityEngine.Object);
				return;
			}
			throw new Exception("Error: [" + v.GetType().Name + ": " + v.ToString() + "] => " + this.objectType);
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.cachedWrapper = wrapper;
			wrapper.objRefs = null;
		}

		public override string EncodeValues(plyVar wrapper)
		{
			this.cachedWrapper = wrapper;
			this.storedTypeName = this.objectType.AssemblyQualifiedName;
			return base.EncodeValues(wrapper);
		}

		public override void DecodeValues(plyVar wrapper, string data)
		{
			this.cachedWrapper = wrapper;
			JsonUtility.FromJsonOverwrite(data, this);
			if (string.IsNullOrEmpty(this.storedTypeName))
			{
				this.objectType = typeof(UnityEngine.Object);
			}
			else
			{
				this.objectType = Type.GetType(this.storedTypeName);
			}
		}

		public UnityEngine.Object GetValue()
		{
			if (this.cachedWrapper == null)
			{
				Debug.LogError("This should not happen!");
				return null;
			}
			if (((this.cachedWrapper.objRefs != null) ? this.cachedWrapper.objRefs.Length : 0) != 0)
			{
				return this.cachedWrapper.objRefs[0];
			}
			return null;
		}

		public T GetObject<T>() where T : UnityEngine.Object
		{
			return (T)this.GetValue();
		}

		public void SetValue(object value)
		{
			this.SetValue(this.cachedWrapper, value);
		}
	}
}
