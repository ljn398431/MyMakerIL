using BloxEngine;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(Type_Block))]
	public class Type_BlockDrawer : BloxBlockDrawer
	{
		private static readonly GUIContent GC_Message = new GUIContent("Type Name");

		public override void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}

		public override void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			Type_Block type_Block = (Type_Block)bdi.b;
			GUILayout.Label((type_Block.typeName.Length == 0) ? "-invalid-" : type_Block.typeName, BloxEdGUI.Styles.ActionLabel);
		}

		public override void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			Type_Block obj = (Type_Block)bdi.b;
			EditorGUILayout.PrefixLabel(Type_BlockDrawer.GC_Message);
			obj.typeName = EditorGUILayout.TextField(obj.typeName);
		}
	}
}
