using System;

namespace BloxEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class BloxBlockDrawerAttribute : Attribute
	{
		public Type TargetType;

		public BloxBlockDrawerAttribute(Type targetType)
		{
			this.TargetType = targetType;
		}
	}
}
