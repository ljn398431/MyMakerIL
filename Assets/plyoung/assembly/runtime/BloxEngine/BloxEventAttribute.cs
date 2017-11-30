using System;

namespace BloxEngine
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class BloxEventAttribute : Attribute
	{
		public string Ident;

		public int Order = 99999;

		public string IconName;

		public bool YieldAllowed;

		public BloxEventAttribute(string ident)
		{
			this.Ident = ident;
		}
	}
}
