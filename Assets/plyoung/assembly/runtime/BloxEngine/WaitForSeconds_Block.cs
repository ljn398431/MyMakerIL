using System;
using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Flow/Wait for Seconds", BloxBlockType.Action, Order = 350, Style = "orange", IsYieldBlock = true, ParamNames = new string[]
	{
		"!seconds"
	}, ParamTypes = new Type[]
	{
		typeof(float)
	})]
	public class WaitForSeconds_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			base.selfOrChildCanYield = true;
		}

		protected override object RunBlock()
		{
			try
			{
				float seconds = (float)((base.paramBlocks[0] == null) ? 0.0 : ((float)base.paramBlocks[0].Run()));
				base.flowSig = BloxFlowSignal.Wait;
				base.yieldInstruction = new WaitForSeconds(seconds);
			}
			catch (Exception ex)
			{
				base.flowSig = BloxFlowSignal.Stop;
				base.yieldInstruction = null;
				base.LogError(ex.Message, null);
			}
			return null;
		}
	}
}
