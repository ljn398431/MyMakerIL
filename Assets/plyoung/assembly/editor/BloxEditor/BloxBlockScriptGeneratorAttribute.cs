using plyLibEditor;
using System;

namespace BloxEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class BloxBlockScriptGeneratorAttribute : plyCustomEdAttribute
	{
		public BloxBlockScriptGeneratorAttribute(Type targetType) : base(targetType)
		{
		}
	}
}
