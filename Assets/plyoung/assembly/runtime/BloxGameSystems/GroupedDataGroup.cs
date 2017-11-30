using BloxEngine;
using System;
using System.Collections.Generic;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class GroupedDataGroup<ItemT>
	{
		public int id;

		public string ident;

		public List<ItemT> items = new List<ItemT>();
	}
}
