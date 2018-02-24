using BloxEngine;
using System;
using System.Reflection;
using UnityEngine;

namespace BloxEditor
{
    public class BloxBlockEd
    {
        public BloxBlock b;

        public BloxBlockDef def;

        public BloxBlockEd next;

        public BloxBlockEd prev;

        public BloxBlockEd firstChild;

        public BloxBlockEd parentBlock;

        public BloxBlockEd owningBlock;

        public int fieldIdx = -1;

        public BloxBlockEd contextBlock;

        public BloxBlockEd[] paramBlocks = new BloxBlockEd[0];

        public BloxBlockDef _changeDefTo;

        private object[] parVals;

        private Type _sgReturnType;

        public object[] DefaultParamVals
        {
            get
            {
                if (this.parVals == null)
                {
                    this.Init_parVals();
                }
                return this.parVals;
            }
        }

        public Type sgReturnType
        {
            get
            {
                return this._sgReturnType ?? this.b.returnType;
            }
            set
            {
                this._sgReturnType = value;
            }
        }

        public BloxBlockEd(BloxBlock b, BloxBlockEd prev, BloxBlockEd parentBlock, BloxBlockEd owningBlock, int fieldIdx, bool checkNextToo = true)
        {
            //Debug.Log("BloxBlockEd", "BloxBlockEd", Color.cyan);
            this.b = b;
            this.def = BloxEd.Instance.FindBlockDef(b);
            this.prev = prev;
            this.parentBlock = parentBlock;
            this.owningBlock = owningBlock;
            if (owningBlock != null)
            {
                Debug.Log("b" + b.ident, "BloxBlockEd", Color.green);
                Debug.Log("owningBlock" + owningBlock.b.ident, "BloxBlockEd", Color.green);
            }
            this.fieldIdx = fieldIdx;
            if (b.contextBlock != null)
            {
                this.contextBlock = new BloxBlockEd(b.contextBlock, null, null, this, -1, true);
            }
            if (((b.paramBlocks != null) ? b.paramBlocks.Length : 0) != 0)
            {
                this.paramBlocks = new BloxBlockEd[b.paramBlocks.Length];
                for (int i = 0; i < b.paramBlocks.Length; i++)
                {
                    if (b.paramBlocks[i] != null)
                    {
                        this.paramBlocks[i] = new BloxBlockEd(b.paramBlocks[i], null, null, this, i, true);
                    }
                }
            }
            if (b.firstChild != null)
            {
                this.firstChild = new BloxBlockEd(b.firstChild, null, this, null, -1, false);
                BloxBlockEd bloxBlockEd = this.firstChild;
                while (bloxBlockEd.b.next != null)
                {
                    bloxBlockEd.next = new BloxBlockEd(bloxBlockEd.b.next, bloxBlockEd, this, null, -1, false);
                    bloxBlockEd = bloxBlockEd.next;
                }
            }
            if (checkNextToo)
            {
                this.UpdateNextBlock(parentBlock);
            }
        }

        private void UpdateNextBlock(BloxBlockEd parentBlock)
        {
            if (this.b != null && this.b.next != null)
            {
                this.next = new BloxBlockEd(this.b.next, this, parentBlock, null, -1, true);
                this.next.UpdateNextBlock(parentBlock);
            }
        }

        private void Init_parVals()
        {
            if (this.b.mi != null)
            {
                if (this.b.mi.MemberType == MemberTypes.Field || this.b.mi.MemberType == MemberTypes.Property)
                {
                    if (this.b.mi.CanSetValue)
                    {
                        this.parVals = new object[1];
                        this.parVals[0] = BloxMemberInfo.GetDefaultValue(this.b.mi.ReturnType);
                    }
                }
                else
                {
                    ParameterInfo[] parameters = this.b.mi.GetParameters();
                    if (parameters != null)
                    {
                        this.parVals = new object[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            this.parVals[i] = BloxMemberInfo.GetDefaultValue(parameters[i].ParameterType);
                        }
                    }
                }
            }
            else
            {
                BloxBlockAttribute[] array = (BloxBlockAttribute[])this.b.GetType().GetCustomAttributes(typeof(BloxBlockAttribute), false);
                if (((array.Length != 0) ? ((array[0].ParamTypes != null) ? array[0].ParamTypes.Length : 0) : 0) != 0)
                {
                    this.parVals = new object[array[0].ParamTypes.Length];
                    for (int j = 0; j < this.parVals.Length; j++)
                    {
                        this.parVals[j] = BloxMemberInfo.GetDefaultValue(array[0].ParamTypes[j]);
                    }
                }
            }
            if (this.parVals == null)
            {
                this.parVals = new object[0];
            }
        }

        public Type[] ParameterTypes()
        {
            Type[] array = null;
            if (this.b.mi != null)
            {
                if (this.b.mi.MemberType == MemberTypes.Field || this.b.mi.MemberType == MemberTypes.Property)
                {
                    array = ((!this.b.mi.CanSetValue) ? new Type[0] : new Type[1]
                    {
                        this.b.mi.ReturnType
                    });
                }
                else
                {
                    ParameterInfo[] parameters = this.b.mi.GetParameters();
                    if (((parameters != null) ? parameters.Length : 0) != 0)
                    {
                        array = new Type[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            array[i] = parameters[i].ParameterType;
                        }
                    }
                    else
                    {
                        array = new Type[0];
                    }
                }
                return array;
            }
            array = this.b.ParamTypes();
            if (array != null)
            {
                return array;
            }
            if (((this.def.paramDefs != null) ? this.def.paramDefs.Length : 0) != 0)
            {
                array = new Type[this.def.paramDefs.Length];
                for (int j = 0; j < this.def.paramDefs.Length; j++)
                {
                    array[j] = this.def.paramDefs[j].type;
                }
            }
            return array;
        }

        public BloxBlockEd LastInChain()
        {
            if (this.next == null)
            {
                return this;
            }
            BloxBlockEd bloxBlockEd = this.next;
            while (bloxBlockEd.next != null)
            {
                bloxBlockEd = bloxBlockEd.next;
            }
            return bloxBlockEd;
        }

        public bool IsOrInLoop()
        {
            if (this.def.att.IsLoopBlock)
            {
                return true;
            }
            if (this.parentBlock != null)
            {
                return this.parentBlock.IsOrInLoop();
            }
            return false;
        }
    }
}
