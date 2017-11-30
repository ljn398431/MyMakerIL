using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Float : plyVarValueHandler
	{
		[SerializeField]
		public float storedValue;

		public override Type variableType
		{
			get
			{
				return typeof(float);
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
				throw new Exception("Can't convert null to Float value.");
			}
			if (v.GetType() == typeof(float))
			{
				this.storedValue = (float)v;
			}
			else
			{
				try
				{
					this.storedValue = Convert.ToSingle(v);
				}
				catch (Exception)
				{
					throw new Exception("Can't convert " + v.GetType().Name + " to Float.");
				}
			}
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = 0f;
		}

		public float GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
