using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[HelpURL("https://plyoung.github.io/blox-bgssettings.html")]
	public class BGSSettings : ScriptableObject
	{
		[HideInInspector]
		public bool autoLoadBootstrapOnUnityPlay = true;

		private static BGSSettings _instance;

		public static BGSSettings Instance
		{
			get
			{
				return BGSSettings._instance ?? (BGSSettings._instance = Resources.Load<BGSSettings>("Blox/BGSSettings"));
			}
		}
	}
}
