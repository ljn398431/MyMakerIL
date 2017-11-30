using BloxEngine;
using System;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class SkillDef : GroupedDataItem
	{
		public float activationCost;

		public SkillLogic skillLogicDef;
	}
}
