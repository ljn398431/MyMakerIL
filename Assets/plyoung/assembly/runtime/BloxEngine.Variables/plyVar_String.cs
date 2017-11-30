using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_String : plyVarValueHandler
	{
		[SerializeField]
		public string storedValue = "";

		public override Type variableType
		{
			get
			{
				return typeof(string);
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
				this.storedValue = null;
			}
			else if (v.GetType() == typeof(string))
			{
				this.storedValue = (string)v;
			}
			else
			{
				try
				{
					this.storedValue = Convert.ToString(v);
				}
				catch (Exception)
				{
					throw new Exception("Can't convert " + v.GetType().Name + " to String.");
				}
			}
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = "";
		}

		public string GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
