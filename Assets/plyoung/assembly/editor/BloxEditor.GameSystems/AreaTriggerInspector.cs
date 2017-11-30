using BloxGameSystems;
using plyLibEditor;
using UnityEditor;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(AreaTrigger))]
	public class AreaTriggerInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			plyEdGUI.DrawInspectorGUI(base.serializedObject);
		}
	}
}
