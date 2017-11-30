using BloxEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Image")]
	public class UIImageUpdater : UIElementUpdater
	{
		[SerializeField]
		private Image target;

		protected void Reset()
		{
			this.target = base.GetComponent<Image>();
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.GetComponent<Image>();
				if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
				{
					Debug.Log("[UIImageUpdater] Could not find any Image component on the GameObject.", base.gameObject);
				}
			}
			base.Awake();
		}

		protected override void OnValueChanged()
		{
			if (!((UnityEngine.Object)this.target == (UnityEngine.Object)null) && base.getter != null)
			{
				try
				{
					this.target.sprite = (Sprite)base.getter.Value;
				}
				catch (Exception exception)
				{
					Debug.LogError("[UIImageUpdater] The managed property's getter must return a Sprite value.", base.gameObject);
					Debug.LogException(exception);
				}
			}
		}
	}
}
