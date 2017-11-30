using System;

namespace BloxEngine
{
	[BloxBlock("Maths/a × b [multiplication]", BloxBlockType.Value, Order = 500, OverrideRenderFields = 4, ReturnType = typeof(object), ParamNames = new string[]
	{
		"!a",
		"!b",
		"×"
	}, ParamTypes = new Type[]
	{
		typeof(object),
		typeof(object),
		null
	})]
	public class MUL_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			object a = (base.paramBlocks[0] == null) ? null : base.paramBlocks[0].Run();
			object b = (base.paramBlocks[1] == null) ? null : base.paramBlocks[1].Run();
			try
			{
				return BloxMathsUtil.Multiply(a, b);
			}
			catch (Exception ex)
			{
				base.LogError(ex.Message, null);
				return null;
			}
		}
	}
}
