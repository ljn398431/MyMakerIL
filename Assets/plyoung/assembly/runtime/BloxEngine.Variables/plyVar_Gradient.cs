using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Gradient : plyVarValueHandler
	{
		[SerializeField]
		public Gradient storedValue = new Gradient();

		public override Type variableType
		{
			get
			{
				return typeof(Gradient);
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
				throw new Exception("Can't convert null to Gradient.");
			}
			if (v.GetType() == typeof(Gradient))
			{
				this.storedValue = (Gradient)v;
				return;
			}
			throw new Exception("Can't convert " + v.GetType().Name + " to Gradient.");
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = new Gradient();
		}

		public Gradient GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
