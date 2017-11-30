using BloxGameSystems;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(DontDestroyOnLoad))]
	public class DontDestroyOnLoadInspector : Editor
	{
		private static readonly string STR_Descr = "This will set the GameObject to not be destroyed when a scene load occurs. Normally all objects would be removed when a new scene/ level is loaded, except if the scene is loaded additively. This component will remove itself once it has done this";

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(DontDestroyOnLoadInspector.STR_Descr, MessageType.None);
		}
	}
}
