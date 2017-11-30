using BloxEngine;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockDrawer(typeof(TriggerEvent_Block))]
	public class TriggeEvent_BlockDrawer : BloxBlockDrawer
	{
		private static readonly GUIContent GC_Head = new GUIContent("Trigger Event");

		private static readonly GUIContent GC_Seconds = new GUIContent("seconds");

		private static readonly GUIContent GC_With = new GUIContent("with");

		private static readonly GUIContent GC_EventsVars = new GUIContent("Events Parameters");

		private static readonly GUIContent GC_Add = new GUIContent(Ico._add, "Add a field");

		private static readonly GUIContent GC_Remove = new GUIContent(Ico._remove, "Remove last field");

		private static readonly GUIContent GC_Param = new GUIContent("");

		private static readonly GUIContent GC_EventVar = new GUIContent(Ico._event_variable);

		public override void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			GUILayout.Label(TriggeEvent_BlockDrawer.GC_Head, BloxEdGUI.Styles.ActionLabel);
		}

		public override void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			ed.DrawBlockField(null, bdi, 0);
			ed.DrawBlockField(null, bdi, 1);
			ed.DrawBlockField(null, bdi, 2);
			GUILayout.Label(TriggeEvent_BlockDrawer.GC_Seconds, BloxEdGUI.Styles.FieldLabel);
			if (bdi.paramBlocks.Length > 3)
			{
				TriggerEvent_Block _ = (TriggerEvent_Block)bdi.b;
				GUILayout.Label(TriggeEvent_BlockDrawer.GC_With, BloxEdGUI.Styles.FieldLabel);
				for (int i = 3; i < bdi.paramBlocks.Length; i++)
				{
					TriggeEvent_BlockDrawer.GC_Param.text = "param" + (i - 3).ToString() + "=";
					GUILayout.Label(TriggeEvent_BlockDrawer.GC_EventVar, BloxEdGUI.Styles.IconLabel);
					GUILayout.Label(TriggeEvent_BlockDrawer.GC_Param, BloxEdGUI.Styles.FieldLabel);
					ed.DrawBlockField(null, bdi, i);
				}
			}
		}

		public override void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi)
		{
			TriggerEvent_Block _ = (TriggerEvent_Block)bdi.b;
			GUILayout.Label(TriggeEvent_BlockDrawer.GC_EventsVars);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(TriggeEvent_BlockDrawer.GC_Add, plyEdGUI.Styles.MiniButtonLeft, GUILayout.Width(30f)))
			{
				ArrayUtility.Add<BloxBlockEd>(ref bdi.paramBlocks, (BloxBlockEd)null);
				ArrayUtility.Add<BloxBlock>(ref bdi.b.paramBlocks, (BloxBlock)null);
				GUI.changed = true;
			}
			GUI.enabled = (bdi.paramBlocks.Length > 3);
			if (GUILayout.Button(TriggeEvent_BlockDrawer.GC_Remove, plyEdGUI.Styles.MiniButtonRight, GUILayout.Width(30f)))
			{
				ArrayUtility.RemoveAt<BloxBlockEd>(ref bdi.paramBlocks, bdi.paramBlocks.Length - 1);
				ArrayUtility.RemoveAt<BloxBlock>(ref bdi.b.paramBlocks, bdi.b.paramBlocks.Length - 1);
				GUI.changed = true;
			}
			GUI.enabled = true;
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
}
