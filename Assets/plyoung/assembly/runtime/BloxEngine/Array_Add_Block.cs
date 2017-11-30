using System;

namespace BloxEngine
{
	[BloxBlock("Common/Array/Add", BloxBlockType.Action, Order = 200, ContextType = typeof(Array), ParamNames = new string[]
	{
		"!value"
	}, ParamTypes = new Type[]
	{
		typeof(object)
	})]
	public class Array_Add_Block : BloxBlock
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
				BloxBlock obj2 = base.paramBlocks[0];
				object value = (obj2 != null) ? obj2.Run() : null;
				try
				{
					Array val = BloxUtil.ArrayAdd(array, value);
					base.contextBlock.UpdateWith(val);
				}
				catch (Exception ex)
				{
					base.LogError(ex.Message, null);
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
