using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Vector2 : plyVarValueHandler
	{
		[SerializeField]
		public Vector2 storedValue = Vector2.zero;

		public override Type variableType
		{
			get
			{
				return typeof(Vector2);
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
				throw new Exception("Can't convert null to Vector2.");
			}
			if (v.GetType() == typeof(Vector2))
			{
				this.storedValue = (Vector2)v;
				return;
			}
			if (v.GetType() == typeof(Vector3))
			{
				this.storedValue = (Vector3)v;
				return;
			}
			if (v.GetType() == typeof(Vector4))
			{
				this.storedValue = (Vector4)v;
				return;
			}
			throw new Exception("Can't convert " + v.GetType().Name + " to Vector2.");
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = Vector2.zero;
		}

		public Vector2 GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
