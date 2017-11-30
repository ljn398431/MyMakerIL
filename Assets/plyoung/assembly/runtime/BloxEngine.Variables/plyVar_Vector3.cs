using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Vector3 : plyVarValueHandler
	{
		[SerializeField]
		public Vector3 storedValue = Vector3.zero;

		public override Type variableType
		{
			get
			{
				return typeof(Vector3);
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
				throw new Exception("Can't convert null to Vector3.");
			}
			if (v.GetType() == typeof(Vector3))
			{
				this.storedValue = (Vector3)v;
				return;
			}
			if (v.GetType() == typeof(Vector2))
			{
				this.storedValue = (Vector2)v;
				return;
			}
			if (v.GetType() == typeof(Vector4))
			{
				this.storedValue = (Vector4)v;
				return;
			}
			throw new Exception("Can't convert " + v.GetType().Name + " to Vector3.");
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = Vector3.zero;
		}

		public Vector3 GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
