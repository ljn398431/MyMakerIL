using System;
using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Flow/While", BloxBlockType.Container, Order = 310, IsLoopBlock = true, ParamNames = new string[]
	{
		"!condition"
	}, ParamTypes = new Type[]
	{
		typeof(bool)
	}, ParamEmptyVal = new string[]
	{
		"-condition-required-"
	})]
	public class While_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			if (base.paramBlocks[0] == null)
			{
				base.LogError("The condition must be set.", null);
			}
		}

		protected override object RunBlock()
		{
			int num = 0;
			bool flag = false;
			try
			{
				flag = (bool)base.paramBlocks[0].Run();
			}
			catch (InvalidCastException)
			{
				base.LogError("A Boolean value was expected in [condition].", null);
				return null;
			}
			catch (Exception ex2)
			{
				base.LogError(ex2.Message, null);
				return null;
			}
			while (flag)
			{
				base.RunChildBlocks();
				if (base.flowSig == BloxFlowSignal.Break)
				{
					base.flowSig = BloxFlowSignal.None;
					break;
				}
				if (base.flowSig == BloxFlowSignal.Stop)
					break;
				if (base.flowSig == BloxFlowSignal.Continue)
				{
					base.flowSig = BloxFlowSignal.None;
				}
				try
				{
					flag = (bool)base.paramBlocks[0].Run();
				}
				catch (InvalidCastException)
				{
					base.LogError("A Boolean value was expected in [condition].", null);
					return null;
				}
				catch (Exception ex4)
				{
					base.LogError(ex4.Message, null);
					return null;
				}
				num++;
				if (num >= BloxGlobal.Instance.deadlockDetect)
				{
					Debug.LogErrorFormat(base.owningEvent.container.gameObject, "Deadlock detected in Loop in Event [{0}:{1}]. Forcing break.", base.owningEvent.container.gameObject.name, base.owningEvent.screenName);
					base.flowSig = BloxFlowSignal.Break;
					break;
				}
			}
			return null;
		}
	}
}
