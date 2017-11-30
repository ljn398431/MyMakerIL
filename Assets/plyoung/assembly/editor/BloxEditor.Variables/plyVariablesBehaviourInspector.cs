using BloxEngine;
using BloxEngine.Variables;
using plyLibEditor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BloxEditor.Variables
{
	[CustomEditor(typeof(plyVariablesBehaviour), true)]
	public class plyVariablesBehaviourInspector : Editor
	{
		protected plyVariablesType varsType;

		protected plyVariablesBehaviour Target;

		protected plyVariablesEditor varEditor;

		protected plyReorderableList.Button[] extraButtons;

		protected virtual void OnEnable()
		{
			this.Target = (plyVariablesBehaviour)base.target;
			if (this.Target.variables == null)
			{
				this.Target.variables = new plyVariables();
				this.Save();
			}
			this.varEditor = new plyVariablesEditor(this.Target.variables, this.varsType, this.Target.gameObject, true, true, true, this.Repaint, this.Save, this.extraButtons);
		}

		public override void OnInspectorGUI()
		{
			this.varEditor.DoLayout();
		}

		private void Save()
		{
			plyEdUtil.SetDirty(this.Target);
			if (plyEdUtil.IsSceneObject(this.Target.gameObject))
			{
				EditorSceneManager.MarkSceneDirty(this.Target.gameObject.scene);
			}
		}
	}
}
