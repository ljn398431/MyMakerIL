using UnityEditor;

namespace BloxEditor
{
	[CustomEditor(typeof(BloxEd))]
	public class BloxEdInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Do not edit or move this asset", MessageType.None);
		}
	}
}
