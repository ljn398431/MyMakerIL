using BloxEngine;
using System;
using UnityEngine;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class CharacterAttributeDefsAsset : GroupedData<CharacterAttributeDefsGroup, CharacterAttributeDef>
	{
		private static CharacterAttributeDefsAsset _instance;

		public static CharacterAttributeDefsAsset Instance
		{
			get
			{
				return CharacterAttributeDefsAsset._instance ?? (CharacterAttributeDefsAsset._instance = Resources.Load<CharacterAttributeDefsAsset>("Blox/Attributes"));
			}
		}
	}
}
