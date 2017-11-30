using System;

namespace BloxEngine
{
	[BloxBlock("Comparison/a OR b", BloxBlockType.Value, Order = 100, OverrideRenderFields = 4, ReturnType = typeof(bool), ParamNames = new string[]
	{
		"!a",
		"!b",
		"or"
	}, ParamTypes = new Type[]
	{
		typeof(object),
		typeof(object),
		null
	})]
	public class OR_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			try
			{
				bool num = base.paramBlocks[0] != null && (bool)base.paramBlocks[0].Run();
				bool flag = base.paramBlocks[1] != null && (bool)base.paramBlocks[1].Run();
				return num | flag;
			}
			catch (Exception ex)
			{
				base.LogError("Boolean values expected. " + ex.Message, null);
				return false;
			}
		}
	}
}
