using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/Helpers/Don't Destroy On Load")]
	[HelpURL("https://plyoung.github.io/blox-components.html")]
	public class DontDestroyOnLoad : MonoBehaviour
	{
		protected void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			Object.Destroy(this);
		}
	}
}
