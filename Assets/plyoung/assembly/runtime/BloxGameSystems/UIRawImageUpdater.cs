using BloxEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/RawImage")]
	public class UIRawImageUpdater : UIElementUpdater
	{
		[SerializeField]
		private RawImage target;

		protected void Reset()
		{
			this.target = base.GetComponent<RawImage>();
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.GetComponent<RawImage>();
				if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
				{
					Debug.Log("[UIRawImageUpdater] Could not find any RawImage component on the GameObject.", base.gameObject);
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
					this.target.texture = (Texture)base.getter.Value;
				}
				catch (Exception exception)
				{
					Debug.LogError("[UIRawImageUpdater] The managed property's getter must return a Sprite value.", base.gameObject);
					Debug.LogException(exception);
				}
			}
		}
	}
}
