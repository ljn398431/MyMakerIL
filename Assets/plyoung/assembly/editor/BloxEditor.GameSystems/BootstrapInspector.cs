using BloxGameSystems;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(Bootstrap))]
	public class BootstrapInspector : Editor
	{
		private SerializedProperty prop_onBootstrapDone;

		private static readonly string STR_Descr = "OnBootstrapDone is triggered when bootstrap is done loading all scenes marked for startup/ auto-loading";

		protected void OnEnable()
		{
			this.prop_onBootstrapDone = base.serializedObject.FindProperty("onBootstrapDone");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(BootstrapInspector.STR_Descr, MessageType.None);
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.prop_onBootstrapDone);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
