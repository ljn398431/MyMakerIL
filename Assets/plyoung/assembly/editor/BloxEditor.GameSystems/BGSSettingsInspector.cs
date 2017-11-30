using BloxGameSystems;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(BGSSettings))]
	public class BGSSettingsInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Do not edit or move this asset", MessageType.None);
		}
	}
}
