using System;

namespace BloxEngine
{
	[BloxBlock("Common/Array/Get Value", BloxBlockType.Value, Order = 200, ContextType = typeof(Array), ReturnType = typeof(object), ParamNames = new string[]
	{
		"at index"
	}, ParamTypes = new Type[]
	{
		typeof(int)
	})]
	public class Array_Get_Block : BloxBlock
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
					base.LogError("There are no values in the Array.", null);
					return null;
				}
				if (num >= 0 && num < array.Length)
				{
					return array.GetValue(num);
				}
				base.LogError("The index [" + num + "] is out of range. It should be a value between Array Start:[0] and Array Size:[" + array.Length + "]. Remember that arrays start indexing at 0 and not 1.", null);
				return null;
			}
			base.LogError("The context must be an Array but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			return null;
		}
	}
}
