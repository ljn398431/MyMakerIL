using BloxEngine;
using BloxEngine.Variables;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[CustomEditor(typeof(ObjectVariables))]
	public class ObjectVariablesInspector : plyVariablesBehaviourInspector
	{
		protected override void OnEnable()
		{
			base.varsType = plyVariablesType.Object;
			base.extraButtons = new plyReorderableList.Button[1]
			{
				new plyReorderableList.Button
				{
					label = new GUIContent(Ico._global_variable, "Go to Global Variables"),
					callback = this.OnGlobalVariablesButton
				}
			};
			base.OnEnable();
			base.varEditor.GC_Head = new GUIContent("Object Variables");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();
			base.OnInspectorGUI();
		}

		private void OnGlobalVariablesButton()
		{
			GameObject bloxGlobalPrefab = BloxEd.BloxGlobalPrefab;
			if ((Object)bloxGlobalPrefab != (Object)null)
			{
				Selection.activeGameObject = bloxGlobalPrefab;
			}
			else
			{
				EditorUtility.DisplayDialog("Global Variables", "The BloxGlobal could not be found or created.", "OK");
			}
		}
	}
}
