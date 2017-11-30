using BloxEngine;
using BloxEngine.Variables;
using System;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class GroupedDataItem
	{
		public int id;

		public string ident;

		public plyVariables meta = new plyVariables();

		[NonSerialized]
		public int _idx = -1;
	}
}
