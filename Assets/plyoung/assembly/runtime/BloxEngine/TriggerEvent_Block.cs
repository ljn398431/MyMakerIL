using System;
using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Flow/Trigger Event", BloxBlockType.Action, Order = 300, ParamNames = new string[]
	{
		"!name",
		"in",
		"after"
	}, ParamTypes = new Type[]
	{
		typeof(string),
		typeof(GameObject),
		typeof(float)
	}, ParamEmptyVal = new string[]
	{
		"-invalid-",
		"self: GameObject",
		"0"
	})]
	public class TriggerEvent_Block : BloxBlock
	{
		private Type[] paramTypes;

		protected override void InitBlock()
		{
			if (base.paramBlocks[0] == null)
			{
				base.LogError("No target Event name specified", null);
			}
		}

		protected override object RunBlock()
		{
			BloxContainer bloxContainer = (base.paramBlocks[1] == null) ? base.owningEvent.container : BloxUtil.GetComponent<BloxContainer>(base.paramBlocks[1].Run());
			if ((UnityEngine.Object)bloxContainer == (UnityEngine.Object)null)
			{
				base.LogError("Could not find a Blox Container component on the target GameObject", null);
				return null;
			}
			string text = (string)base.paramBlocks[0].Run();
			if (string.IsNullOrEmpty(text))
			{
				base.LogError("The target Event name is invalid", null);
				return null;
			}
			float afterSeconds = (float)((base.paramBlocks[2] == null) ? 0.0 : ((float)base.paramBlocks[2].Run()));
			if (base.paramBlocks.Length > 3)
			{
				BloxEventArg[] array = new BloxEventArg[base.paramBlocks.Length - 3];
				for (int i = 3; i < base.paramBlocks.Length; i++)
				{
					BloxBlock obj = base.paramBlocks[i];
					object val = (obj != null) ? obj.Run() : null;
					array[i - 3] = new BloxEventArg("param" + (i - 3).ToString(), val);
				}
				bloxContainer.TriggerEvent(text, afterSeconds, array);
			}
			else
			{
				bloxContainer.TriggerEvent(text, afterSeconds);
			}
			return null;
		}

		public override Type[] ParamTypes()
		{
			if (this.paramTypes == null || this.paramTypes.Length != base.paramBlocks.Length)
			{
				this.paramTypes = new Type[base.paramBlocks.Length];
				this.paramTypes[0] = typeof(string);
				this.paramTypes[1] = typeof(GameObject);
				this.paramTypes[2] = typeof(float);
				for (int i = 3; i < this.paramTypes.Length; i++)
				{
					this.paramTypes[i] = typeof(object);
				}
			}
			return this.paramTypes;
		}
	}
}
