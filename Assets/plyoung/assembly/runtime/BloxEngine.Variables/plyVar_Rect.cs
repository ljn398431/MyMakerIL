using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Rect : plyVarValueHandler
	{
		[SerializeField]
		public Rect storedValue;

		public override Type variableType
		{
			get
			{
				return typeof(Rect);
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
				throw new Exception("Can't convert null to Rect.");
			}
			if (v.GetType() == typeof(Rect))
			{
				this.storedValue = (Rect)v;
				return;
			}
			throw new Exception("Can't convert " + v.GetType().Name + " to Rect.");
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = default(Rect);
		}

		public Rect GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
