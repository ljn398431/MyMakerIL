using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/Helpers/Disable On Awake")]
	[HelpURL("https://plyoung.github.io/blox-components.html")]
	public class DisableOnAwake : MonoBehaviour
	{
		protected void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
