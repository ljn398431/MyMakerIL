using BloxEngine;
using BloxEngine.Databinding;
using System;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	public class CharacterAttribute
	{
		public CharacterAttributeDef def;

		private float _value;

		private float _max;

		private DataBinding valueModifier;

		private DataBinding maxValModifier;

		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (this._value != value)
				{
					this._value = Mathf.Clamp(value, 0f, this._max);
					Action obj = this.onValueChanged;
					if (obj != null)
					{
						obj();
					}
				}
			}
		}

		public float MaxValue
		{
			get
			{
				return this._max;
			}
			set
			{
				if (this._max != value)
				{
					this._max = Mathf.Max(0f, value);
					Action obj = this.onMaxValChanged;
					if (obj != null)
					{
						obj();
					}
					if (this.def.resetValueToMax)
					{
						this.Value = this._max;
					}
				}
			}
		}

		public event Action onValueChanged;

		public event Action onMaxValChanged;

		public CharacterAttribute(CharacterAttributeDef def)
		{
			this.def = def;
		}

		public CharacterAttribute(int id)
		{
			this.def = CharacterAttributeDefsAsset.Instance.GetItem(id);
		}

		public CharacterAttribute(string ident)
		{
			this.def = CharacterAttributeDefsAsset.Instance.GetItem(ident);
		}

		public void Initialize(ICharacterAttributesOwner owner)
		{
			Component owner2 = (Component)owner;
			this.Value = this.def.initialValue;
			this.valueModifier = this.def.valueModifier.Copy();
			this.valueModifier.Initialize(this.OnValueModifierChange, owner2);
			this.MaxValue = this.def.initialMaxVal;
			this.maxValModifier = this.def.maxValModifier.Copy();
			this.valueModifier.Initialize(this.OnMaxValModifierChange, owner2);
		}

		private void OnValueModifierChange()
		{
			this.Value = (float)this.valueModifier.Value;
		}

		private void OnMaxValModifierChange()
		{
			this.MaxValue = (float)this.valueModifier.Value;
		}
	}
}
