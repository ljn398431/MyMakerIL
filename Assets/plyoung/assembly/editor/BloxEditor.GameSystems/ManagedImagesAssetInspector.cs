using BloxGameSystems;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(ManagedImagesAsset))]
	public class ManagedImagesAssetInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Do not edit or move this asset", MessageType.None);
		}
	}
}
