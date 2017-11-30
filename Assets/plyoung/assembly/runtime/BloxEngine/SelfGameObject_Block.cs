using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Values/Self: GameObject", BloxBlockType.Value, ReturnType = typeof(GameObject), Order = 0)]
	public class SelfGameObject_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			return base.owningEvent.container.gameObject;
		}
	}
}
