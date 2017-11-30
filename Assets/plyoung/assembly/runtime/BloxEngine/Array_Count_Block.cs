using System;

namespace BloxEngine
{
	[BloxBlock("Common/Array/Count", BloxBlockType.Value, Order = 200, ContextType = typeof(Array), ReturnType = typeof(int))]
	public class Array_Count_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			if (base.contextBlock == null)
			{
				base.LogError("The context should be set.", null);
			}
		}

		protected override object RunBlock()
		{
			object obj = base.contextBlock.Run();
			Array array = obj as Array;
			if (array != null)
			{
				return array.Length;
			}
			base.LogError("The context must be an Array but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			return 0;
		}
	}
}
