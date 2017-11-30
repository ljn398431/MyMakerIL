using System;

namespace BloxEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class BloxBlockAttribute : Attribute
	{
		public string Ident;

		public BloxBlockType BlockType;

		public int Order = 99999;

		public Type ReturnType;

		public string IconName;

		public string Style;

		public Type ContextType;

		public Type[] ParamTypes;

		public string[] ParamNames;

		public string[] ParamEmptyVal;

		public int OverrideRenderFields;

		public bool IsYieldBlock;

		public bool IsLoopBlock;

		public BloxBlockAttribute(string ident, BloxBlockType blockType)
		{
			this.Ident = ident;
			this.BlockType = blockType;
		}
	}
}
