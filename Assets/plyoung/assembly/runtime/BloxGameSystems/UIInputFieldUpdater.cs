using BloxEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Input Field")]
	public class UIInputFieldUpdater : UIElementUpdater
	{
		[SerializeField]
		private InputField target;

		private bool ignore;

		protected void Reset()
		{
			this.target = base.GetComponent<InputField>();
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.GetComponent<InputField>();
				if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
				{
					Debug.Log("[UIInputFieldUpdater] Could not find any InputField component on the GameObject.", base.gameObject);
					return;
				}
			}
			this.target.onValueChanged.AddListener(this.OnTargetUIValueChange);
			base.Awake();
		}

		protected override void OnValueChanged()
		{
			if (!((UnityEngine.Object)this.target == (UnityEngine.Object)null))
			{
				this.ignore = true;
				if (base.getter != null)
				{
					try
					{
						InputField obj = this.target;
						object value = base.getter.Value;
						obj.text = ((value != null) ? value.ToString() : null);
					}
					catch (Exception exception)
					{
						Debug.LogError("[UIInputFieldUpdater] The managed property's getter must return a String value.", base.gameObject);
						Debug.LogException(exception);
					}
				}
				this.ignore = false;
			}
		}

		private void OnTargetUIValueChange(string value)
		{
			if (!this.ignore)
			{
				PropertiesManager.SetValue(base.sourceTargetName, value);
			}
		}
	}
}
