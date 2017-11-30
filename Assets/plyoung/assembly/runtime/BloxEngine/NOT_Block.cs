using System;

namespace BloxEngine
{
	[BloxBlock("Comparison/Not a", BloxBlockType.Value, Order = 100, OverrideRenderFields = 4, ReturnType = typeof(bool), ParamNames = new string[]
	{
		"!a"
	}, ParamTypes = new Type[]
	{
		typeof(bool)
	})]
	public class NOT_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			if (base.paramBlocks[0] != null)
			{
				try
				{
					return !(bool)base.paramBlocks[0].Run();
				}
				catch (Exception ex)
				{
					base.LogError("Boolean value expected." + ex.Message, null);
					return false;
				}
			}
			return true;
		}
	}
}
