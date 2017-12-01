using BloxEngine;
using plyLibEditor;
using System.Collections;
using System.Collections.Generic;

namespace BloxEditor
{
    public class BloxEventEd
    {
        public BloxEvent ev;

        public BloxEventDef def;

        public BloxBlockEd firstBlock;

        public List<BloxBlockEd> unlinkedBlocks = new List<BloxBlockEd>();

        public bool hasUndefinedblocks;

        private bool _loading;

        private plyEdCoroutine _loader;

        private const int EntriesPorcessCount = 1;

        public bool Loading
        {
            get
            {
                return this._loading;
            }
        }

        public void Clear()
        {
            this._loading = false;
            plyEdCoroutine loader = this._loader;
            if (loader != null)
            {
                loader.Stop();
            }
            this.ev = null;
            this.def = null;
            this.firstBlock = null;
            this.hasUndefinedblocks = false;
            this.unlinkedBlocks.Clear();
        }

        public void Set(BloxEvent ev, bool forScriptGen = false)
        {
            Debug.Log("Set ", "BloxEventEd", UnityEngine.Color.green);
            if (this.ev != ev)
            {
                this.hasUndefinedblocks = false;
                this.Clear();
                if (ev != null)
                {
                    this.def = BloxEd.Instance.FindEventDef(ev);
                    if (this.def != null)
                    {
                        this.ev = ev;
                        if (forScriptGen)
                        {
                            Debug.Log("forScriptGen " + forScriptGen, "BloxEventEd", UnityEngine.Color.green);
                            this.firstBlock = ((ev.firstBlock == null) ? null : new BloxBlockEd(ev.firstBlock, null, null, null, -1, false));
                            if (this.firstBlock != null)
                            {
                                BloxBlockEd next = this.firstBlock;
                                while (next.b.next != null)
                                {
                                    next.next = new BloxBlockEd(next.b.next, next, null, null, -1, false);
                                    next = next.next;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("forScriptGen " + forScriptGen, "BloxEventEd", UnityEngine.Color.green);
                            this._loading = true;
                            this._loader = plyEdCoroutine.Start(this.LoadEvent(), true);
                        }
                    }
                }
            }
        }

        public void DoUpdate()
        {
            if (this._loader != null)
            {
                this._loader.DoUpdate();
            }
        }
        /// <summary>
        /// 加载 事件内 积木
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadEvent()
        {
            int count = 0;
            if (this.ev.firstBlock == null)
            {
                Debug.Log("LoadEvent .ev.firstBlock == null", "BloxEventEd", UnityEngine.Color.green);
                this.firstBlock = null;
            }
            else
            {
                yield return (object)null;
                this.firstBlock = new BloxBlockEd(this.ev.firstBlock, null, null, null, -1, false);
                BloxBlockEd bdi2 = this.firstBlock;
                Debug.Log("bdi2" + bdi2.b.ident, "BloxEventEd", UnityEngine.Color.green);
                while (bdi2.b.next != null)
                {
                    count++;
                    if (count >= 1)
                    {
                        count = 0;
                        yield return (object)null;
                    }
                    Debug.Log("Draw linked bdi2.b.next"+ bdi2.b.next.ident, "BloxEventEd", UnityEngine.Color.green);
                    bdi2.next = new BloxBlockEd(bdi2.b.next, bdi2, null, null, -1, false);
                    bdi2 = bdi2.next;
                }
            }
            if (this.ev.unlinkedBlocks.Count > 0)
            {
                Debug.Log(".ev.unlinkedBlocks.Count " + this.ev.unlinkedBlocks.Count, "BloxEventEd", UnityEngine.Color.green);
                yield return (object)null;
                for (int i = 0; i < this.ev.unlinkedBlocks.Count; i++)
                {
                    Debug.Log("draw unlinked blocks " + this.ev.unlinkedBlocks.Count, "BloxEventEd", UnityEngine.Color.green);
                    BloxBlockEd bdi;
                    BloxBlockEd f_bdi = bdi = new BloxBlockEd(this.ev.unlinkedBlocks[i], null, null, null, -1, false);
                    while (bdi.b.next != null)
                    {
                        count++;
                        if (count >= 1)
                        {
                            count = 0;
                            yield return (object)null;
                        }
                        bdi.next = new BloxBlockEd(bdi.b.next, bdi, null, null, -1, false);
                        bdi = bdi.next;
                    }
                    this.unlinkedBlocks.Add(f_bdi);
                }
            }
            yield return (object)null;
            //Debug.Log(" this._loading = false", "BloxEventEd", UnityEngine.Color.red);
            this._loading = false;
        }

        public bool HasYieldInstruction()
        {
            return this._CheckForYieldRecursive(this.firstBlock);
        }

        private bool _CheckForYieldRecursive(BloxBlockEd b)
        {
            while (b != null)
            {
                if (b.def.isYieldBlock)
                {
                    return true;
                }
                if (b.firstChild != null && this._CheckForYieldRecursive(b.firstChild))
                {
                    return true;
                }
                b = b.next;
            }
            return false;
        }

        public bool CheckEventBlockDefs()
        {
            this.hasUndefinedblocks = false;
            if (this.firstBlock == null)
            {
                return true;
            }
            if (BloxEd.Instance.BlockDefsLoading)
            {
                BloxEd.Instance.DoUpdate();
                return false;
            }
            if (BloxBlocksList.HasInstance && BloxBlocksList.Instance.IsBuildingList)
            {
                BloxBlocksList.Instance.DoUpdate();
                return false;
            }
            BloxBlockEd next = this.firstBlock;
            while (next != null && !this.CheckBlockDef(next))
            {
                next = next.next;
            }
            return true;
        }

        private bool CheckBlockDef(BloxBlockEd bdi)
        {
            BloxBlock _ = bdi.b;
            if (bdi.b.blockType != 0 && (bdi.b.mi == null || bdi.b.mi.IsValid))
            {
                if (bdi.def == null)
                {
                    bdi.def = BloxEd.Instance.FindBlockDef(bdi.b);
                    if (((bdi.def != null) ? bdi.def.blockType : BloxBlockType.Unknown) != 0)
                    {
                        goto IL_0078;
                    }
                    this.hasUndefinedblocks = true;
                    return true;
                }
                goto IL_0078;
            }
            this.hasUndefinedblocks = true;
            return true;
        IL_0078:
            if (bdi.contextBlock != null && this.CheckBlockDef(bdi.contextBlock))
            {
                return true;
            }
            for (int i = 0; i < bdi.paramBlocks.Length; i++)
            {
                if (bdi.paramBlocks[i] != null && this.CheckBlockDef(bdi.paramBlocks[i]))
                {
                    return true;
                }
            }
            for (bdi = bdi.firstChild; bdi != null; bdi = bdi.next)
            {
                if (this.CheckBlockDef(bdi))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
