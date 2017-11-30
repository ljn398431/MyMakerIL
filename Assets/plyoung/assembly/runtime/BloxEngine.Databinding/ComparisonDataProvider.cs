using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	public class ComparisonDataProvider : DataProvider
	{
		public ComparisonOperation comparisonOpt;

		public DataBinding param1 = new DataBinding();

		public DataBinding param2 = new DataBinding();

		private bool _val;

		public bool Value
		{
			get
			{
				this.GetValue();
				return this._val;
			}
		}

		public override Type DataType()
		{
			return typeof(bool);
		}

		public override DataProvider CreateRuntimeCopy()
		{
			ComparisonDataProvider comparisonDataProvider = ScriptableObject.CreateInstance<ComparisonDataProvider>();
			comparisonDataProvider.hideFlags = HideFlags.HideAndDontSave;
			comparisonDataProvider.comparisonOpt = this.comparisonOpt;
			comparisonDataProvider.param1 = this.param1.Copy();
			comparisonDataProvider.param2 = this.param2.Copy();
			return comparisonDataProvider;
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
			if (this.comparisonOpt == ComparisonOperation.And)
			{
				try
				{
					bool flag = this.param1.Value != null && (bool)Convert.ChangeType(this.param1.Value, typeof(bool));
					if (!flag)
					{
						this._val = false;
					}
					else
					{
						bool flag2 = this.param2.Value != null && (bool)Convert.ChangeType(this.param2.Value, typeof(bool));
						this._val = (flag & flag2);
					}
				}
				catch
				{
					Debug.LogErrorFormat("[Comparison DataProvider] Could not perform 'and' on the values: [{0}] and [{1}]", this.param1.Value, this.param2.Value);
					this._val = false;
				}
			}
			else if (this.comparisonOpt == ComparisonOperation.Or)
			{
				try
				{
					bool flag3 = this.param1.Value != null && (bool)Convert.ChangeType(this.param1.Value, typeof(bool));
					if (flag3)
					{
						this._val = true;
					}
					else
					{
						bool flag4 = this.param2.Value != null && (bool)Convert.ChangeType(this.param2.Value, typeof(bool));
						this._val = (flag3 | flag4);
					}
				}
				catch
				{
					Debug.LogErrorFormat("[Comparison DataProvider] Could not perform 'or' on the values: [{0}] and [{1}]", this.param1.Value, this.param2.Value);
					this._val = false;
				}
			}
			else
			{
				IComparable comparable = (IComparable)this.param1.Value;
				if (comparable == null)
				{
					if (this.comparisonOpt == ComparisonOperation.Equal)
					{
						this._val = (this.param2.Value == null);
					}
					else if (this.comparisonOpt == ComparisonOperation.NotEqual)
					{
						this._val = (this.param2.Value != null);
					}
					else
					{
						this._val = false;
					}
				}
				else
				{
					object obj3 = (this.param2.Value == null) ? null : Convert.ChangeType(this.param2.Value, comparable.GetType());
					int num = comparable.CompareTo(obj3);
					switch (this.comparisonOpt)
					{
					case ComparisonOperation.Equal:
						this._val = (num == 0);
						break;
					case ComparisonOperation.NotEqual:
						this._val = (num != 0);
						break;
					case ComparisonOperation.SmallerThan:
						this._val = (num < 0);
						break;
					case ComparisonOperation.GreaterThan:
						this._val = (num > 0);
						break;
					case ComparisonOperation.SmallerThanOrEqual:
						this._val = (num == 0 || num < 0);
						break;
					case ComparisonOperation.GreaterThanOrEqual:
						this._val = (num == 0 || num > 0);
						break;
					}
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
			if (this.param2 != null && base.IsValidForAutoDestroy(this.param2.dataprovider))
			{
				list.Add(this.param2.dataprovider);
				list.AddRange(this.param2.dataprovider.GetDataProvidersForDestruction());
			}
			return list;
		}
	}
}
