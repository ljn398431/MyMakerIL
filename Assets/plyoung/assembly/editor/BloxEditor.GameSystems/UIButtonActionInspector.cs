using BloxEditor.Databinding;
using BloxEditor.Variables;
using BloxEngine;
using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(UIButtonAction))]
	public class UIButtonActionInspector : plyVariablesBehaviourInspector
	{
		private static readonly GUIContent GC_Target = new GUIContent("Target", "Target UI element. Will try to automatically find it if you do not set it here");

		private static readonly GUIContent GC_ClickAction = new GUIContent("OnClick Action");

		private static readonly GUIContent GC_Button = new GUIContent("-none-");

		private SerializedProperty prop_button;

		protected override void OnEnable()
		{
			base.varsType = plyVariablesType.Custom;
			base.OnEnable();
			base.varEditor.GC_Head = new GUIContent("Blackboard");
			base.varEditor.canStartDragDrop = false;
			this.prop_button = base.serializedObject.FindProperty("button");
		}

		public override void OnInspectorGUI()
		{
			UIButtonAction uIButtonAction = (UIButtonAction)base.target;
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.prop_button, UIButtonActionInspector.GC_Target);
			base.serializedObject.ApplyModifiedProperties();
			DataProviderEd.DataBindingField(UIButtonActionInspector.GC_ClickAction, uIButtonAction.databinding, uIButtonAction, true, null);
			EditorGUILayout.Space();
			base.OnInspectorGUI();
			if (GUI.changed)
			{
				plyEdUtil.SetDirty(uIButtonAction);
				GUI.changed = false;
			}
		}
	}
}
