using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_AnimationCurve : plyVarValueHandler
	{
		[SerializeField]
		public AnimationCurve storedValue = new AnimationCurve();

		public override Type variableType
		{
			get
			{
				return typeof(AnimationCurve);
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
				throw new Exception("Can't convert null to AnimationCurve.");
			}
			if (v.GetType() == typeof(AnimationCurve))
			{
				this.storedValue = (AnimationCurve)v;
				return;
			}
			throw new Exception("Can't convert " + v.GetType().Name + " to AnimationCurve.");
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedValue = new AnimationCurve();
		}

		public AnimationCurve GetValue()
		{
			return this.storedValue;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
