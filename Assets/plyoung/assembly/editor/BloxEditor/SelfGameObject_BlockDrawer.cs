using BloxEngine;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(SelfGameObject_Block))]
	public class SelfGameObject_BlockDrawer : BloxBlockDrawer
	{
		private static readonly GUIContent GC_Self = new GUIContent("self: GameObject");

		public override void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			GUILayout.Label(SelfGameObject_BlockDrawer.GC_Self, BloxEdGUI.Styles.FieldLabel);
		}

		public override void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}

		public override void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}
	}
}
