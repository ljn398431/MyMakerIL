using BloxEngine.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace BloxEngine
{
	[ExcludeFromBlox]
	[RequireComponent(typeof(Button))]
	[AddComponentMenu("Blox/Helpers/Trigger Blox Event on Click")]
	[HelpURL("https://plyoung.github.io/blox-components.html")]
	public class TriggerBloxEventOnClick : plyVariablesBehaviour
	{
		[SerializeField]
		public Button button;

		[SerializeField]
		public BloxContainer container;

		[SerializeField]
		public string eventName;

		protected void Reset()
		{
			this.button = base.GetComponent<Button>();
		}

		protected override void Awake()
		{
			base.Awake();
			if ((Object)this.button == (Object)null || (Object)this.container == (Object)null || string.IsNullOrEmpty(this.eventName))
			{
				base.enabled = false;
			}
			else
			{
				this.button.onClick.AddListener(this.OnButtonClick);
			}
		}

		private void OnButtonClick()
		{
			if (base.variables == null || base.variables.varDefs == null || base.variables.varDefs.Count == 0)
			{
				this.container.TriggerEvent(this.eventName);
			}
			else
			{
				BloxEventArg[] array = new BloxEventArg[base.variables.varDefs.Count];
				for (int i = 0; i < base.variables.varDefs.Count; i++)
				{
					array[i] = new BloxEventArg(base.variables.varDefs[i].name, base.variables.varDefs[i].GetValue());
				}
				this.container.TriggerEvent(this.eventName, array);
			}
		}
	}
}
