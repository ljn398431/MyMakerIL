using System;
using UnityEngine;

namespace BloxGameSystems
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class CharacterAttributeAttribute : PropertyAttribute
	{
		public string Label;

		public CharacterAttributeAttribute(string label)
		{
			this.Label = label;
		}
	}
}
