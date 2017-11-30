using System;

namespace BloxEngine
{
	[BloxBlock("Common/Array/Remove at", BloxBlockType.Action, Order = 200, ContextType = typeof(Array), ParamNames = new string[]
	{
		"index"
	}, ParamTypes = new Type[]
	{
		typeof(int)
	})]
	public class Array_RemoveAt_Block : BloxBlock
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
				if (array.Length == 0)
				{
					base.LogError("There are no values in the list.", null);
					return null;
				}
				if (num >= 0 && num < array.Length)
				{
					Array val = BloxUtil.ArrayRemoveAt(array, num);
					base.contextBlock.UpdateWith(val);
					goto IL_0121;
				}
				base.LogError("The index [" + num + "] is out of range. It should be a value between Array Start:[0] and Array Size:[" + array.Length + "]. Remember that arrays start indexing at 0 and not 1.", null);
				return null;
			}
			base.LogError("The context must be an Array but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			goto IL_0121;
			IL_0121:
			return null;
		}
	}
}
