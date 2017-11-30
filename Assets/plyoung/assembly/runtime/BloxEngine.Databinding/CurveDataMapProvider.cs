using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	public class CurveDataMapProvider : DataProvider
	{
		public DataBinding param1 = new DataBinding();

		public AnimationCurve curve = new AnimationCurve();

		public RoundingOption roundingOpt;

		private float _val;

		public override Type DataType()
		{
			return typeof(float);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			CurveDataMapProvider curveDataMapProvider = ScriptableObject.CreateInstance<CurveDataMapProvider>();
			curveDataMapProvider.hideFlags = HideFlags.HideAndDontSave;
			curveDataMapProvider.param1 = this.param1.Copy();
			curveDataMapProvider.curve = this.curve;
			curveDataMapProvider.roundingOpt = this.roundingOpt;
			return curveDataMapProvider;
		}

		public override object GetValue()
		{
			if (this.param1.UpdateValue())
			{
				this.UpdateLocalValue();
			}
			return this._val;
		}

		public override void Initialize(Component owner)
		{
			base.Initialize(owner);
			base.RegisterBindWithCallbacks(this.param1);
		}

		protected override void BoundValueChanged()
		{
			this.UpdateLocalValue();
			base.BoundValueChanged();
		}

		private void UpdateLocalValue()
		{
			float time = BloxUtil.AsFloat(this.param1.Value);
			this._val = this.curve.Evaluate(time);
			if (this.roundingOpt != 0)
			{
				if (this.roundingOpt == RoundingOption.Round)
				{
					this._val = Mathf.Round(this._val);
				}
				else if (this.roundingOpt == RoundingOption.Floor)
				{
					this._val = Mathf.Floor(this._val);
				}
				else if (this.roundingOpt == RoundingOption.Ceil)
				{
					this._val = Mathf.Ceil(this._val);
				}
			}
		}

		public override List<DataProvider> GetDataProvidersForDestruction()
		{
			List<DataProvider> list = new List<DataProvider>();
			if (this.param1 != null && base.IsValidForAutoDestroy(this.param1.dataprovider))
			{
				list.Add(this.param1.dataprovider);
				list.AddRange(this.param1.dataprovider.GetDataProvidersForDestruction());
			}
			return list;
		}
	}
}
