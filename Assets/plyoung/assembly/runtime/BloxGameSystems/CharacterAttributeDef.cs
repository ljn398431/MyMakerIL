using BloxEngine;
using BloxEngine.Databinding;
using System;

namespace BloxGameSystems
{
	[Serializable]
	[ExcludeFromBlox]
	public class CharacterAttributeDef : GroupedDataItem
	{
		public float initialValue;

		public float initialMaxVal;

		public bool resetValueToMax;

		public DataBinding valueModifier = new DataBinding();

		public DataBinding maxValModifier = new DataBinding();
	}
}
