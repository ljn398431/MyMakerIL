using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Flow/Wait for FixedUpdate", BloxBlockType.Action, Order = 350, Style = "orange", IsYieldBlock = true)]
	public class WaitForFixedUpdate_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			base.selfOrChildCanYield = true;
		}

		protected override object RunBlock()
		{
			base.flowSig = BloxFlowSignal.Wait;
			base.yieldInstruction = new WaitForFixedUpdate();
			return null;
		}
	}
}
