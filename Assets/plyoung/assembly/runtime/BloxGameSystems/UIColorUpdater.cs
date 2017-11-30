using BloxEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Color")]
	public class UIColorUpdater : UIElementUpdater
	{
		[SerializeField]
		private Graphic target;

		protected void Reset()
		{
			this.target = base.GetComponent<Graphic>();
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.GetComponent<Graphic>();
				if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
				{
					Debug.Log("[UIColorUpdater] Could not find any UI component on the GameObject.", base.gameObject);
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
					this.target.color = (Color)base.getter.Value;
				}
				catch (Exception exception)
				{
					Debug.LogError("[UIColorUpdater] The managed property's getter must return a color value.", base.gameObject);
					Debug.LogException(exception);
				}
			}
		}
	}
}
