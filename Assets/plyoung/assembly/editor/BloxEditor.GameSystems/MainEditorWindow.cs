using plyLibEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class MainEditorWindow : EditorWindow
	{
		public static float PanelContentWidth = 450f;

		private static List<MainEdChild> editors = new List<MainEdChild>();

		private static GUIContent[] labels = new GUIContent[0];

		private static int edIdx = -1;

		private static string[] forceShow = null;

		public static void Show_MainEditorWindow()
		{
			BloxEdGlobal.CheckAllData();
			MainEditorWindow window = EditorWindow.GetWindow<MainEditorWindow>("BGS");
			Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.bgs" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(MainEditorWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			window.titleContent = new GUIContent("BGS", image);
		}

		public static void Show_MainEditorWindow(string showEditorWithIdent, string sendStringToEditor)
		{
			MainEditorWindow.forceShow = new string[2]
			{
				showEditorWithIdent,
				sendStringToEditor
			};
			MainEditorWindow.Show_MainEditorWindow();
		}

		protected void OnEnable()
		{
			Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.bgs" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(MainEditorWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			base.titleContent = new GUIContent("BGS", image);
		}

		protected void OnFocus()
		{
		}

		protected void OnGUI()
		{
			if (MainEditorWindow.editors.Count == 0)
			{
				this.LoadEditors();
			}
			if (MainEditorWindow.forceShow != null)
			{
				this.ForceShowEditor();
			}
			if (plyEdGUI.Tabbar(ref MainEditorWindow.edIdx, MainEditorWindow.labels))
			{
				MainEditorWindow.editors[MainEditorWindow.edIdx].editorWindow = this;
				MainEditorWindow.editors[MainEditorWindow.edIdx].OnFocus();
			}
			MainEditorWindow.editors[MainEditorWindow.edIdx].editorWindow = this;
			MainEditorWindow.editors[MainEditorWindow.edIdx].OnGUI();
		}

		private void LoadEditors()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] exportedTypes = assemblies[i].GetExportedTypes();
				for (int j = 0; j < exportedTypes.Length; j++)
				{
					Type type = exportedTypes[j];
					if (type.IsClass && typeof(MainEdChild).IsAssignableFrom(type) && type.Name != "MainEdChild")
					{
						MainEdChild item = (MainEdChild)Activator.CreateInstance(type);
						MainEditorWindow.editors.Add(item);
					}
				}
			}
			MainEditorWindow.editors.Sort((MainEdChild a, MainEdChild b) => a.order.CompareTo(b.order));
			MainEditorWindow.labels = new GUIContent[MainEditorWindow.editors.Count];
			for (int k = 0; k < MainEditorWindow.editors.Count; k++)
			{
				MainEditorWindow.labels[k] = new GUIContent(MainEditorWindow.editors[k].label);
			}
			MainEditorWindow.edIdx = 0;
			MainEditorWindow.editors[MainEditorWindow.edIdx].editorWindow = this;
			MainEditorWindow.editors[MainEditorWindow.edIdx].OnFocus();
		}

		private void ForceShowEditor()
		{
			int num = 0;
			while (num < MainEditorWindow.editors.Count)
			{
				if (!(MainEditorWindow.editors[num].ident == MainEditorWindow.forceShow[0]))
				{
					num++;
					continue;
				}
				MainEditorWindow.edIdx = num;
				MainEditorWindow.editors[MainEditorWindow.edIdx].editorWindow = this;
				MainEditorWindow.editors[MainEditorWindow.edIdx].OnFocus();
				MainEditorWindow.editors[MainEditorWindow.edIdx].OnForcedShow(MainEditorWindow.forceShow[1]);
				break;
			}
			MainEditorWindow.forceShow = null;
		}
	}
}
