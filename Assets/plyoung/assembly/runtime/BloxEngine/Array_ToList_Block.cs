using System;
using System.Collections.Generic;

namespace BloxEngine
{
	[BloxBlock("Common/Array/to List", BloxBlockType.Value, Order = 200, ContextType = typeof(Array), ReturnType = typeof(List<object>))]
	public class Array_ToList_Block : BloxBlock
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
				return BloxUtil.ArrayToList(array);
			}
			base.LogError("The context must be an Array but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			return null;
		}
	}
}
