using System;

namespace BloxEngine
{
	[BloxBlock("Comparison/a != b", BloxBlockType.Value, Order = 100, OverrideRenderFields = 4, ReturnType = typeof(bool), ParamNames = new string[]
	{
		"!a",
		"!b",
		"!!="
	}, ParamTypes = new Type[]
	{
		typeof(object),
		typeof(object),
		null
	})]
	public class NEQ_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			object obj = (base.paramBlocks[0] == null) ? base.DefaultParamVals[0] : base.paramBlocks[0].Run();
			object obj2 = (base.paramBlocks[1] == null) ? base.DefaultParamVals[1] : base.paramBlocks[1].Run();
			try
			{
				return !BloxUtil.IsEqual(obj, obj2, true);
			}
			catch (Exception ex)
			{
				base.LogError("The values [" + obj + "] and [" + obj2 + "] can't be compared. " + ex.Message, null);
				return false;
			}
		}
	}
}
