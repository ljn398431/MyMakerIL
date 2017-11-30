using System.Collections;
using System.Collections.Generic;

namespace BloxEngine
{
	[BloxBlock("Common/List/Clone", BloxBlockType.Value, Order = 200, ContextType = typeof(List<object>), ReturnType = typeof(List<object>))]
	public class List_Clone_Block : BloxBlock
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
				return BloxUtil.ListClone(list);
			}
			base.LogError("The context must be a List but was [" + ((obj != null) ? obj.GetType() : null) + "]", null);
			return null;
		}
	}
}
