using System;

namespace BloxEngine
{
	[BloxBlock("Values/Type", BloxBlockType.Value, ReturnType = typeof(Type), Order = 0, OverrideRenderFields = 2)]
	public class Type_Block : BloxBlock
	{
		public string typeName = "Transform";

		private Type t;

		protected override void InitBlock()
		{
			if (string.IsNullOrEmpty(this.typeName))
			{
				base.LogError("No type name given.", null);
			}
			else
			{
				this.t = BloxUtil.FindType(this.typeName);
				if (this.t == null)
				{
					base.LogError("The type could not be found: " + this.typeName, null);
				}
			}
		}

		protected override object RunBlock()
		{
			return this.t;
		}

		public override void CopyTo(BloxBlock block)
		{
			base.CopyTo(block);
			((Type_Block)block).typeName = this.typeName;
		}
	}
}
