using BloxEngine.Variables;
using System;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	public class ConstantValueProvider : DataProvider
	{
		public plyVar constant;

		private object _val;

		public override Type DataType()
		{
			return this.constant.variableType;
		}

		public override DataProvider CreateRuntimeCopy()
		{
			return this;
		}

		public override object GetValue()
		{
			return this._val;
		}

		public override void OnCreated()
		{
			this.constant = plyVar.Create(typeof(plyVar_Bool));
		}

		public override void Initialize(Component owner)
		{
			base.Initialize(owner);
			this._val = this.constant.GetValue();
			if (!Application.isEditor)
			{
				this.constant = null;
			}
		}

		public override void Deserialize()
		{
			base.Deserialize();
			this.constant.Deserialize();
		}

		public override void Serialize()
		{
			base.Serialize();
			this.constant.Serialize();
		}
	}
}
