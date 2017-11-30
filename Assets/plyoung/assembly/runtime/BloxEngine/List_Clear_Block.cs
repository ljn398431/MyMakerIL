using System.Collections;
using System.Collections.Generic;

namespace BloxEngine
{
	[BloxBlock("Common/List/Clear", BloxBlockType.Action, Order = 200, ContextType = typeof(List<object>))]
	public class List_Clear_Block : BloxBlock
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
				list.Clear();
			}
			else
			{
				base.LogError("The context must be a List but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			}
			return null;
		}
	}
}
