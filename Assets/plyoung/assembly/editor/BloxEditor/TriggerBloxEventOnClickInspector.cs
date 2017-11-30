using BloxEditor.Variables;
using BloxEngine;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[CustomEditor(typeof(TriggerBloxEventOnClick))]
	public class TriggerBloxEventOnClickInspector : plyVariablesBehaviourInspector
	{
		private static readonly GUIContent GC_Button = new GUIContent("Button", "The button to bind to. Will respond on clicks on this button");

		private static readonly GUIContent GC_Container = new GUIContent("Container", "The Blox container which contains the Blox Definition(s) with target event");

		private static readonly GUIContent GC_EventName = new GUIContent("Event Name", "Name of event to trigger");

		private SerializedProperty prop_button;

		private SerializedProperty prop_container;

		private SerializedProperty prop_eventName;

		protected override void OnEnable()
		{
			base.varsType = plyVariablesType.Custom;
			base.OnEnable();
			base.varEditor.GC_Head = new GUIContent("Values");
			base.varEditor.canStartDragDrop = false;
			base.varEditor.ForceParamNames();
			this.prop_button = base.serializedObject.FindProperty("button");
			this.prop_container = base.serializedObject.FindProperty("container");
			this.prop_eventName = base.serializedObject.FindProperty("eventName");
		}

		public override void OnInspectorGUI()
		{
			TriggerBloxEventOnClick dirty = (TriggerBloxEventOnClick)base.target;
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.prop_button, TriggerBloxEventOnClickInspector.GC_Button);
			EditorGUILayout.PropertyField(this.prop_container, TriggerBloxEventOnClickInspector.GC_Container);
			EditorGUILayout.PropertyField(this.prop_eventName, TriggerBloxEventOnClickInspector.GC_EventName);
			base.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Space();
			base.OnInspectorGUI();
			if (GUI.changed)
			{
				plyEdUtil.SetDirty(dirty);
				GUI.changed = false;
			}
		}
	}
}
