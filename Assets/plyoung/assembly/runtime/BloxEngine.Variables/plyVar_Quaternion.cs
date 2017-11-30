using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Quaternion : plyVarValueHandler
	{
		[SerializeField]
		public Quaternion storedValue = Quaternion.identity;

		public override Type variableType
		{
			get
			{
				return typeof(Quaternion);
			}
		}

		public override object GetValue(plyVar wrapper)
		{
			return this.storedValue;
		}

		public override void SetValue(plyVar wrapper, object v)
		{
			if (v == null)
			{
				throw new Exception("Can't convert null to Quaternion.");
			}
			if (v.GetType() == typeof(Quaternion))
			{
				this.storedValue = (Quaternion)v;
				return;
			}
			throw new Exception("Can't convert " + v.GetType().Name + " to Quaternion.");
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = Quaternion.identity;
		}

		public Quaternion GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
