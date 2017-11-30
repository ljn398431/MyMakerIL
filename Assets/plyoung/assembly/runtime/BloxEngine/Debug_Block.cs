using System;
using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Common/Debug", BloxBlockType.Action, Order = 200, OverrideRenderFields = 1, Style = "debug")]
	public class Debug_Block : BloxBlock
	{
		public enum DebugBlockLogType
		{
			Log = 0,
			Warning = 1,
			Error = 2
		}

		public string message = "";

		public DebugBlockLogType logType;

		private Type[] paramTypes;

		public override void CopyTo(BloxBlock block)
		{
			base.CopyTo(block);
			Debug_Block obj = block as Debug_Block;
			obj.message = this.message;
			obj.logType = this.logType;
		}

		protected override object RunBlock()
		{
			string arg = this.message;
			if (base.paramBlocks != null)
			{
				for (int i = 0; i < base.paramBlocks.Length; i++)
				{
					arg = arg + ((i == 0) ? " " : ", ") + ((base.paramBlocks[i] == null) ? "null" : base.paramBlocks[i].Run());
				}
			}
			if (this.logType == DebugBlockLogType.Log)
			{
				Debug.Log(arg, base.owningEvent.container.gameObject);
			}
			else if (this.logType == DebugBlockLogType.Warning)
			{
				Debug.LogWarning(arg, base.owningEvent.container.gameObject);
			}
			else
			{
				Debug.LogError(arg, base.owningEvent.container.gameObject);
			}
			return null;
		}

		public override Type[] ParamTypes()
		{
			if (base.paramBlocks == null)
			{
				return null;
			}
			if (this.paramTypes == null || this.paramTypes.Length != base.paramBlocks.Length)
			{
				this.paramTypes = new Type[base.paramBlocks.Length];
				for (int i = 0; i < this.paramTypes.Length; i++)
				{
					this.paramTypes[i] = typeof(object);
				}
			}
			return this.paramTypes;
		}
	}
}
