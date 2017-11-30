using BloxGameSystems;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(PropertiesManager))]
	public class PropertiesManagerInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Do not edit or move this asset", MessageType.None);
		}
	}
}
