using BloxEngine;
using System;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Active State")]
	public class UIActiveStateUpdater : UIElementUpdater
	{
		[SerializeField]
		private GameObject target;

		protected void Reset()
		{
			this.target = base.gameObject;
		}

		protected override void Awake()
		{
			if ((UnityEngine.Object)this.target == (UnityEngine.Object)null)
			{
				this.target = base.gameObject;
			}
			base.Awake();
		}

		protected override void OnValueChanged()
		{
			if (base.getter != null)
			{
				try
				{
					bool active = (bool)base.getter.Value;
					this.target.SetActive(active);
				}
				catch (Exception exception)
				{
					Debug.LogError("[UIActiveStateUpdater] The managed property's getter must return a Boolean value.", base.gameObject);
					Debug.LogException(exception);
				}
			}
		}
	}
}
