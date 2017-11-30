using System;

namespace BloxEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SuppressBaseMembersInBloxAttribute : Attribute
	{
	}
}
