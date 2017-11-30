using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Flow/Wait for EndOfFrame", BloxBlockType.Action, Order = 350, Style = "orange", IsYieldBlock = true)]
	public class WaitForEndOfFrame_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			base.selfOrChildCanYield = true;
		}

		protected override object RunBlock()
		{
			base.flowSig = BloxFlowSignal.Wait;
			base.yieldInstruction = new WaitForEndOfFrame();
			return null;
		}
	}
}
