using BloxGameSystems;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[SkillLogicEd(typeof(SkillLogic_ChainProjectile), "Projectile/Chain")]
	public class SkillLogic_ChainProjectileEd : SkillLogicEd
	{
		public override void DrawEditor()
		{
			GUILayout.Label("Projectile/Chain settings here ...");
		}
	}
}
