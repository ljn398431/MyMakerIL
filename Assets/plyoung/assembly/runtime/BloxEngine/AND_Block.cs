using System;

namespace BloxEngine
{
	[BloxBlock("Comparison/a AND b", BloxBlockType.Value, Order = 100, OverrideRenderFields = 4, ReturnType = typeof(bool), ParamNames = new string[]
	{
		"!a",
		"!b",
		"and"
	}, ParamTypes = new Type[]
	{
		typeof(object),
		typeof(object),
		null
	})]
	public class AND_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			try
			{
				bool num = base.paramBlocks[0] != null && (bool)base.paramBlocks[0].Run();
				bool flag = base.paramBlocks[1] != null && (bool)base.paramBlocks[1].Run();
				return num & flag;
			}
			catch (Exception ex)
			{
				base.LogError("Boolean values expected. " + ex.Message, null);
				return false;
			}
		}
	}
}
