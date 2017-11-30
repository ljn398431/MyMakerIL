using BloxEngine;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(EQ_Block))]
	[BloxBlockDrawer(typeof(NEQ_Block))]
	[BloxBlockDrawer(typeof(AND_Block))]
	[BloxBlockDrawer(typeof(OR_Block))]
	[BloxBlockDrawer(typeof(GT_Block))]
	[BloxBlockDrawer(typeof(GTE_Block))]
	[BloxBlockDrawer(typeof(LT_Block))]
	[BloxBlockDrawer(typeof(LTE_Block))]
	public class Conditions_BlockDrawer : BloxBlockDrawer
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
