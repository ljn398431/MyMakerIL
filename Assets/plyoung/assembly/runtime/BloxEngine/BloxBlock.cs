using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace BloxEngine
{
	public class BloxBlock
	{
		public BloxBlockType blockType;

		public string ident;

		public Type returnType;

		public object returnValue;

		public BloxMemberInfo mi;

		public bool active;

		public Vector2 _ed_viewOffs = Vector2.zero;

		public BloxBlock next;

		public BloxBlock firstChild;

		public BloxBlock contextBlock;

		public BloxBlock[] paramBlocks;

		public BloxEvent owningEvent;

		public BloxBlock owningBlock;

		public BloxBlock parentBlock;

		public BloxBlock prevBlock;

		public bool selfOrChildCanYield;

		public object yieldInstruction;

		public BloxFlowSignal flowSig;

		protected bool isValid = true;

		protected int fieldIdx = -1;

		private Type contextType;

		private Type[] paramTypes;

		private bool hasReferenceTypes;

		private object context;

		private object[] paramValues;

		private object[] _defaultParamVals;

		protected object[] DefaultParamVals
		{
			get
			{
				if (this._defaultParamVals == null)
				{
					this.InitParamDefaults();
				}
				return this._defaultParamVals;
			}
		}

		public virtual void CopyTo(BloxBlock block)
		{
			block.blockType = this.blockType;
			block.ident = this.ident;
			block.returnType = this.returnType;
			block.returnValue = this.returnValue;
			block.mi = this.mi;
			block.active = true;
			block._ed_viewOffs = Vector2.zero;
		}

		public virtual Type[] ParamTypes()
		{
			return this.paramTypes;
		}

		public virtual Type ContextType()
		{
			return this.contextType;
		}

		protected virtual void InitBlock()
		{
		}

		protected virtual object RunBlock()
		{
			if (this.mi != null)
			{
				this.RunContextBlock();
				if (this.mi.MemberType == MemberTypes.Field || this.mi.MemberType == MemberTypes.Property)
				{
					if (this.owningBlock == null)
					{
						this.RunParamBlocks();
						this.mi.SetValue(this.context, this.paramValues[0]);
						BloxBlock obj = this.contextBlock;
						if (obj != null)
						{
							obj.UpdateWith(this.context);
						}
					}
					else
					{
						this.returnValue = this.mi.GetValue(this.context);
					}
				}
				else
				{
					this.RunParamBlocks();
					this.returnValue = this.mi.Invoke(this.context, this.paramValues);
					if (this.hasReferenceTypes)
					{
						Type[] array = this.ParamTypes();
						for (int i = 0; i < this.paramValues.Length; i++)
						{
							try
							{
								if (array.Length > i)
								{
									if (array[i].IsByRef)
									{
										((Variable_Block)this.paramBlocks[i]).v.SetValue(this.paramValues[i]);
									}
									continue;
								}
							}
							catch (Exception e)
							{
								this.LogError("Error while processing param[" + i + "] => [" + array[i] + "]", e);
								continue;
							}
							break;
						}
					}
				}
			}
			return this.returnValue;
		}

		protected virtual void UpdateBlockWith(object value)
		{
			if ((this.mi == null || !this.returnType.IsValueType || this.returnType.IsPrimitive) && !this.returnType.IsArray)
				return;
			this.mi.SetValue(this.context, value);
		}

		private void RunContextBlock()
		{
			if (this.mi != null && this.mi.IsStatic)
				return;
			Type type = this.ContextType();
			if (this.contextBlock != null)
			{
				this.context = this.contextBlock.Run();
				if (this.context != null && this.context.GetType() != type)
				{
					if (typeof(Component).IsAssignableFrom(type))
					{
						GameObject gameObject = this.context as GameObject;
						if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
						{
							this.context = gameObject.GetComponent(type);
							if (this.context == null)
							{
								this.LogError("Could not find component [" + type.Name + "] on GameObject [" + this.owningEvent.container.gameObject.name + "] while trying to resolve context.", null);
								return;
							}
						}
						else
						{
							Component component = this.context as Component;
							if ((UnityEngine.Object)component != (UnityEngine.Object)null)
							{
								this.context = component.GetComponent(type);
								if (this.context == null)
								{
									this.LogError("Could not find component [" + type.Name + "] on GameObject [" + this.owningEvent.container.gameObject.name + "] while trying to resolve context.", null);
									return;
								}
							}
						}
					}
					if (typeof(GameObject).IsAssignableFrom(type))
					{
						Component component2 = this.context as Component;
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							this.context = component2.gameObject;
						}
					}
				}
			}
			else
			{
				if (this.context == null)
				{
					if (typeof(GameObject).IsAssignableFrom(type))
					{
						this.context = this.owningEvent.container.gameObject;
					}
					else if (typeof(Component).IsAssignableFrom(type))
					{
						this.context = this.owningEvent.container.GetComponent(type);
						if (this.context == null)
						{
							this.LogError("Could not find component [" + type.Name + "] on GameObject [" + this.owningEvent.container.gameObject.name + "] while trying to resolve context.", null);
							return;
						}
					}
				}
				if (this.context == null)
				{
					this.LogError("Error while trying to resolve context: " + type, null);
				}
			}
		}

		private void RunParamBlocks()
		{
			if (((this.paramBlocks != null) ? this.paramBlocks.Length : 0) != 0)
			{
				for (int i = 0; i < this.paramBlocks.Length; i++)
				{
					this.RunParamBlock(i);
				}
			}
		}

		private void RunParamBlock(int i)
		{
			Type type = this.ParamTypes()[i];
			if (type == null)
			{
				this.LogError("Pram type [" + i + "] is null.", null);
			}
			else if (this.paramBlocks[i] != null)
			{
				this.paramValues[i] = this.paramBlocks[i].Run();
				if (this.paramValues[i] != null && this.paramValues[i].GetType() != type)
				{
					if (typeof(Component).IsAssignableFrom(type))
					{
						GameObject gameObject = this.paramValues[i] as GameObject;
						if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
						{
							this.paramValues[i] = gameObject.GetComponent(type);
							if (this.paramValues[i] == null)
							{
								this.LogError("Could not find component [" + type.Name + "] on GameObject [" + this.owningEvent.container.gameObject.name + "] while trying to resolve parameter [" + i + "]", null);
								return;
							}
						}
						else
						{
							Component component = this.paramValues[i] as Component;
							if ((UnityEngine.Object)component != (UnityEngine.Object)null)
							{
								this.paramValues[i] = component.GetComponent(type);
								if (this.paramValues[i] == null)
								{
									this.LogError("Could not find component [" + type.Name + "] on GameObject [" + this.owningEvent.container.gameObject.name + "] while trying to resolve parameter [" + i + "]", null);
									return;
								}
							}
						}
					}
					if (typeof(GameObject).IsAssignableFrom(type))
					{
						Component component2 = this.paramValues[i] as Component;
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							this.paramValues[i] = component2.gameObject;
						}
					}
				}
			}
			else if (typeof(GameObject).IsAssignableFrom(type))
			{
				this.paramValues[i] = this.owningEvent.container.gameObject;
			}
			else if (typeof(Component).IsAssignableFrom(type))
			{
				this.paramValues[i] = this.owningEvent.container.GetComponent(type);
				if (this.paramValues[i] == null)
				{
					this.LogError("Could not find component [" + type.Name + "] on GameObject [" + this.owningEvent.container.gameObject.name + "] while trying to resolve parameter [" + i + "]", null);
				}
			}
		}

		private void InitParamDefaults()
		{
			if (this.mi != null)
			{
				if (this.mi.MemberType == MemberTypes.Field || this.mi.MemberType == MemberTypes.Property)
				{
					if (this.mi.CanSetValue)
					{
						this._defaultParamVals = new object[1];
						this._defaultParamVals[0] = BloxMemberInfo.GetDefaultValue(this.mi.ReturnType);
					}
				}
				else if (this.paramTypes != null)
				{
					this._defaultParamVals = new object[this.paramTypes.Length];
					for (int i = 0; i < this.paramTypes.Length; i++)
					{
						this._defaultParamVals[i] = BloxMemberInfo.GetDefaultValue(this.paramTypes[i]);
					}
				}
			}
			else
			{
				BloxBlockAttribute[] array = (BloxBlockAttribute[])base.GetType().GetCustomAttributes(typeof(BloxBlockAttribute), false);
				if (((array.Length != 0) ? ((array[0].ParamTypes != null) ? array[0].ParamTypes.Length : 0) : 0) != 0)
				{
					this._defaultParamVals = new object[array[0].ParamTypes.Length];
					for (int j = 0; j < this._defaultParamVals.Length; j++)
					{
						this._defaultParamVals[j] = BloxMemberInfo.GetDefaultValue(array[0].ParamTypes[j]);
					}
				}
			}
			if (this._defaultParamVals == null)
			{
				this._defaultParamVals = new object[0];
			}
		}

		public void Init()
		{
			if (this.active)
			{
				if (this.blockType == BloxBlockType.Unknown || (this.mi != null && !this.mi.IsValid))
				{
					this.isValid = false;
				}
				else
				{
					if (this.returnValue == null && this.returnType != null && this.returnType != typeof(void))
					{
						this.returnValue = BloxMemberInfo.GetDefaultValue(this.returnType);
					}
					if (this.owningBlock != null && this.mi != null && (this.mi.MemberType == MemberTypes.Field || this.mi.MemberType == MemberTypes.Property))
					{
						this.paramBlocks = null;
					}
					BloxMemberInfo obj = this.mi;
					this.contextType = ((obj != null) ? obj.ReflectedType : null);
					if (this.contextBlock != null)
					{
						this.contextBlock.owningEvent = this.owningEvent;
						this.contextBlock.owningBlock = this;
						this.contextBlock.Init();
					}
					BloxMemberInfo obj2 = this.mi;
					ParameterInfo[] array = (obj2 != null) ? obj2.GetParameters() : null;
					if (((array != null) ? array.Length : 0) != 0)
					{
						this.paramTypes = new Type[array.Length];
						for (int i = 0; i < this.paramTypes.Length; i++)
						{
							this.paramTypes[i] = array[i].ParameterType;
							if (this.paramTypes[i].IsByRef)
							{
								this.hasReferenceTypes = true;
								if (this.paramBlocks != null && this.paramBlocks.Length >= i && this.paramBlocks[i] != null && this.paramBlocks[i].GetType() == typeof(Variable_Block))
								{
									continue;
								}
								this.LogError("The Block field [" + array[i].Name + "] expected a Variable Block.", null);
								return;
							}
						}
					}
					else if (this.mi != null && (this.mi.MemberType == MemberTypes.Field || this.mi.MemberType == MemberTypes.Property))
					{
						this.paramTypes = new Type[1]
						{
							this.mi.ReturnType
						};
					}
					if (((this.paramBlocks != null) ? this.paramBlocks.Length : 0) != 0)
					{
						this.paramValues = new object[this.paramBlocks.Length];
						for (int j = 0; j < this.paramBlocks.Length; j++)
						{
							if (this.paramBlocks[j] == null)
							{
								if (this.paramTypes != null && this.paramTypes.Length > j)
								{
									this.paramValues[j] = BloxMemberInfo.GetDefaultValue(this.paramTypes[j]);
								}
							}
							else
							{
								this.paramBlocks[j].owningEvent = this.owningEvent;
								this.paramBlocks[j].owningBlock = this;
								this.paramBlocks[j].fieldIdx = j;
								this.paramBlocks[j].Init();
							}
						}
					}
					this.InitBlock();
					if (this.isValid)
					{
						for (BloxBlock bloxBlock = this.firstChild; bloxBlock != null; bloxBlock = bloxBlock.next)
						{
							bloxBlock.parentBlock = this;
							bloxBlock.owningEvent = this.owningEvent;
							if (bloxBlock.next != null)
							{
								bloxBlock.next.prevBlock = bloxBlock;
							}
							bloxBlock.Init();
							if (bloxBlock.selfOrChildCanYield)
							{
								this.selfOrChildCanYield = true;
							}
						}
					}
				}
			}
		}

		public object Run()
		{
			this.flowSig = BloxFlowSignal.None;
			if (this.active && this.isValid)
			{
				object result = this.RunBlock();
				if (this.parentBlock != null)
				{
					this.parentBlock.flowSig = this.flowSig;
					return result;
				}
				this.owningEvent.flowSig = this.flowSig;
				return result;
			}
			return null;
		}

		public void UpdateWith(object val)
		{
			if (this.active && this.isValid)
			{
				this.UpdateBlockWith(val);
			}
		}

		protected void RunChildBlocks()
		{
			if (this.owningEvent.canYield)
			{
				BloxGlobal.Instance.StartCoroutine(this._RunChildBlocksYield());
			}
			else
			{
				this._RunChildBlocks();
			}
		}

		private void _RunChildBlocks()
		{
			BloxBlock bloxBlock = this.firstChild;
			int num = 0;
			while (true)
			{
				if (bloxBlock != null)
				{
					if (bloxBlock == this.firstChild)
					{
						num++;
						if (num >= BloxGlobal.Instance.deadlockDetect)
							break;
					}
					bloxBlock.Run();
					if (this.flowSig == BloxFlowSignal.Continue)
						return;
					if (this.flowSig == BloxFlowSignal.Break)
						return;
					if (this.flowSig == BloxFlowSignal.Stop)
						return;
					bloxBlock = bloxBlock.next;
					continue;
				}
				return;
			}
			Debug.LogErrorFormat(this.owningEvent.container.gameObject, "Deadlock detected in Event [{0}:{1}]. Forcing stop.", this.owningEvent.container.gameObject.name, this.owningEvent.screenName);
			this.flowSig = BloxFlowSignal.Break;
		}

		private IEnumerator _RunChildBlocksYield()
		{
			this.flowSig = BloxFlowSignal.None;
			BloxBlock b = this.firstChild;
			int deadlock = 0;
			while (true)
			{
				if (b != null)
				{
					if (b == this.firstChild)
					{
						deadlock++;
						if (deadlock >= BloxGlobal.Instance.deadlockDetect)
							break;
					}
					b.Run();
					if (this.flowSig == BloxFlowSignal.Wait)
					{
						this.yieldInstruction = b.yieldInstruction;
						yield return b.yieldInstruction;
					}
					else
					{
						if (this.flowSig == BloxFlowSignal.Continue)
							yield break;
						if (this.flowSig == BloxFlowSignal.Break)
							yield break;
						if (this.flowSig == BloxFlowSignal.Stop)
							yield break;
					}
					b = b.next;
					continue;
				}
				yield break;
			}
			Debug.LogErrorFormat(this.owningEvent.container.gameObject, "Deadlock detected in Event [{0}:{1}]. Forcing stop.", this.owningEvent.container.gameObject.name, this.owningEvent.screenName);
			this.flowSig = BloxFlowSignal.Break;
		}

		protected void LogError(string message, Exception e = null)
		{
			this.isValid = false;
			if (this.owningBlock != null)
			{
				if (e != null)
				{
					Debug.LogErrorFormat(this.owningEvent.container, "Error in Block [{2}] used as field of Block [{0}:{1}:{5}]\n{3} => {4}", this.owningEvent.container.gameObject.name, this.owningEvent.screenName, this.ident, message, e.Message, this.owningBlock.ident);
				}
				else
				{
					Debug.LogErrorFormat(this.owningEvent.container, "Error in Block [{2}] used as field of Block [{0}:{1}:{4}]\n{3}", this.owningEvent.container.gameObject.name, this.owningEvent.screenName, this.ident, message, this.owningBlock.ident);
				}
			}
			else if (e != null)
			{
				Debug.LogErrorFormat(this.owningEvent.container, "Error in Block [{0}:{1}:{2}]\n{3} => {4}", this.owningEvent.container.gameObject.name, this.owningEvent.screenName, this.ident, message, e.Message);
			}
			else
			{
				Debug.LogErrorFormat(this.owningEvent.container, "Error in Block [{0}:{1}:{2}]\n{3}", this.owningEvent.container.gameObject.name, this.owningEvent.screenName, this.ident, message);
			}
		}

		protected void LogWarning(string message)
		{
			Debug.LogErrorFormat(this.owningEvent.container, "Problem in Block [{0}:{1}:{2}]\n{3}", this.owningEvent.container.gameObject.name, this.owningEvent.screenName, this.ident, message);
		}
	}
}
