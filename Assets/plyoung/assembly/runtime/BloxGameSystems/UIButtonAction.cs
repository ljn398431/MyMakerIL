using BloxEngine;
using BloxEngine.Databinding;
using BloxEngine.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/GUI/Updaters/Button Action")]
	[HelpURL("https://plyoung.github.io/blox-ui-updaters.html")]
	public class UIButtonAction : plyVariablesBehaviour
	{
		[SerializeField]
		public Button button;

		[SerializeField]
		public DataBinding databinding = new DataBinding();

		protected void Reset()
		{
			this.button = base.GetComponent<Button>();
		}

		protected override void Awake()
		{
			base.Awake();
			if ((Object)this.button == (Object)null)
			{
				this.button = base.GetComponent<Button>();
				if ((Object)this.button == (Object)null)
				{
					Debug.Log("[UIButtonAction] Could not find any Button component on the GameObject.", base.gameObject);
					return;
				}
			}
			this.button.onClick.AddListener(this.OnButtonClick);
		}

		protected void Start()
		{
			this.databinding.Initialize(null, this);
		}

		private void OnButtonClick()
		{
			DataProvider dataprovider = this.databinding.dataprovider;
			if ((object)dataprovider != null)
			{
				dataprovider.Execute(null);
			}
		}
	}
}
