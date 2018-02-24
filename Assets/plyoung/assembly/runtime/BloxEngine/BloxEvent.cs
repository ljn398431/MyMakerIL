using BloxEngine.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine
{
	[Serializable]
	public class BloxEvent
	{
		public string ident;

		public string screenName;

		public bool active;

		public Vector2 _ed_viewOffs = Vector2.zero;

		public bool yieldAllowed;

		public BloxEventData data;

		public List<int> storedBlocksIdx;

		[NonSerialized]
		public BloxBlock firstBlock;

		[NonSerialized]
		public List<BloxBlock> unlinkedBlocks = new List<BloxBlock>();

		[NonSerialized]
		public Blox blox;

		[NonSerialized]
		public BloxContainer container;

		[NonSerialized]
		private plyVariables variables = new plyVariables();

		[NonSerialized]
		public bool canYield;

		[NonSerialized]
		public BloxFlowSignal flowSig;

		private bool _isDirty;

		public BloxEvent Copy()
		{
			BloxEvent bloxEvent = new BloxEvent();
			this.CopyTo(bloxEvent);
			return bloxEvent;
		}

		public void CopyTo(BloxEvent ev)
		{
            Debug.Log("CopyTo", "BloxEvent", Color.yellow);
            if (Application.isPlaying)
			{
				Debug.LogError("This can't be done at runtime");
			}
			else
			{
                Debug.Log("CopyTo", "Blox", Color.yellow);
                this.Serialize();
				if (this.data == null)
				{
					this.data = new BloxEventData();
				}
				ev.ident = this.ident;
				ev.screenName = this.screenName;
				ev.active = this.active;
				ev._ed_viewOffs = Vector2.zero;
				ev.data = this.data.Copy();
				ev.storedBlocksIdx = new List<int>();
				ev.storedBlocksIdx.AddRange(this.storedBlocksIdx);
				ev.Deserialize(this.blox);
			}
		}

		public override string ToString()
		{
			return this.screenName;
		}

		public BloxEvent Init(BloxContainer container, Blox blox)
		{
            Debug.Log("Init", "BloxEvent", Color.yellow);
            if (!this.active)
			{
				return null;
			}
			BloxEvent bloxEvent = new BloxEvent();
			bloxEvent.ident = this.ident;
			bloxEvent.screenName = this.screenName;
			bloxEvent.active = this.active;
			bloxEvent.yieldAllowed = this.yieldAllowed;
			bloxEvent.container = container;
			bloxEvent.data = this.data;
			bloxEvent.storedBlocksIdx = this.storedBlocksIdx;
			bloxEvent.Deserialize(blox);
			bloxEvent.data = null;
			bloxEvent.storedBlocksIdx = null;
			for (BloxBlock next = bloxEvent.firstBlock; next != null; next = next.next)
			{
				next.owningEvent = bloxEvent;
				if (next.next != null)
				{
					next.next.prevBlock = next;
				}
				next.Init();
				if (next.selfOrChildCanYield)
				{
					if (bloxEvent.yieldAllowed)
					{
						bloxEvent.canYield = true;
					}
					else
					{
						Debug.LogWarningFormat("You should not use Flow Blocks related to Waiting in this Event [{0}]. The Wait Block will be ignored.", this.ident);
					}
				}
			}
			return bloxEvent;
		}

		public void GetReadyToRun(BloxEventArg[] args)
		{
			this.variables.Clear();
			if (((args != null) ? args.Length : 0) != 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					this.variables.AddWithValue(args[i].name, args[i].val);
				}
			}
		}

		public plyVar FindVariable(string name, Type expectedType)
		{
			plyVar plyVar = this.variables.FindVariable(name);
			if (plyVar == null && expectedType != null)
			{
				plyVar = this.variables.AddOfValueType(name, expectedType);
			}
			return plyVar;
		}

		public void Run()
		{
			if (this.active && this.firstBlock != null)
			{
				this.flowSig = BloxFlowSignal.None;
				BloxBlock next = this.firstBlock;
				int num = 0;
				while (true)
				{
					if (next != null)
					{
						if (next == this.firstBlock)
						{
							num++;
							if (num >= BloxGlobal.Instance.deadlockDetect)
								break;
						}
						next.Run();
						if (this.flowSig == BloxFlowSignal.Continue)
						{
							this.flowSig = BloxFlowSignal.None;
							next = this.firstBlock;
						}
						else
						{
							if (this.flowSig == BloxFlowSignal.Break)
								return;
							if (this.flowSig == BloxFlowSignal.Stop)
								return;
							next = next.next;
						}
						continue;
					}
					return;
				}
				Debug.LogErrorFormat(this.container.gameObject, "Deadlock detected in Event [{0}:{1}]. Forcing stop.", this.container.gameObject.name, this.screenName);
			}
		}

		public IEnumerator RunYield()
		{
			if (this.active && this.firstBlock != null)
			{
				this.flowSig = BloxFlowSignal.None;
				BloxBlock b = this.firstBlock;
				int deadlock = 0;
				while (true)
				{
					if (b != null)
					{
						if (b == this.firstBlock)
						{
							deadlock++;
							if (deadlock >= BloxGlobal.Instance.deadlockDetect)
								break;
						}
						b.Run();
						if (this.flowSig == BloxFlowSignal.Wait)
						{
							yield return b.yieldInstruction;
						}
						else
						{
							if (this.flowSig == BloxFlowSignal.Continue)
							{
								this.flowSig = BloxFlowSignal.None;
								b = this.firstBlock;
								continue;
							}
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
				Debug.LogErrorFormat(this.container.gameObject, "Deadlock detected in Event [{0}:{1}]. Forcing stop.", this.container.gameObject.name, this.screenName);
			}
		}

		public void _SetDirty()
		{
			if (!Application.isPlaying)
			{
                //Debug.Log("_SetDirty", "BloxEvent", Color.cyan);
                this._isDirty = true;
			}
		}

		public void Serialize()
		{
			if (this._isDirty)
			{
                //Debug.Log("Serialize", "BloxEvent", Color.yellow);
                this._isDirty = false;
				this.storedBlocksIdx = new List<int>();
				List<BloxBlock> list = new List<BloxBlock>();
				this.data = new BloxEventData();
				if (this.firstBlock != null)
				{
					this.storedBlocksIdx.Add(0);
					for (BloxBlock next = this.firstBlock; next != null; next = next.next)
					{
						this.CollectBlockData(next, list, this.data);
					}
				}
				else
				{
					this.storedBlocksIdx.Add(-1);
				}
				for (int i = 0; i < this.unlinkedBlocks.Count; i++)
				{
					this.storedBlocksIdx.Add(list.Count);
					for (BloxBlock bloxBlock = this.unlinkedBlocks[i]; bloxBlock != null; bloxBlock = bloxBlock.next)
					{
						this.CollectBlockData(bloxBlock, list, this.data);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j] != null)
					{
						if (list[j].next != null)
						{
							this.data.blocks[j].next = list.IndexOf(list[j].next);
						}
						if (list[j].firstChild != null)
						{
							this.data.blocks[j].firstChild = list.IndexOf(list[j].firstChild);
						}
						if (list[j].contextBlock != null)
						{
							this.data.blocks[j].contextBlock = list.IndexOf(list[j].contextBlock);
						}
						if (((list[j].paramBlocks != null) ? list[j].paramBlocks.Length : 0) != 0)
						{
							this.data.blocks[j].paramBlocks = new int[list[j].paramBlocks.Length];
							for (int k = 0; k < list[j].paramBlocks.Length; k++)
							{
								this.data.blocks[j].paramBlocks[k] = list.IndexOf(list[j].paramBlocks[k]);
							}
						}
					}
				}
			}
		}

		private void CollectBlockData(BloxBlock b, List<BloxBlock> savedBlocks, BloxEventData data)
		{
            //Debug.Log("CollectBlockData", "BloxEvent", Color.yellow);
            BloxBlockData item = new BloxBlockData(b);
			savedBlocks.Add(b);
			data.blocks.Add(item);
			if (b.contextBlock != null)
			{
				this.CollectBlockData(b.contextBlock, savedBlocks, data);
			}
			if (((b.paramBlocks != null) ? b.paramBlocks.Length : 0) != 0)
			{
				for (int i = 0; i < b.paramBlocks.Length; i++)
				{
					if (b.paramBlocks[i] != null)
					{
						this.CollectBlockData(b.paramBlocks[i], savedBlocks, data);
					}
				}
			}
			if (b.firstChild != null)
			{
				for (b = b.firstChild; b != null; b = b.next)
				{
					this.CollectBlockData(b, savedBlocks, data);
				}
			}
		}

		public void Deserialize(Blox owningBlox)
		{
            //Debug.Log("Deserialize", "BloxEvent", Color.yellow);
            this.blox = owningBlox;
			if (this._isDirty)
			{
                Debug.Log("Deserialize", "BloxEvent", Color.yellow);
                this.Serialize();
			}
			if (this.data != null)
			{
				this.firstBlock = null;
				this.unlinkedBlocks.Clear();
				List<BloxBlock> list = new List<BloxBlock>();
				for (int i = 0; i < this.data.blocks.Count; i++)
				{
					BloxBlock item = this.data.blocks[i].CreateBlock();
					list.Add(item);
				}
				for (int j = 0; j < this.data.blocks.Count; j++)
				{
					if (this.data.blocks[j].next >= 0)
					{
						list[j].next = list[this.data.blocks[j].next];
					}
					if (this.data.blocks[j].firstChild >= 0)
					{
						list[j].firstChild = list[this.data.blocks[j].firstChild];
					}
					if (this.data.blocks[j].contextBlock >= 0)
					{
						list[j].contextBlock = list[this.data.blocks[j].contextBlock];
					}
					if (this.data.blocks[j].paramBlocks.Length != 0)
					{
						list[j].paramBlocks = new BloxBlock[this.data.blocks[j].paramBlocks.Length];
						for (int k = 0; k < this.data.blocks[j].paramBlocks.Length; k++)
						{
							if (this.data.blocks[j].paramBlocks[k] >= 0)
							{
								list[j].paramBlocks[k] = list[this.data.blocks[j].paramBlocks[k]];
							}
						}
					}
				}
				if (this.storedBlocksIdx != null)
				{
					if (this.storedBlocksIdx.Count > 0 && this.storedBlocksIdx[0] >= 0)
					{
						this.firstBlock = list[this.storedBlocksIdx[0]];
					}
					if (this.storedBlocksIdx.Count > 1)
					{
						for (int l = 1; l < this.storedBlocksIdx.Count; l++)
						{
							if (this.storedBlocksIdx[l] >= 0 && this.storedBlocksIdx[l] < list.Count)
							{
								BloxBlock bloxBlock = list[this.storedBlocksIdx[l]];
								if (bloxBlock != null)
								{
									this.unlinkedBlocks.Add(bloxBlock);
								}
							}
						}
					}
				}
			}
		}
	}
}
