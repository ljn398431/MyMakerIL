using BloxEngine;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(NOT_Block))]
	public class NOT_BlockDrawer : BloxBlockDrawer
	{
		private static GUIContent GC_Not = new GUIContent("not");

		public override void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}

		public override void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			GUILayout.Label(NOT_BlockDrawer.GC_Not, BloxEdGUI.Styles.ActionBoldLabel);
			ed.DrawBlockField(null, bdi, 0);
		}

		public override void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}
	}
}
