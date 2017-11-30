using BloxEngine;
using UnityEditor;

namespace BloxEditor
{
	[CustomEditor(typeof(BloxGlobal))]
	public class BloxGlobalInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Do not edit or move this asset", MessageType.None);
		}
	}
}
