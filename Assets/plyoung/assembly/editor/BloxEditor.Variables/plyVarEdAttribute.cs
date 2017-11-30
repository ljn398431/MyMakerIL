using plyLibEditor;
using System;

namespace BloxEditor.Variables
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class plyVarEdAttribute : plyCustomEdAttribute
	{
		public string VarTypeName;

		public int Order = 999;

		public bool UsesAdvancedEditor;

		public plyVarEdAttribute(Type targeType, string varTypeName) : base(targeType)
		{
			this.VarTypeName = varTypeName;
		}
	}
}
