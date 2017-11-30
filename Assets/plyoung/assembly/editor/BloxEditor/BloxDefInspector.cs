using BloxEngine;
using UnityEditor;

namespace BloxEditor
{
	[CustomEditor(typeof(Blox))]
	public class BloxDefInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Do not edit or move this asset", MessageType.None);
		}
	}
}
