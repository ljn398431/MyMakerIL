using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(UIElementUpdater), true)]
	public class UIElementUpdaterInspector : Editor
	{
		private static readonly GUIContent GC_Target = new GUIContent("Target", "Target UI element. Will try to automatically find it if you do not set it here");

		private static readonly GUIContent GC_PropertyName = new GUIContent("Property Name", "Name of a property defined in the Properties Manager");

		private UIElementUpdater Target;

		private SerializedProperty prop_target;

		protected void OnEnable()
		{
			this.prop_target = base.serializedObject.FindProperty("target");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.prop_target, UIElementUpdaterInspector.GC_Target);
			base.serializedObject.ApplyModifiedProperties();
			this.Target = (UIElementUpdater)base.target;
			this.Target.sourceTargetName = plyEdGUI.PopupTextField(UIElementUpdaterInspector.GC_PropertyName, this.Target.sourceTargetName, PropertiesManagerEd.PropertyLabels, false);
			if (GUI.changed)
			{
				plyEdUtil.SetDirty(this.Target);
				GUI.changed = false;
			}
		}
	}
}
