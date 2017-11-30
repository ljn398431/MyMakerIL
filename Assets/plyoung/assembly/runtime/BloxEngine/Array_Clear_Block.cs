using System;

namespace BloxEngine
{
	[BloxBlock("Common/Array/Clear", BloxBlockType.Action, Order = 200, ContextType = typeof(Array))]
	public class Array_Clear_Block : BloxBlock
	{
		protected override void InitBlock()
		{
			if (base.contextBlock == null)
			{
				base.LogError("The context should be set.", null);
			}
			else if (base.contextBlock.mi != null)
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

		protected override object RunBlock()
		{
			object obj = base.contextBlock.Run();
			Array array = obj as Array;
			if (array != null)
			{
				Array val = BloxUtil.ArrayClear(array);
				base.contextBlock.UpdateWith(val);
			}
			else
			{
				base.LogError("The context must be an Array but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			}
			return null;
		}
	}
}
