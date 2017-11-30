using System;
using System.Collections.Generic;

namespace BloxEngine
{
	[Serializable]
	[ExcludeFromBlox]
	public class BloxEventData
	{
		public List<BloxBlockData> blocks = new List<BloxBlockData>();

		public BloxEventData Copy()
		{
			BloxEventData bloxEventData = new BloxEventData();
			for (int i = 0; i < this.blocks.Count; i++)
			{
				bloxEventData.blocks.Add(this.blocks[i].Copy());
			}
			return bloxEventData;
		}
	}
}
