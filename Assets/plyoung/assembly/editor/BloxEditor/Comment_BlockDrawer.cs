using BloxEngine;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(Comment_Block))]
	public class Comment_BlockDrawer : BloxBlockDrawer
	{
		private static readonly GUIContent GC_Message = new GUIContent("");

		private static readonly GUIContent GC_Head = new GUIContent("Message");

		public override void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi)
		{
		}

		public override void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			Comment_Block comment_Block = (Comment_Block)bdi.b;
			Comment_BlockDrawer.GC_Message.text = comment_Block.message;
			GUILayout.Label(Comment_BlockDrawer.GC_Message, BloxEdGUI.Styles.ActionLabel);
		}

		public override void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			if (bdi.b.paramBlocks == null)
			{
				bdi.b.paramBlocks = new BloxBlock[0];
			}
			Comment_Block obj = (Comment_Block)bdi.b;
			EditorGUILayout.PrefixLabel(Comment_BlockDrawer.GC_Head);
			obj.message = EditorGUILayout.TextArea(obj.message, plyEdGUI.Styles.TextArea);
		}
	}
}
