using BloxEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Toggle")]
	public class UIToggleUpdater : UIElementUpdater
	{
		[SerializeField]
		private Toggle target;

		private bool ignore;

		protected void Reset()
		{
			this.target = base.GetComponent<Toggle>();
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.GetComponent<Toggle>();
				if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
				{
					Debug.Log("[UIToggleUpdater] Could not find any Toggle component on the GameObject.", base.gameObject);
					return;
				}
			}
			this.target.onValueChanged.AddListener(this.OnTargetValueChange);
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
						bool isOn = (bool)base.getter.Value;
						this.target.isOn = isOn;
					}
					catch (Exception exception)
					{
						Debug.LogError("[UIToggleUpdater] The managed property's getter must return a Boolean value.", base.gameObject);
						Debug.LogException(exception);
					}
				}
				this.ignore = false;
			}
		}

		private void OnTargetValueChange(bool value)
		{
			if (!this.ignore)
			{
				PropertiesManager.SetValue(base.sourceTargetName, value);
			}
		}
	}
}
