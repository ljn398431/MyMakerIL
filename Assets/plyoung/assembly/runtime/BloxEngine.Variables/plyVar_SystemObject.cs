using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_SystemObject : plyVarValueHandler
	{
		[SerializeField]
		public object storedValue;

		public override Type variableType
		{
			get
			{
				return typeof(object);
			}
		}

		public override object GetValue(plyVar wrapper)
		{
			return this.storedValue;
		}

		public override void SetValue(plyVar wrapper, object v)
		{
			this.storedValue = v;
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = null;
		}

		public object GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
