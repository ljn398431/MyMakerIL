using BloxEngine.Variables;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	public class plyVarCreateWiz : EditorWindow
	{
		private int curr = -1;

		private Action<plyVarCreateWiz> callback;

		private bool accepted;

		private bool lostFocus;

		private bool valid = true;

		private string forceName;

		private static readonly GUIContent GC_Name = new GUIContent("Name");

		private static readonly GUIContent GC_Type = new GUIContent("Type");

		public plyVar var
		{
			get;
			private set;
		}

		public static void ShowWiz(Action<plyVarCreateWiz> callback, string nameString, string forceName = null)
		{
			plyVariablesEditor.LoadVarEds();
			plyVarCreateWiz window = EditorWindow.GetWindow<plyVarCreateWiz>(true, "Create " + nameString, true);
			window.callback = callback;
			window.curr = -1;
			window.forceName = forceName;
			window.minSize = new Vector2(250f, 100f);
			window.ShowUtility();
		}

		private void OnFocus()
		{
			this.lostFocus = false;
		}

		private void OnLostFocus()
		{
			this.lostFocus = true;
		}

		private void Update()
		{
			if (this.lostFocus)
			{
				base.Close();
			}
			if (this.accepted && this.callback != null)
			{
				this.callback(this);
			}
		}

		private void OnGUI()
		{
			if (this.curr < 0)
			{
				int num = 0;
				while (num < plyVariablesEditor.edLabels.Length)
				{
					if (!(plyVariablesEditor.edLabels[num].text == "String"))
					{
						num++;
						continue;
					}
					this.curr = num;
					break;
				}
				if (this.curr < 0)
				{
					this.curr = 0;
				}
				this.CreateVar();
			}
			EditorGUIUtility.labelWidth = 70f;
			EditorGUILayout.Space();
			GUI.enabled = (this.forceName == null);
			this.var.name = EditorGUILayout.TextField(plyVarCreateWiz.GC_Name, this.var.name);
			GUI.enabled = true;
			EditorGUI.BeginChangeCheck();
			this.curr = EditorGUILayout.Popup(plyVarCreateWiz.GC_Type, this.curr, plyVariablesEditor.edLabels);
			if (EditorGUI.EndChangeCheck())
			{
				this.CreateVar();
			}
			this.valid = plyVariablesEditor.editors[this.var.ValueHandler.GetType()].DrawCreateWizard(this.var);
			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginHorizontal(plyEdGUI.Styles.BottomBar);
			GUILayout.FlexibleSpace();
			GUI.enabled = (this.valid && !string.IsNullOrEmpty(this.var.name));
			if (GUILayout.Button("Accept", GUILayout.Width(80f)))
			{
				this.accepted = true;
			}
			GUI.enabled = true;
			GUILayout.Space(5f);
			if (GUILayout.Button("Cancel", GUILayout.Width(80f)))
			{
				base.Close();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private void CreateVar()
		{
			string name = (this.var == null) ? "" : this.var.name;
			if (this.forceName != null)
			{
				name = this.forceName;
			}
			string text = plyVariablesEditor.edLabels[this.curr].text;
			Type t = null;
			foreach (plyVarEd value in plyVariablesEditor.editors.Values)
			{
				if (((plyVarEdAttribute)value.nfo).VarTypeName.Equals(text))
				{
					t = value.nfo.TargetType;
					break;
				}
			}
			this.var = plyVar.Create(t);
			this.var.name = name;
		}
	}
}
