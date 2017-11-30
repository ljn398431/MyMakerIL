using System;

namespace BloxEngine
{
	[BloxBlock("Flow/ElseIF", BloxBlockType.Container, Order = 302, ParamNames = new string[]
	{
		"!condition"
	}, ParamTypes = new Type[]
	{
		typeof(bool)
	}, ParamEmptyVal = new string[]
	{
		"-no-condition-"
	})]
	public class ElseIF_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			if (base.prevBlock != null)
			{
				if (base.prevBlock.GetType() == typeof(IF_Block))
					return;
				if (base.prevBlock.GetType() == typeof(ElseIF_Block))
					return;
			}
			base.LogError("The ElseIF Block must always follow on an IF Block or ElseIF Block.", null);
		}

		protected override object RunBlock()
		{
			bool flag = false;
			try
			{
				flag = (bool)base.prevBlock.returnValue;
				base.returnValue = flag;
				if (flag)
				{
					return null;
				}
			}
			catch (Exception e)
			{
				base.LogError("The previous Block did not return a Boolean value.", e);
				base.returnValue = false;
				return null;
			}
			try
			{
				flag = (base.paramBlocks[0] == null || (bool)base.paramBlocks[0].Run());
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
