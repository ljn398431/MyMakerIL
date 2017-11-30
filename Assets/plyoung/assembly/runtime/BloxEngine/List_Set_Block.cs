using System;
using System.Collections;
using System.Collections.Generic;

namespace BloxEngine
{
	[BloxBlock("Common/List/Set Value", BloxBlockType.Action, Order = 200, ContextType = typeof(List<object>), ParamNames = new string[]
	{
		"at index",
		"to"
	}, ParamTypes = new Type[]
	{
		typeof(int),
		typeof(object)
	})]
	public class List_Set_Block : BloxBlock
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
				int num = 0;
				try
				{
					num = ((base.paramBlocks[0] != null) ? ((int)base.paramBlocks[0].Run()) : 0);
				}
				catch (Exception e)
				{
					object value = base.paramBlocks[0].Run();
					object obj2 = null;
					if (BloxUtil.TryConvert(value, typeof(int), out obj2))
					{
						num = (int)obj2;
						goto end_IL_003d;
					}
					base.LogError("The index is invalid. Expected Integer value.", e);
					return null;
					end_IL_003d:;
				}
				if (list.Count == 0)
				{
					base.LogError("There are no values in the list.", null);
					return null;
				}
				if (num >= 0 && num < list.Count)
				{
					BloxBlock obj3 = base.paramBlocks[1];
					object value2 = (obj3 != null) ? obj3.Run() : null;
					try
					{
						BloxUtil.ListSet(list, value2, num);
					}
					catch (Exception ex)
					{
						base.LogError(ex.Message, null);
					}
					goto IL_013c;
				}
				base.LogError("The index [" + num + "] is out of range. It should be a value between List Start:[0] and List Size:[" + list.Count + "]. Remember that lists start indexing at 0 and not 1.", null);
				return null;
			}
			base.LogError("The context must be a List but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			goto IL_013c;
			IL_013c:
			return null;
		}
	}
}
