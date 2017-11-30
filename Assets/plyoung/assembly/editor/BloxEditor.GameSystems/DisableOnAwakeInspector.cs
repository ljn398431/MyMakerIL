using BloxGameSystems;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(DisableOnAwake))]
	public class DisableOnAwakeInspector : Editor
	{
		private static readonly string STR_Descr = "This will disable the GameObject during Awake (when the object is loaded). This component will remove itself once it has done this";

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(DisableOnAwakeInspector.STR_Descr, MessageType.None);
		}
	}
}
