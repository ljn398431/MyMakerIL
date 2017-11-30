using BloxEngine;
using BloxEngine.Variables;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[CustomEditor(typeof(GlobalVariables))]
	public class GlobalVariablesInspector : plyVariablesBehaviourInspector
	{
		private static readonly string STR_Descr = "These Variables are Global to the game and can be accessed at any time by any object. If you want to create variables specific to an object you should use the 'Object Variables' component on that object";

		protected override void OnEnable()
		{
			base.varsType = plyVariablesType.Global;
			base.OnEnable();
			base.varEditor.GC_Head = new GUIContent("Global Variables");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(GlobalVariablesInspector.STR_Descr, MessageType.None);
			base.OnInspectorGUI();
		}
	}
}
