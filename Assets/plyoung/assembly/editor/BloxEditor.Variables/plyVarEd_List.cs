using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_List), "List", UsesAdvancedEditor = true, Order = 5)]
	public class plyVarEd_List : plyVarEd
	{
		private class MenuData
		{
			public Type baseType;

			public Type varType;
		}

		private static GUIContent GC_Button = new GUIContent("");

		private static GUIContent GC_Head = new GUIContent("List");

		private static GUIContent GC_Element = new GUIContent("0");

		private static GenericMenu menu = null;

		private plyReorderableList list;

		private plyVar targetVar;

		private plyVar_List targetHandler;

		private bool doCloseStandalone;

		private Action saveCallback;

		private bool isOnSceneObject;

		public override string VarTypeName(plyVar target)
		{
			plyVar_List plyVar_List = (plyVar_List)target.ValueHandler;
			return "List<" + BloxEd.PrettyTypeName(plyVar_List.baseType, false) + ">";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(List<>)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("List")
			};
		}

		public override bool DrawCreateWizard(plyVar target)
		{
			this.targetVar = target;
			this.targetHandler = (plyVar_List)target.ValueHandler;
			if (plyVarEd_List.menu == null)
			{
				List<plyVarEd> list = new List<plyVarEd>();
				foreach (plyVarEd value in plyVariablesEditor.editors.Values)
				{
					if (!((plyVarEdAttribute)value.nfo).UsesAdvancedEditor)
					{
						list.Add(value);
					}
				}
				plyVarEd_List.menu = new GenericMenu();
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].handleTypes.Count == 1)
					{
						string varTypeName = ((plyVarEdAttribute)list[i].nfo).VarTypeName;
						MenuData userData = new MenuData
						{
							varType = ((plyVarEdAttribute)list[i].nfo).TargetType,
							baseType = list[i].handleTypes[0]
						};
						plyVarEd_List.menu.AddItem(new GUIContent(varTypeName), false, this.OnTypeSelected, userData);
					}
					else
					{
						for (int j = 0; j < list[i].handleTypeNames.Length; j++)
						{
							string text = ((plyVarEdAttribute)list[i].nfo).VarTypeName + "/" + list[i].handleTypeNames[j].text;
							MenuData userData2 = new MenuData
							{
								varType = ((plyVarEdAttribute)list[i].nfo).TargetType,
								baseType = list[i].handleTypes[j]
							};
							plyVarEd_List.menu.AddItem(new GUIContent(text), false, this.OnTypeSelected, userData2);
						}
					}
				}
			}
			if (GUILayout.Button(BloxEd.PrettyTypeName(this.targetHandler.baseType, false)))
			{
				plyVarEd_List.menu.ShowAsContext();
			}
			return this.targetHandler.baseType != null;
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_List plyVar_List = (plyVar_List)target.ValueHandler;
			this.targetVar = target;
			this.targetHandler = plyVar_List;
			this.isOnSceneObject = isOnSceneObject;
			bool result = false;
			plyVarEd_List.GC_Button.text = "[" + plyVar_List.variables.Count + "] click to edit list";
			if (GUI.Button(rect, plyVarEd_List.GC_Button, GUI.skin.label))
			{
				result = true;
				plyVarEd_List.GC_Head.text = "List: <b>" + target.name + "</b>";
			}
			return result;
		}

		public override bool DrawAdvancedEditor(plyVar target, bool isOnSceneObject, Action saveCallback)
		{
			this.doCloseStandalone = false;
			this.isOnSceneObject = isOnSceneObject;
			this.saveCallback = saveCallback;
			this.targetVar = target;
			this.targetHandler = (plyVar_List)target.ValueHandler;
			if (this.list == null)
			{
				this.list = new plyReorderableList(null, typeof(plyVar), false, true, true, true, true, false, false, new plyReorderableList.Button[1]
				{
					new plyReorderableList.Button
					{
						label = new GUIContent(Ico._arrow_up, "Back to variables list"),
						callback = delegate
						{
							this.doCloseStandalone = true;
						}
					}
				}, null);
				this.list.elementHeight = (float)(EditorGUIUtility.singleLineHeight + 2.0);
				this.list.drawHeaderCallback = this.DrawListHeader;
				this.list.drawElementCallback = this.DrawElement;
				this.list.onAddElement = this.OnAdd;
				this.list.onRemoveElement = this.OnRemove;
			}
			this.list.list = this.targetHandler.variables;
			this.list.DoLayoutList();
			return this.doCloseStandalone;
		}

		private void DrawListHeader(Rect rect)
		{
			GUI.Label(rect, plyVarEd_List.GC_Head, plyEdGUI.Styles.Label_RT);
		}

		private void OnAdd()
		{
			plyVar plyVar = plyVar.Create(this.targetHandler.plyVarType);
			this.targetHandler.variables.Add(plyVar);
			this.targetHandler.UpdateVarNames();
			if (typeof(UnityEngine.Object).IsAssignableFrom(this.targetHandler.baseType))
			{
				plyVar.ValueHandler.SetStoredType(this.targetHandler.baseType);
				if (this.targetVar.objRefs == null)
				{
					this.targetVar.objRefs = new UnityEngine.Object[0];
				}
				ArrayUtility.Add<UnityEngine.Object>(ref this.targetVar.objRefs, (UnityEngine.Object)null);
			}
			this.saveCallback();
		}

		private void OnTypeSelected(object userData)
		{
			MenuData menuData = (MenuData)userData;
			this.targetHandler.SetStoredType(menuData.baseType, menuData.varType);
		}

		private void OnRemove()
		{
			int index = this.list.index;
			this.targetHandler.variables.RemoveAt(index);
			if (typeof(UnityEngine.Object).IsAssignableFrom(this.targetHandler.baseType) && this.targetVar.objRefs != null && this.targetVar.objRefs.Length > index)
			{
				ArrayUtility.RemoveAt<UnityEngine.Object>(ref this.targetVar.objRefs, index);
			}
			this.saveCallback();
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			plyVar plyVar = this.targetHandler.variables[index];
			if (plyVar != null && plyVar.ValueHandler != null)
			{
				plyVarEd plyVarEd = plyVariablesEditor.editors[plyVar.ValueHandler.GetType()];
				plyVarEd_List.GC_Element.text = this.targetHandler.variables.Count.ToString();
				float num = (float)(plyEdGUI.Styles.Label_RT.CalcSize(plyVarEd_List.GC_Element).x + 5.0);
				Rect rect2 = rect;
				rect2.width = num;
				plyVarEd_List.GC_Element.text = index.ToString();
				GUI.Label(rect2, plyVarEd_List.GC_Element, plyEdGUI.Styles.Label_RT);
				rect2.x = rect2.xMax;
				rect2.width = rect.width - num;
				EditorGUI.BeginChangeCheck();
				plyVarEd.DrawEditor(rect2, this.isOnSceneObject, plyVar, this.targetVar, index);
				if (EditorGUI.EndChangeCheck())
				{
					this.saveCallback();
				}
			}
		}
	}
}
