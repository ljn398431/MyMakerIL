using System;
using System.Collections;
using System.Collections.Generic;

namespace BloxEngine
{
	[BloxBlock("Common/List/Remove", BloxBlockType.Action, Order = 200, ContextType = typeof(List<object>), ParamNames = new string[]
	{
		"!value"
	}, ParamTypes = new Type[]
	{
		typeof(object)
	})]
	public class List_Remove_Block : BloxBlock
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
			IList list = obj as IList;
			if (list != null)
			{
				BloxBlock obj2 = base.paramBlocks[0];
				object value = (obj2 != null) ? obj2.Run() : null;
				BloxUtil.ListRemove(list, value);
			}
			else
			{
				base.LogError("The context must be a List but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			}
			return null;
		}
	}
}
