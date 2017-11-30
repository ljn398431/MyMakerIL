using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(WindowManager))]
	public class WindowManagerInspector : Editor
	{
		private static readonly GUIContent GC_Head = new GUIContent("Windows");

		private plyReorderableList windowList;

		private WindowManager manager;

		private SerializedProperty p_onWindowShown;

		private SerializedProperty p_onWindowHidden;

		protected void OnEnable()
		{
			this.manager = (WindowManager)base.target;
			this.p_onWindowShown = base.serializedObject.FindProperty("onWindowShown");
			this.p_onWindowHidden = base.serializedObject.FindProperty("onWindowHidden");
		}

		public override void OnInspectorGUI()
		{
			this.manager = (WindowManager)base.target;
			if (this.windowList == null)
			{
				this.windowList = new plyReorderableList(this.manager.windows, typeof(GameObject), true, true, true, true, false, false, false, null, null);
				this.windowList.elementHeight = (float)(EditorGUIUtility.singleLineHeight + 6.0);
				this.windowList.drawHeaderCallback = this.DrawListHeader;
				this.windowList.drawElementCallback = this.DrawElement;
				this.windowList.onAddElement = this.OnAdd;
				this.windowList.onRemoveElement = this.OnRemove;
				this.windowList.onReorder = this.OnReorder;
			}
			EditorGUILayout.Space();
			this.windowList.DoLayoutList();
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.p_onWindowShown);
			EditorGUILayout.PropertyField(this.p_onWindowHidden);
			base.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Space();
			if (GUI.changed)
			{
				plyEdUtil.SetDirty(this.manager);
				GUI.changed = false;
			}
		}

		private void DrawListHeader(Rect rect)
		{
			GUI.Label(rect, WindowManagerInspector.GC_Head);
		}

		private void OnAdd()
		{
			this.manager.windows.Add(null);
			plyEdUtil.SetDirty(this.manager);
		}

		private void OnRemove()
		{
			this.manager.windows.RemoveAt(this.windowList.index);
			plyEdUtil.SetDirty(this.manager);
		}

		private void OnReorder()
		{
			plyEdUtil.SetDirty(this.manager);
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			Rect position = rect;
			position.y += 2f;
			position.height = EditorGUIUtility.singleLineHeight;
			this.manager.windows[index] = (GameObject)EditorGUI.ObjectField(position, this.manager.windows[index], typeof(GameObject), true);
		}
	}
}
