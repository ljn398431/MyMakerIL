using System;

namespace BloxEngine
{
	[BloxBlock("Common/Array/Sort", BloxBlockType.Action, Order = 200, ContextType = typeof(Array), ReturnType = typeof(Array))]
	public class Array_Sort_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			if (base.contextBlock == null)
			{
				base.LogError("The context should be set.", null);
			}
			else if (base.owningBlock == null)
			{
				if (base.contextBlock.mi != null)
				{
					if (!base.contextBlock.mi.CanSetValue)
					{
						base.LogError("The Context is invalid since the provided value can't be changed/ updated.", null);
					}
				}
				else if (base.contextBlock.GetType() != typeof(Variable_Block))
				{
					base.LogError("The Context is invalid. Using the Block with this context will have no effect. You are passing a value which will never be accessed again.", null);
				}
			}
		}

		protected override object RunBlock()
		{
			object obj = base.contextBlock.Run();
			Array array = obj as Array;
			if (array != null)
			{
				if (array.Length > 0)
				{
					if (base.owningBlock == null)
					{
						BloxUtil.ArraySort(array);
						return null;
					}
					return BloxUtil.ArraySortRes(array);
				}
				if (base.owningBlock != null)
				{
					return array;
				}
			}
			else
			{
				base.LogError("The context must be an Array but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			}
			return null;
		}
	}
}
