using BloxEngine.Variables;
using System;
using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Flow/Loop", BloxBlockType.Container, Order = 310, IsLoopBlock = true, ParamNames = new string[]
	{
		"with",
		"from",
		"while less than"
	}, ParamTypes = new Type[]
	{
		typeof(plyVar),
		typeof(int),
		typeof(int)
	}, ParamEmptyVal = new string[]
	{
		"-no-var-",
		"0",
		"0"
	})]
	public class Loop_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			if (base.paramBlocks[2] == null)
			{
				base.LogError("You should specify a maximum number of iterations for the loop (the last field must be set).", null);
			}
		}

		protected override object RunBlock()
		{
			int num = 0;
			int num2 = 0;
			try
			{
				if (base.paramBlocks[1] != null)
				{
					num = (int)base.paramBlocks[1].Run();
				}
				if (base.paramBlocks[2] != null)
				{
					num2 = (int)base.paramBlocks[2].Run();
				}
			}
			catch (InvalidCastException)
			{
				base.LogError("An Integer was expected in [from] and [while less].", null);
				return null;
			}
			catch (Exception ex2)
			{
				base.LogError(ex2.Message, null);
				return null;
			}
			if (num > num2)
			{
				base.LogError("The start value should be smaller than the end value", null);
				return null;
			}
			plyVar plyVar = null;
			try
			{
				if (base.paramBlocks[0] != null)
				{
					plyVar = ((Variable_Block)base.paramBlocks[0]).RunAndGetVariable(typeof(int));
					if (plyVar != null && plyVar.ValueHandler != null && plyVar.variableType == typeof(int))
					{
						goto end_IL_0081;
					}
					base.LogError("The Variable used in [with] must be of Integer type.", null);
					return null;
				}
				end_IL_0081:;
			}
			catch (InvalidCastException)
			{
				base.LogError("A Variable was expected in [with].", null);
				return null;
			}
			catch (Exception ex4)
			{
				base.LogError(ex4.Message, null);
				return null;
			}
			int num3 = 0;
			int num4 = num;
			while (num4 != num2)
			{
				if (plyVar != null)
				{
					plyVar.SetValue(num4);
				}
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
				num4++;
				num3++;
				if (num3 >= BloxGlobal.Instance.deadlockDetect)
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
