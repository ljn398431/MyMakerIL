using BloxEngine;
using System;
using UnityEngine;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class SkillDefsAsset : GroupedData<SkillDefsGroup, SkillDef>
	{
		private static SkillDefsAsset _instance;

		public static SkillDefsAsset Instance
		{
			get
			{
				return SkillDefsAsset._instance ?? (SkillDefsAsset._instance = Resources.Load<SkillDefsAsset>("Blox/Skills"));
			}
		}
	}
}
