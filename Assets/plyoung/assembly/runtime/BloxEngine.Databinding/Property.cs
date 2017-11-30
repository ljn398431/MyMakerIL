using System;

namespace BloxEngine.Databinding
{
	/// <summary> A property has a value and will trigger an event when that value changes </summary>
	[ExcludeFromBlox]
	public class Property
	{
		protected object _valO;

		public object Value
		{
			get
			{
				return this._valO;
			}
			set
			{
				if (!object.Equals(this._valO, value))
				{
					this._valO = value;
					this.ValueChanged();
				}
			}
		}

		public event Action<object> onValueChanged;

		protected void ValueChanged()
		{
			Action<object> obj = this.onValueChanged;
			if (obj != null)
			{
				obj(this._valO);
			}
		}

		public virtual Type ValueType()
		{
			object valO = this._valO;
			if (valO == null)
			{
				return null;
			}
			return valO.GetType();
		}
	}
	/// <summary> A property has a value and will trigger an event when that value changes </summary>
	public class Property<T> : Property
	{
		private T _valT;

		public new T Value
		{
			get
			{
				return this._valT;
			}
			set
			{
				if (!object.Equals(this._valT, value))
				{
					this._valT = value;
					base.Value = value;
				}
			}
		}

		public Property()
		{
			base._valO = (this._valT = (T)BloxMemberInfo.GetDefaultValue(typeof(T)));
		}

		public Property(T value, bool withCallback = false)
		{
			if (withCallback)
			{
				this.Value = value;
			}
			else
			{
				base._valO = (this._valT = value);
			}
		}

		public override Type ValueType()
		{
			return typeof(T);
		}
	}
}
