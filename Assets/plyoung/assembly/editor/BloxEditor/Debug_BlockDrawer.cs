using BloxEngine;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(Debug_Block))]
	public class Debug_BlockDrawer : BloxBlockDrawer
	{
		private static readonly GUIContent GC_Head = new GUIContent("Debug");

		private static readonly GUIContent GC_Message = new GUIContent("Message");

		private static readonly GUIContent GC_Info = new GUIContent("Use the [+] and [-] buttons to increase or decrease the number of Block fields to show. These can be used to include additional values in the message.");

		private static readonly GUIContent GC_Add = new GUIContent(Ico._add, "Add a field");

		private static readonly GUIContent GC_Remove = new GUIContent(Ico._remove, "Remove last added field");

		private static readonly GUIContent GC_Comma = new GUIContent(",");

		public override void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			GUILayout.Label(Debug_BlockDrawer.GC_Head, BloxEdGUI.Styles.ActionLabel);
		}

		public override void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			if (bdi.b.paramBlocks == null)
			{
				bdi.b.paramBlocks = new BloxBlock[0];
			}
			GUILayout.Label(((Debug_Block)bdi.b).message, BloxEdGUI.Styles.FieldLabel);
			if (bdi.paramBlocks.Length != 0)
			{
				for (int i = 0; i < bdi.paramBlocks.Length; i++)
				{
					if (i != 0)
					{
						GUILayout.Label(Debug_BlockDrawer.GC_Comma, BloxEdGUI.Styles.FieldLabel);
					}
					ed.DrawBlockField(null, bdi, i);
				}
			}
		}

		public override void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			if (bdi.b.paramBlocks == null)
			{
				bdi.b.paramBlocks = new BloxBlock[0];
			}
			Debug_Block debug_Block = (Debug_Block)bdi.b;
			EditorGUILayout.PrefixLabel(Debug_BlockDrawer.GC_Message);
			debug_Block.message = EditorGUILayout.TextField(debug_Block.message);
			debug_Block.logType = (Debug_Block.DebugBlockLogType)EditorGUILayout.EnumPopup((Enum)(object)debug_Block.logType);
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(Debug_BlockDrawer.GC_Add, plyEdGUI.Styles.MiniButtonLeft, GUILayout.Width(30f)))
			{
				ArrayUtility.Add<BloxBlockEd>(ref bdi.paramBlocks, (BloxBlockEd)null);
				ArrayUtility.Add<BloxBlock>(ref bdi.b.paramBlocks, (BloxBlock)null);
				GUI.changed = true;
			}
			GUI.enabled = (bdi.paramBlocks.Length != 0);
			if (GUILayout.Button(Debug_BlockDrawer.GC_Remove, plyEdGUI.Styles.MiniButtonRight, GUILayout.Width(30f)))
			{
				ArrayUtility.RemoveAt<BloxBlockEd>(ref bdi.paramBlocks, bdi.paramBlocks.Length - 1);
				ArrayUtility.RemoveAt<BloxBlock>(ref bdi.b.paramBlocks, bdi.b.paramBlocks.Length - 1);
				GUI.changed = true;
			}
			GUI.enabled = true;
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Label(Debug_BlockDrawer.GC_Info, plyEdGUI.Styles.WordWrappedLabel);
		}
	}
}
