using BloxGameSystems;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[SkillLogicEd(typeof(SkillLogic_Basic), "Basic")]
	public class SkillLogic_BasicEd : SkillLogicEd
	{
		public override void DrawEditor()
		{
			GUILayout.Label("Basic settings here ...");
		}
	}
}
