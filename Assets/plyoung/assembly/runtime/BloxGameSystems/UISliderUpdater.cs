using BloxEngine;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Slider")]
	public class UISliderUpdater : UIElementUpdater
	{
		[SerializeField]
		private Slider target;

		private bool ignore;

		protected void Reset()
		{
			this.target = base.GetComponent<Slider>();
		}

		protected override void Awake()
		{
			if ((Object)this.target == (Object)null)
			{
				this.target = base.GetComponent<Slider>();
				if ((Object)this.target == (Object)null)
				{
					Debug.Log("[UISliderUpdater] Could not find any Slider component on the GameObject.", base.gameObject);
					return;
				}
			}
			this.target.onValueChanged.AddListener(this.OnTargetUIValueChange);
			base.Awake();
		}

		protected override void OnValueChanged()
		{
			if (!((Object)this.target == (Object)null))
			{
				this.ignore = true;
				if (base.getter != null)
				{
					this.target.value = BloxUtil.AsFloat(base.getter.Value);
				}
				this.ignore = false;
			}
		}

		private void OnTargetUIValueChange(float value)
		{
			if (!this.ignore)
			{
				PropertiesManager.SetValue(base.sourceTargetName, value);
			}
		}
	}
}
