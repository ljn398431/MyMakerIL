using BloxEngine;
using BloxEngine.Databinding;
using System;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("")]
	[HelpURL("https://plyoung.github.io/blox-ui-updaters.html")]
	public class UIElementUpdater : MonoBehaviour
	{
		public string sourceTargetName = "";

		[NonSerialized]
		protected DataBinding getter;

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			ManagedProperty property = PropertiesManager.GetProperty(this.sourceTargetName);
			this.getter = ((property != null) ? property.getter : null);
			PropertiesManager.AddValueChangeListener(this.sourceTargetName, this.OnValueChanged);
			if (this.getter != null)
			{
				this.getter.UpdateValue();
				this.OnValueChanged();
			}
		}

		protected virtual void OnDestroy()
		{
			PropertiesManager.RemoveValueChangeListener(this.sourceTargetName, this.OnValueChanged);
		}

		protected virtual void OnEnable()
		{
			if (this.getter != null && this.getter.UpdateValue())
			{
				this.OnValueChanged();
			}
		}

		protected virtual void OnValueChanged()
		{
		}
	}
}
