using System;

namespace BloxEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class BloxBlockIconAttribute : Attribute
	{
		public string Icon = "";

		public BloxBlockIconAttribute(string icon)
		{
			this.Icon = icon;
		}
	}
}
