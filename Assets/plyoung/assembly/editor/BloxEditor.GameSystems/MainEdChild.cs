using UnityEditor;

namespace BloxEditor.GameSystems
{
	public abstract class MainEdChild
	{
		public EditorWindow editorWindow;

		public abstract int order
		{
			get;
		}

		public abstract string label
		{
			get;
		}

		public abstract string ident
		{
			get;
		}

		public abstract void OnFocus();

		public abstract void OnGUI();

		public abstract void OnForcedShow(string infoString);
	}
}
