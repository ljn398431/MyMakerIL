using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Bool : plyVarValueHandler
	{
		[SerializeField]
		public bool storedValue;

		public override Type variableType
		{
			get
			{
				return typeof(bool);
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
				throw new Exception("Can't convert null to Boolean.");
			}
			if (v.GetType() == typeof(bool))
			{
				this.storedValue = (bool)v;
			}
			else
			{
				try
				{
					this.storedValue = Convert.ToBoolean(v);
				}
				catch (Exception)
				{
					throw new Exception("Can't convert " + v.GetType().Name + " to Boolean.");
				}
			}
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = false;
		}

		public bool GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
