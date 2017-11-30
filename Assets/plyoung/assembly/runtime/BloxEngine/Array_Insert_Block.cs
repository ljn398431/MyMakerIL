using System;

namespace BloxEngine
{
	[BloxBlock("Common/Array/Insert", BloxBlockType.Action, Order = 200, ContextType = typeof(Array), ParamNames = new string[]
	{
		"!value",
		"at index"
	}, ParamTypes = new Type[]
	{
		typeof(object),
		typeof(int)
	})]
	public class Array_Insert_Block : BloxBlock
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
					num = ((base.paramBlocks[1] != null) ? ((int)base.paramBlocks[1].Run()) : 0);
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
				if (num >= 0 && num <= array.Length)
				{
					BloxBlock obj3 = base.paramBlocks[0];
					object value2 = (obj3 != null) ? obj3.Run() : null;
					try
					{
						Array val = BloxUtil.ArrayInsert(array, value2, num);
						base.contextBlock.UpdateWith(val);
					}
					catch (Exception ex)
					{
						base.LogError(ex.Message, null);
					}
					goto IL_0135;
				}
				base.LogError("The index [" + num + "] is out of range. It should be a value between Array Start:[0] and Array Size:[" + array.Length + "]. Remember that arrays start indexing at 0 and not 1.", null);
				return null;
			}
			base.LogError("The context must be an Array but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			goto IL_0135;
			IL_0135:
			return null;
		}
	}
}
