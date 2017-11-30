using BloxEngine.Variables;
using System;
using UnityEngine;

namespace BloxEngine
{
	[BloxBlock("Common/Variable", BloxBlockType.Action, Order = 200, OverrideRenderFields = 1, ReturnType = typeof(object), ParamNames = new string[]
	{
		"!value",
		"in"
	}, ParamTypes = new Type[]
	{
		typeof(object),
		typeof(GameObject)
	})]
	public class Variable_Block : BloxBlock
	{
		public string varName = "-var-name-";

		public plyVariablesType varType;

		[NonSerialized]
		public plyVar v;

		public override void CopyTo(BloxBlock block)
		{
			base.CopyTo(block);
			Variable_Block obj = block as Variable_Block;
			obj.varName = this.varName;
			obj.varType = this.varType;
		}

		protected override void InitBlock()
		{
			if (this.varType == plyVariablesType.Blox)
			{
				this.v = base.owningEvent.container.FindVariable(base.owningEvent.blox.ident, this.varName);
				if (this.v == null)
				{
					base.LogError("The Blox Variable [" + this.varName + "] does not exist on GameObject: " + base.owningEvent.container.name, null);
				}
			}
			else if (this.varType == plyVariablesType.Global)
			{
				this.v = GlobalVariables.Instance.variables.FindVariable(this.varName);
				if (this.v == null)
				{
					base.LogError("The Global Variable [" + this.varName + "] does not exist on GameObject: " + GlobalVariables.Instance.name, null);
				}
			}
		}

		protected override object RunBlock()
		{
			if (base.owningBlock != null)
			{
				if (this.RunAndGetVariable(null) == null)
				{
					return null;
				}
				return this.v.GetValue();
			}
			try
			{
				BloxBlock obj = base.paramBlocks[0];
				object value = (obj != null) ? obj.Run() : null;
				if (this.RunAndGetVariable(null) == null)
				{
					return null;
				}
				this.v.SetValue(value);
			}
			catch (Exception e)
			{
				base.LogError("Could not set Variable Value: " + this.varName, e);
			}
			return null;
		}

		protected override void UpdateBlockWith(object value)
		{
			if (base.owningBlock != null && this.RunAndGetVariable(null) != null)
			{
				try
				{
					this.v.SetValue(value);
				}
				catch (Exception e)
				{
					base.LogError("Could not set Variable [" + this.varName + "] with Value [" + value + "] during UpdateBlockWith(...)", e);
				}
			}
		}

		public plyVar RunAndGetVariable(Type expectedType = null)
		{
			if (this.varType == plyVariablesType.Event)
			{
				this.v = base.owningEvent.FindVariable(this.varName, expectedType);
				if (this.v == null)
				{
					if (base.owningBlock == null)
					{
						BloxBlock obj = base.paramBlocks[0];
						expectedType = (((obj != null) ? obj.returnType : null) ?? typeof(object));
					}
					else if (base.fieldIdx == -1)
					{
						expectedType = (base.owningBlock.ContextType() ?? typeof(object));
					}
					else
					{
						Type[] array = base.owningBlock.ParamTypes();
						expectedType = ((array == null || base.fieldIdx < 0 || base.fieldIdx >= array.Length) ? typeof(object) : (array[base.fieldIdx] ?? typeof(object)));
					}
					this.v = base.owningEvent.FindVariable(this.varName, expectedType);
					if (this.v == null)
					{
						base.LogError("The Event Variable [" + this.varName + "] does not exist in Event [" + base.owningEvent.screenName + "] on GameObject: " + base.owningEvent.container.name, null);
						return null;
					}
				}
			}
			else if (this.varType != plyVariablesType.Blox)
			{
				if (this.varType == plyVariablesType.Object)
				{
					GameObject gameObject = null;
					object obj2 = (base.paramBlocks[1] == null) ? base.owningEvent.container.gameObject : base.paramBlocks[1].Run();
					if (obj2 != null)
					{
						gameObject = (obj2 as GameObject);
						if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
						{
							Component component = obj2 as Component;
							if ((UnityEngine.Object)component != (UnityEngine.Object)null)
							{
								gameObject = component.gameObject;
							}
						}
					}
					if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
					{
						ObjectVariables component2 = gameObject.GetComponent<ObjectVariables>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							this.v = component2.variables.FindVariable(this.varName);
							if (this.v == null)
							{
								base.LogError("The Object Variable [" + this.varName + "] does not exist on GameObject: " + gameObject.name, null);
								return null;
							}
							goto IL_0265;
						}
						base.LogError("Could not find ObjectVariables component on GameObject [" + gameObject.name + "] for Object Variable: " + this.varName, null);
						return null;
					}
					base.LogError("The 2nd field must be set to a target GameObject that has the Object Variable component on it with the variable named: " + this.varName, null);
					return null;
				}
				if (this.varType != plyVariablesType.Global)
				{
					base.LogError("Invalid Variable Type specified. This should not happen.", null);
					return null;
				}
			}
			goto IL_0265;
			IL_0265:
			return this.v;
		}
	}
}
