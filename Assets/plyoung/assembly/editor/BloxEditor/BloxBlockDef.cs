using BloxEngine;
using System;
using UnityEngine;

namespace BloxEditor
{
	public class BloxBlockDef : BloxEdDef
	{
		public class Param
		{
			public string name;

			public Type type;

			public bool showName = true;

			public string emptyVal;

			public bool isRefType;
		}

		public BloxBlockType blockType;

		public string shortName;

		public Texture2D icon;

		private GUIStyle[] _style;

		public bool showIconInCanvas;

		public Type blockSystemType;

		public BloxBlockAttribute att;

		public Type returnType;

		public string returnName;

		public BloxMemberInfo mi;

		public Type contextType;

		public string contextName;

		public BloxBlockDef singleton;

		public Param[] paramDefs = new Param[0];

		public int overrideRenderFields;

		public BloxBlockDrawer drawer;

		public bool isYieldBlock;

		public GUIStyle[] style
		{
			get
			{
				object obj = this._style;
				if (obj == null)
				{
					BloxBlockType bt = this.blockType;
					BloxBlockAttribute obj2 = this.att;
					obj = (this._style = BloxEd.GetBlockStyle(bt, (obj2 != null) ? obj2.Style : null));
				}
				return (GUIStyle[])obj;
			}
		}
	}
}
