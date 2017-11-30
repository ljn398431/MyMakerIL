using BloxEngine.Databinding;
using UnityEditor;

namespace BloxEditor.Databinding
{
	[CustomEditor(typeof(DataProvider), true)]
	public class DataProviderInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Do not edit or move this asset", MessageType.None);
		}
	}
}
