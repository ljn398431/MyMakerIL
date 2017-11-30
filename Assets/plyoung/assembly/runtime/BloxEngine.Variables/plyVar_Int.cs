using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Int : plyVarValueHandler
	{
		[SerializeField]
		public int storedValue;

		public override Type variableType
		{
			get
			{
				return typeof(int);
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
				throw new Exception("Can't convert null to Integer value.");
			}
			if (v.GetType() == typeof(int))
			{
				this.storedValue = (int)v;
			}
			else
			{
				try
				{
					this.storedValue = Convert.ToInt32(v);
				}
				catch (Exception)
				{
					throw new Exception("Can't convert " + v.GetType().Name + " to Integer.");
				}
			}
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = 0;
		}

		public int GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
