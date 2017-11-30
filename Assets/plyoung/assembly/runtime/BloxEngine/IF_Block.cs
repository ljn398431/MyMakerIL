using System;

namespace BloxEngine
{
	[BloxBlock("Flow/IF", BloxBlockType.Container, Order = 301, ParamNames = new string[]
	{
		"!condition"
	}, ParamTypes = new Type[]
	{
		typeof(bool)
	}, ParamEmptyVal = new string[]
	{
		"-condition-required-"
	})]
	public class IF_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			if (base.paramBlocks[0] == null)
			{
				base.LogError("Condition required.", null);
			}
		}

		protected override object RunBlock()
		{
			bool flag = false;
			try
			{
				flag = (bool)base.paramBlocks[0].Run();
				base.returnValue = flag;
			}
			catch (InvalidCastException)
			{
				base.LogError("A Boolean value was expected in [condition].", null);
				base.returnValue = false;
				return null;
			}
			catch (Exception ex2)
			{
				base.LogError(ex2.Message, null);
				base.returnValue = false;
				return null;
			}
			if (flag && base.firstChild != null)
			{
				base.RunChildBlocks();
			}
			return null;
		}
	}
}
