using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Color : plyVarValueHandler
	{
		[SerializeField]
		public Color storedValue = Color.black;

		public override Type variableType
		{
			get
			{
				return typeof(Color);
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
				throw new Exception("Can't convert null to Color.");
			}
			if (v.GetType() == typeof(Color))
			{
				this.storedValue = (Color)v;
				return;
			}
			if (v.GetType() == typeof(Color32))
			{
				this.storedValue = (Color32)v;
				return;
			}
			if (v.GetType() == typeof(Vector4))
			{
				this.storedValue = (Vector4)v;
				return;
			}
			throw new Exception("Can't convert " + v.GetType().Name + " to Color.");
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = Color.black;
		}

		public Color GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
