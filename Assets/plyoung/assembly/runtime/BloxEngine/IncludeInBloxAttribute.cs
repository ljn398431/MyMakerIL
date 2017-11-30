using System;

namespace BloxEngine
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public class IncludeInBloxAttribute : Attribute
	{
	}
}
