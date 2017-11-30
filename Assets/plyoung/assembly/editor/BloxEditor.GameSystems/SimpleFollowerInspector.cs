using BloxGameSystems;
using plyLibEditor;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(SimpleFollower))]
	public class SimpleFollowerInspector : Editor
	{
		private static readonly string STR_Descr = "Make this object follow the target object";

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(SimpleFollowerInspector.STR_Descr, MessageType.None);
			plyEdGUI.DrawInspectorGUI(base.serializedObject);
		}
	}
}
