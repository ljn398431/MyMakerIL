using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	public class MathsDataProvider : DataProvider
	{
		public MathsOperation opt;

		public DataBinding param1 = new DataBinding();

		public DataBinding param2 = new DataBinding();

		private float _val;

		public override Type DataType()
		{
			return typeof(float);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			MathsDataProvider mathsDataProvider = ScriptableObject.CreateInstance<MathsDataProvider>();
			mathsDataProvider.hideFlags = HideFlags.HideAndDontSave;
			mathsDataProvider.opt = this.opt;
			mathsDataProvider.param1 = this.param1.Copy();
			mathsDataProvider.param2 = this.param2.Copy();
			return mathsDataProvider;
		}

		public override object GetValue()
		{
			if (this.param1.UpdateValue() || this.param2.UpdateValue())
			{
				this.UpdateLocalValue();
			}
			return this._val;
		}

		public override void Initialize(Component owner)
		{
			base.Initialize(owner);
			base.RegisterBindWithCallbacks(this.param1);
			base.RegisterBindWithCallbacks(this.param2);
		}

		protected override void BoundValueChanged()
		{
			this.UpdateLocalValue();
			base.BoundValueChanged();
		}

		private void UpdateLocalValue()
		{
			object value = this.param1.Value;
			object value2 = this.param2.Value;
			object o = null;
			try
			{
				switch (this.opt)
				{
				case MathsOperation.Add:
					o = BloxMathsUtil.Add(value, value2);
					break;
				case MathsOperation.Subtract:
					o = BloxMathsUtil.Subtract(value, value2);
					break;
				case MathsOperation.Multiply:
					o = BloxMathsUtil.Multiply(value, value2);
					break;
				case MathsOperation.Divide:
					o = BloxMathsUtil.Divide(value, value2);
					break;
				case MathsOperation.Modulo:
					o = BloxMathsUtil.Modulo(value, value2);
					break;
				}
				this._val = BloxUtil.AsFloat(o);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
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
			if (this.param2 != null && base.IsValidForAutoDestroy(this.param2.dataprovider))
			{
				list.Add(this.param2.dataprovider);
				list.AddRange(this.param2.dataprovider.GetDataProvidersForDestruction());
			}
			return list;
		}
	}
}
