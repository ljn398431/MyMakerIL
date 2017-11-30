using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Prefab : plyVarValueHandler
	{
		[NonSerialized]
		private plyVar cachedWrapper;

		public override Type variableType
		{
			get
			{
				return typeof(GameObject);
			}
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
			}
			else
			{
				if (wrapper.objRefs == null || wrapper.objRefs.Length == 0)
				{
					wrapper.objRefs = new UnityEngine.Object[1];
				}
				wrapper.objRefs[0] = (v as GameObject);
				if (!(wrapper.objRefs[0] != (UnityEngine.Object)null))
				{
					Component component = v as Component;
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						wrapper.objRefs[0] = component.gameObject;
						return;
					}
					throw new Exception("Can't convert [" + v.GetType().Name + ": " + v.ToString() + "] to GameObject.");
				}
			}
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.cachedWrapper = wrapper;
			wrapper.objRefs = null;
		}

		public override string EncodeValues(plyVar wrapper)
		{
			this.cachedWrapper = wrapper;
			return "";
		}

		public override void DecodeValues(plyVar wrapper, string data)
		{
			this.cachedWrapper = wrapper;
		}

		public GameObject GetValue()
		{
			if (this.cachedWrapper == null)
			{
				Debug.LogError("This should not happen!");
				return null;
			}
			if (((this.cachedWrapper.objRefs != null) ? this.cachedWrapper.objRefs.Length : 0) != 0)
			{
				return (GameObject)this.cachedWrapper.objRefs[0];
			}
			return null;
		}

		public void SetValue(object value)
		{
			this.SetValue(this.cachedWrapper, value);
		}
	}
}
