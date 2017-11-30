using BloxGameSystems;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[SkillLogicEd(typeof(SkillLogic_BasicProjectile), "Projectile/Basic")]
	public class SkillLogic_BasicProjectileEd : SkillLogicEd
	{
		public override void DrawEditor()
		{
			GUILayout.Label("Projectile/Basic settings here ...");
		}
	}
}
