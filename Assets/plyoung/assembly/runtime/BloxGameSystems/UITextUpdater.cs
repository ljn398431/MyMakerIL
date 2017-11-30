using BloxEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Text")]
	public class UITextUpdater : UIElementUpdater
	{
		[SerializeField]
		private Text target;

		protected void Reset()
		{
			this.target = base.GetComponent<Text>();
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.GetComponent<Text>();
				if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
				{
					Debug.Log("[UITextUpdater] Could not find any Text component on the GameObject.", base.gameObject);
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
					Text obj = this.target;
					object value = base.getter.Value;
					obj.text = ((value != null) ? value.ToString() : null);
				}
				catch (Exception exception)
				{
					Debug.LogError("[UITextUpdater] The managed property's getter must return a String value.", base.gameObject);
					Debug.LogException(exception);
				}
			}
		}
	}
}
