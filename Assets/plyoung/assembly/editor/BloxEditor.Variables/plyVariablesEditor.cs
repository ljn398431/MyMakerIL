using BloxEngine;
using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	public class plyVariablesEditor
	{
		public class DragDropData
		{
			public string name;

			public plyVariablesType plyVarType;

			public Type varValType;
		}

		public GUIContent GC_Head = new GUIContent("Variables");

		public string nameString = "Variable";

		public bool doFlexiSpace;

		public bool canStartDragDrop = true;

		private static GUIContent GC_VarName = new GUIContent();

		private static GUIContent GC_VarType = new GUIContent();

		private bool forceParamNames;

		private plyVariables variables;

		private plyVariablesType varsType;

		private GameObject variablesOwner;

		private bool isOnSceneObject;

		private bool canEditValues;

		private bool canEditVariables;

		private Action repaint;

		private Action save;

		private plyReorderableList varsList;

		private plyVar advVar;

		private plyVarEd advEd;

		public static Dictionary<Type, plyVarEd> editors = null;

		public static GUIContent[] edLabels = new GUIContent[0];

		public int SelectedIdx
		{
			get
			{
				if (this.varsList != null)
				{
					return this.varsList.index;
				}
				return -1;
			}
		}

		public static void LoadVarEds()
		{
			if (plyVariablesEditor.editors == null)
			{
				List<plyVarEd> list = plyCustomEd.CreateCustomEditorsList<plyVarEd>(typeof(plyVarEdAttribute));
				list.Sort((plyVarEd a, plyVarEd b) => ((plyVarEdAttribute)a.nfo).Order.CompareTo(((plyVarEdAttribute)b.nfo).Order));
				plyVariablesEditor.editors = new Dictionary<Type, plyVarEd>();
				plyVariablesEditor.edLabels = new GUIContent[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					plyVariablesEditor.edLabels[i] = new GUIContent(((plyVarEdAttribute)list[i].nfo).VarTypeName);
					plyVariablesEditor.editors.Add(list[i].nfo.TargetType, list[i]);
				}
			}
		}

		public plyVariablesEditor(plyVariables variables, plyVariablesType varsType, GameObject variablesOwner, bool canEditValues, bool canEditVariables, bool displayHeader, Action repaint, Action save, plyReorderableList.Button[] extraButtons)
		{
			plyVariablesEditor.LoadVarEds();
			this.variables = variables;
			this.varsType = varsType;
			this.variablesOwner = variablesOwner;
			this.isOnSceneObject = (!((UnityEngine.Object)variablesOwner == (UnityEngine.Object)null) && plyEdUtil.IsSceneObject(variablesOwner));
			this.canEditValues = canEditValues;
			this.canEditVariables = canEditVariables;
			this.repaint = repaint;
			this.save = save;
			if (extraButtons == null)
			{
				extraButtons = new plyReorderableList.Button[0];
			}
			if (canEditVariables)
			{
				ArrayUtility.Add(ref extraButtons, new plyReorderableList.Button
				{
					label = new GUIContent(Ico._rename, "Rename selected " + this.nameString),
					callback = this.OnRenameButton,
					requireSelected = true
				});
			}
			if (canEditVariables)
			{
				this.varsList = new plyReorderableList((variables != null) ? variables.varDefs : null, typeof(plyVar), false, true, true, true, false, false, false, extraButtons, null);
				this.varsList.elementHeight = (float)((float)((!canEditValues) ? 1 : 2) * (EditorGUIUtility.singleLineHeight + 2.0) + 4.0);
				this.varsList.drawHeaderCallback = this.DrawListHeader;
				this.varsList.drawElementCallback = this.DrawElement;
				this.varsList.onAddElement = this.OnAdd;
				this.varsList.onRemoveElement = this.OnRemove;
				this.varsList.onReorder = null;
			}
			else
			{
				this.varsList = new plyReorderableList((variables != null) ? variables.varDefs : null, typeof(plyVar), false, displayHeader, false, false, false, false, false, extraButtons, null);
				this.varsList.elementHeight = (float)((float)((!canEditValues) ? 1 : 2) * (EditorGUIUtility.singleLineHeight + 2.0) + 4.0);
				if (displayHeader)
				{
					this.varsList.drawHeaderCallback = this.DrawListHeader;
				}
				this.varsList.drawElementCallback = this.DrawElement;
				this.varsList.onAddElement = null;
				this.varsList.onRemoveElement = null;
				this.varsList.onReorder = null;
			}
		}

		public void SetTarget(plyVariables variables, GameObject variablesOwner)
		{
			if (this.variables == variables && (UnityEngine.Object)this.variablesOwner == (UnityEngine.Object)variablesOwner)
				return;
			this.variables = variables;
			this.variablesOwner = variablesOwner;
			this.isOnSceneObject = (!((UnityEngine.Object)variablesOwner == (UnityEngine.Object)null) && plyEdUtil.IsSceneObject(variablesOwner));
			this.varsList.list = ((variables != null) ? variables.varDefs : null);
		}

		public void DoLayout()
		{
			if (this.variables == null)
			{
				if (this.doFlexiSpace)
				{
					GUILayout.FlexibleSpace();
				}
				else
				{
					GUILayout.Space(5f);
				}
			}
			else
			{
				if (this.advVar != null && this.advEd != null)
				{
					if (this.advEd.DrawAdvancedEditor(this.advVar, this.isOnSceneObject, this.Save))
					{
						this.advVar = null;
						this.advEd = null;
						plyEdGUI.ClearFocus();
					}
				}
				else
				{
					this.varsList.DoLayoutList();
				}
				if (this.doFlexiSpace)
				{
					GUILayout.FlexibleSpace();
				}
			}
		}

		public void VariableWasAdded()
		{
			this.advVar = null;
			this.advEd = null;
		}

		public void ForceParamNames()
		{
			this.forceParamNames = true;
			this.varsList.RemoveButton(this.OnRenameButton);
		}

		private void DrawListHeader(Rect rect)
		{
			GUI.Label(rect, this.GC_Head);
		}

		private void OnRenameButton()
		{
			plyTextInputWiz.ShowWiz("Rename " + this.nameString, "Enter a unique name", this.variables.varDefs[this.varsList.index].name, this.OnRenameVariable, null, 250f);
		}

		private void OnRenameVariable(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text) && !text.Equals(this.variables.varDefs[this.varsList.index].name))
			{
				if (plyEdUtil.StringIsUnique(this.variables.varDefs, text))
				{
					this.variables.varDefs[this.varsList.index].name = text;
					this.Save();
				}
				else
				{
					EditorUtility.DisplayDialog(this.nameString, "The " + this.nameString + " name must be unique.", "OK");
				}
			}
			this.repaint();
		}

		private void OnAdd()
		{
			if (this.forceParamNames)
			{
				string forceName = "param" + this.variables.varDefs.Count;
				plyVarCreateWiz.ShowWiz(this.CreateVariable, this.nameString, forceName);
			}
			else
			{
				plyVarCreateWiz.ShowWiz(this.CreateVariable, this.nameString, null);
			}
		}

		private void CreateVariable(plyVarCreateWiz wiz)
		{
			plyVar var = wiz.var;
			wiz.Close();
			this.VariableWasAdded();
			if (!string.IsNullOrEmpty(var.name))
			{
				if (plyEdUtil.StringIsUnique(this.variables.varDefs, var.name))
				{
					var.ident = this.variables.CreateVariableIdent();
					this.variables.varDefs.Add(var);
					this.Save();
					plyEdUtil.ApplyPrefabInstanceChanges(this.variablesOwner);
				}
				else
				{
					EditorUtility.DisplayDialog(this.nameString, "The " + this.nameString + " name must be unique.", "OK");
				}
			}
			this.repaint();
		}

		private void OnRemove()
		{
			if (this.varsList.index >= 0 && this.varsList.index < this.variables.varDefs.Count)
			{
				this.variables.varDefs.RemoveAt(this.varsList.index);
				if (this.forceParamNames)
				{
					for (int i = 0; i < this.variables.varDefs.Count; i++)
					{
						this.variables.varDefs[i].name = "param" + i;
					}
				}
				this.Save();
				plyEdUtil.ApplyPrefabInstanceChanges(this.variablesOwner);
			}
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			plyVar plyVar = this.variables.varDefs[index];
			plyVarEd plyVarEd = plyVariablesEditor.editors[plyVar.ValueHandler.GetType()];
			plyVariablesEditor.GC_VarName.text = plyVar.name;
			plyVariablesEditor.GC_VarType.text = plyVarEd.VarTypeName(plyVar);
			float num = (float)(plyEdGUI.Styles.Label_RT.CalcSize(plyVariablesEditor.GC_VarType).x + 7.0);
			Rect rect2 = rect;
			rect2.height = EditorGUIUtility.singleLineHeight;
			rect2.y += 3f;
			rect2.width -= num;
			GUI.Label(rect2, plyVariablesEditor.GC_VarName, EditorStyles.boldLabel);
			rect2.x = (float)(rect2.xMax + 5.0);
			rect2.width = num;
			GUI.Label(rect2, plyVariablesEditor.GC_VarType);
			rect2.y += (float)(rect2.height + 2.0);
			rect2.x = (float)(rect.x + 20.0);
			rect2.width = (float)(rect.width - 20.0);
			EditorGUI.BeginChangeCheck();
			if (plyVarEd.DrawEditor(rect2, this.isOnSceneObject, plyVar, plyVar, 0))
			{
				this.advVar = plyVar;
				this.advEd = plyVarEd;
				plyEdGUI.ClearFocus();
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.Save();
			}
			if (this.canStartDragDrop && Event.current.type == EventType.MouseDrag && rect.Contains(Event.current.mousePosition))
			{
				plyEdGUI.ClearFocus();
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new UnityEngine.Object[0];
				DragAndDrop.paths = null;
				DragAndDrop.SetGenericData("plyVariable", new DragDropData
				{
					name = plyVar.name,
					plyVarType = this.varsType,
					varValType = plyVar.variableType
				});
				DragAndDrop.StartDrag(plyVar.name);
				Event.current.Use();
			}
		}

		private void Save()
		{
			this.variables._SetDirty();
			this.save();
		}
	}
}
