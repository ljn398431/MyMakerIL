using System;
using UnityEngine;

namespace BloxGameSystems
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class CharacterAttributeDefLinkAttribute : PropertyAttribute
	{
		public string Header;
	}
}
