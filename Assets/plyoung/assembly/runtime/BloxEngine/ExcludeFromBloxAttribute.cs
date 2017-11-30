using System;

namespace BloxEngine
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public class ExcludeFromBloxAttribute : Attribute
	{
		public bool ExceptForSpecifiedMembers;
	}
}
