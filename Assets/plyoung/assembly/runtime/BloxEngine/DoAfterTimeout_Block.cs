using BloxEngine.Variables;
using System;
using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Flow/Do After Timeout", BloxBlockType.Container, Order = 301, ParamNames = new string[]
	{
		"with"
	}, ParamTypes = new Type[]
	{
		typeof(Variable_Block)
	}, ParamEmptyVal = new string[]
	{
		"-variable-required-"
	})]
	public class DoAfterTimeout_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			Variable_Block variable_Block = base.paramBlocks[0] as Variable_Block;
			if (variable_Block == null)
			{
				base.LogError("You must specify the variable to use for count-down, via the Variable block.", null);
			}
			else if (variable_Block.varType == plyVariablesType.Event)
			{
				base.LogError("Event Variables are not allowed in this Block.", null);
			}
		}

		protected override object RunBlock()
		{
			plyVar plyVar = ((Variable_Block)base.paramBlocks[0]).RunAndGetVariable(null);
			if (plyVar == null)
			{
				return null;
			}
			if (plyVar.variableType != typeof(float))
			{
				base.LogError("The variable must be a Float type variable.", null);
				return null;
			}
			float num = (float)plyVar.GetValue();
			if (num <= 0.0)
			{
				return null;
			}
			num -= Time.deltaTime;
			if (num <= 0.0)
			{
				num = 0f;
				plyVar.SetValue(num);
				base.RunChildBlocks();
			}
			else
			{
				plyVar.SetValue(num);
			}
			return null;
		}
	}
}
