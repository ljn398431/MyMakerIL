using plyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	public class GraphMappedValuesProvider : DataProvider
	{
		public DataBinding param1 = new DataBinding();

		public plyGraphMappedValues curve = new plyGraphMappedValues();

		public bool yIsInput = true;

		private float _val;

		public override Type DataType()
		{
			return typeof(float);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			GraphMappedValuesProvider graphMappedValuesProvider = ScriptableObject.CreateInstance<GraphMappedValuesProvider>();
			graphMappedValuesProvider.hideFlags = HideFlags.HideAndDontSave;
			graphMappedValuesProvider.param1 = this.param1.Copy();
			graphMappedValuesProvider.curve = this.curve;
			graphMappedValuesProvider.yIsInput = this.yIsInput;
			return graphMappedValuesProvider;
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
			float num = BloxUtil.AsFloat(this.param1.Value);
			this._val = (float)(this.yIsInput ? this.curve.GetXValue(num) : this.curve.GetYValue(Mathf.RoundToInt(num)));
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
