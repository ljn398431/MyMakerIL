using BloxGameSystems;
using plyLibEditor;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(SoundVolumeUpdater))]
	public class SoundVolumeUpdaterInspector : Editor
	{
		private static readonly string STR_Descr = "Automatically update the target audio source's volume when the associated volume type changes";

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(SoundVolumeUpdaterInspector.STR_Descr, MessageType.None);
			plyEdGUI.DrawInspectorGUI(base.serializedObject);
		}
	}
}
