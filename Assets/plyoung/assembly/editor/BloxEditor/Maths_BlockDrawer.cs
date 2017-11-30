using BloxEngine;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(ADD_Block))]
	[BloxBlockDrawer(typeof(SUB_Block))]
	[BloxBlockDrawer(typeof(DIV_Block))]
	[BloxBlockDrawer(typeof(MOD_Block))]
	[BloxBlockDrawer(typeof(MUL_Block))]
	[BloxBlockDrawer(typeof(B_AND_Block))]
	[BloxBlockDrawer(typeof(B_OR_Block))]
	[BloxBlockDrawer(typeof(B_XOR_Block))]
	[BloxBlockDrawer(typeof(B_LS_Block))]
	[BloxBlockDrawer(typeof(B_RS_Block))]
	public class Maths_BlockDrawer : BloxBlockDrawer
	{
		public override void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}

		public override void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			ed.DrawBlockField(null, bdi, 0);
			GUILayout.Label(bdi.def.paramDefs[2].name, BloxEdGUI.Styles.ActionBoldLabel);
			ed.DrawBlockField(null, bdi, 1);
		}

		public override void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}
	}
}
