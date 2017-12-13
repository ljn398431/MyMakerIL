using BloxEditor.Variables;
using BloxEngine;
using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[CustomEditor(typeof(BloxContainer))]
	public class BloxContainerInspector : Editor
	{
		private static readonly GUIContent GC_Head = new GUIContent("Attached Blox");

		private static readonly GUIContent GC_VarsHead = new GUIContent("Blox Variables");

		private BloxContainer Target;

		private List<Blox> targetBloxCache = new List<Blox>();

		private plyReorderableList list;

		private bool deletedDetected;

		private Blox dragDropBlox;

		private plyVariablesEditor varEditor;

		[NonSerialized]
		private Blox selectedBlox;

		[NonSerialized]
		private BloxVariables activeVars;

		public static BloxContainerInspector Instance
		{
			get;
			private set;
		}

		protected void OnEnable()
		{
			BloxContainerInspector.Instance = this;
			this.Target = (BloxContainer)base.target;
			if ((UnityEngine.Object)this.Target.bloxGlobalPrefab != (UnityEngine.Object)BloxEd.BloxGlobalPrefab)
			{
				this.Target.bloxGlobalPrefab = BloxEd.BloxGlobalPrefab;
				this.Save();
			}
			this.CacheBloxList(null, null);
			plyReorderableList.Button[] extraButtons = new plyReorderableList.Button[3]
			{
				new plyReorderableList.Button
				{
					label = new GUIContent(Ico._settings, "Open Blox settings"),
					callback = BloxSettingsWindow.Show_BloxSettingsWindow
				},
				new plyReorderableList.Button
				{
					label = new GUIContent(Ico._blox + "Edit", "Edit selected Blox"),
					callback = this.OnDoubleClick,
					requireSelected = true
				},
				new plyReorderableList.Button
				{
					label = new GUIContent(Ico._rename, "Rename selected Blox"),
					callback = this.OnRenameBlox,
					requireSelected = true
				}
			};
			plyReorderableList.Button[] addOptions = new plyReorderableList.Button[2]
			{
				new plyReorderableList.Button
				{
					label = new GUIContent("Create new"),
					callback = this.AddNewBloxDef
				},
				new plyReorderableList.Button
				{
					label = new GUIContent("Add Existing"),
					callback = this.AddExistingBloxDef
				}
			};
			this.list = new plyReorderableList(this.targetBloxCache, typeof(Blox), true, true, true, true, false, false, false, extraButtons, addOptions);
			this.list.elementHeight = (float)(EditorGUIUtility.singleLineHeight + 2.0 + 4.0);
			this.list.drawHeaderCallback = this.DrawListHeader;
			this.list.drawElementCallback = this.DrawListElement;
			this.list.onSelectElement = this.OnSelectBlox;
			this.list.onDoubleClickElement = this.OnDoubleClick;
			this.list.onRemoveElement = this.OnRemove;
			this.list.onReorder = this.OnReorder;
			this.varEditor = new plyVariablesEditor(null, plyVariablesType.Blox, this.Target.gameObject, true, false, true, this.Repaint, this.Save, null);
			this.varEditor.GC_Head = BloxContainerInspector.GC_VarsHead;
			if (this.targetBloxCache.Count > 0)
			{
				this.list.index = 0;
				this.OnSelectBlox();
			}
		}

		protected void OnDisable()
		{
			BloxContainerInspector.Instance = null;
			this.Target = null;
			this.targetBloxCache.Clear();
		}

		protected void OnDestroy()
		{
			this.OnDisable();
		}

		private void CacheBloxList(string bloxIdent, object[] args)
		{
			this.targetBloxCache.Clear();
			if (this.Target.bloxIdents.Count != 0)
			{
				bool flag = false;
				for (int num = this.Target.bloxIdents.Count - 1; num >= 0; num--)
				{
					Blox blox = BloxEd.BloxGlobalObj.FindBloxDef(this.Target.bloxIdents[num]);
					if ((UnityEngine.Object)blox == (UnityEngine.Object)null)
					{
						flag = true;
						this.Target._RemoveBloxVariables(this.Target.bloxIdents[num]);
						this.Target.bloxIdents.RemoveAt(num);
					}
					else
					{
						this.targetBloxCache.Insert(0, blox);
					}
				}
				if (flag)
				{
					this.Save();
				}
			}
		}

		public override void OnInspectorGUI()
		{
            base.OnInspectorGUI();
			this.Target = (BloxContainer)base.target;
			EditorGUILayout.Space();
			this.list.DoLayoutList();
			if (this.activeVars != null)
			{
				this.varEditor.DoLayout();
			}
			if (this.deletedDetected && Event.current.type == EventType.Repaint)
			{
				this.deletedDetected = false;
				this.CacheBloxList(null, null);
			}
			Event current = Event.current;
			switch (current.type)
			{
			case EventType.DragExited:
				this.dragDropBlox = null;
				current.Use();
				break;
			case EventType.DragPerform:
				if ((UnityEngine.Object)this.dragDropBlox != (UnityEngine.Object)null)
				{
					DragAndDrop.AcceptDrag();
					plyEdGUI.ClearFocus();
					current.Use();
					if (!this.Target.bloxIdents.Contains(this.dragDropBlox.ident))
					{
						if (!this.targetBloxCache.Contains(this.dragDropBlox))
						{
							this.targetBloxCache.Add(this.dragDropBlox);
						}
						this.Target.bloxIdents.Add(this.dragDropBlox.ident);
						this.Save();
					}
					this.dragDropBlox = null;
				}
				break;
			case EventType.DragUpdated:
				this.dragDropBlox = (DragAndDrop.GetGenericData("BloxDefinition") as Blox);
				if ((UnityEngine.Object)this.dragDropBlox != (UnityEngine.Object)null)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Move;
					current.Use();
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
				break;
			}
		}

		public void OnBloxVariablesChanged(string bloxIdent)
		{
			if (!((UnityEngine.Object)this.selectedBlox == (UnityEngine.Object)null) && !(this.selectedBlox.ident != bloxIdent))
			{
				this.OnSelectBlox();
			}
		}

		private void AddNewBloxDef()
		{
			Blox blox = BloxEd.CreateNewBloxDef();
			this.Target.bloxIdents.Add(blox.ident);
			this.targetBloxCache.Add(blox);
			this.Save();
			BloxListWindow instance = BloxListWindow.Instance;
			if ((object)instance != null)
			{
				instance.Repaint();
			}
		}

		private void AddExistingBloxDef()
		{
			BloxListPopup.Show_BloxListPopup(this.Target, this.CacheBloxList, null);
		}

		private void DrawListHeader(Rect rect)
		{
			GUI.Label(rect, BloxContainerInspector.GC_Head);
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if ((UnityEngine.Object)this.targetBloxCache[index] == (UnityEngine.Object)null)
			{
				this.deletedDetected = true;
			}
			else if (Event.current.type == EventType.Repaint)
			{
				rect.y += 2f;
				plyEdGUI.Styles.RL_Element.Draw(rect, this.targetBloxCache[index].screenName, false, isActive, isActive, isFocused);
			}
		}

		private void OnRemove()
		{
			if (this.list.index >= 0 && this.list.index < this.targetBloxCache.Count)
			{
				this.varEditor.SetTarget(null, this.Target.gameObject);
				this.Target._RemoveBloxVariables(this.Target.bloxIdents[this.list.index]);
				this.Target.bloxIdents.RemoveAt(this.list.index);
				this.targetBloxCache.RemoveAt(this.list.index);
				this.list.index = -1;
				this.Save();
			}
		}

		private void OnReorder()
		{
			this.Target._ClearBloxVarCache();
			this.Target.bloxIdents.Clear();
			for (int i = 0; i < this.targetBloxCache.Count; i++)
			{
				this.Target.bloxIdents.Add(this.targetBloxCache[i].ident);
			}
			this.Save();
		}

		private void OnSelectBlox()
		{
			this.activeVars = null;
			this.selectedBlox = this.targetBloxCache[this.list.index];
			if ((UnityEngine.Object)this.selectedBlox != (UnityEngine.Object)null)
			{
				if (this.selectedBlox.variables.varDefs.Count == 0)
				{
					this.selectedBlox.variables.Serialize();
					this.selectedBlox.variables.Deserialize(false);
				}
				BloxContainerInspector.GC_VarsHead.text = "Variables of: " + this.selectedBlox.screenName;
				this.activeVars = this.Target.GetBloxVariables(this.selectedBlox.ident, this.selectedBlox);
				this.activeVars._SetDirty();
				this.Save();
				this.varEditor.SetTarget(this.activeVars, this.Target.gameObject);
				base.Repaint();
			}
			if (!BloxEdGlobal.DoubleClickOpenBloxDef && this.list.index >= 0 && this.list.index < this.targetBloxCache.Count)
			{
				BloxEditorWindow.Show_BloxEditorWindow(this.targetBloxCache[this.list.index]);
			}
		}

		private void OnDoubleClick()
		{
			if (this.list.index >= 0 && this.list.index < this.targetBloxCache.Count)
			{
				BloxEditorWindow.Show_BloxEditorWindow(this.targetBloxCache[this.list.index]);
			}
		}

		private void OnRenameBlox()
		{
			if (this.list.index >= 0 && this.list.index < this.targetBloxCache.Count && !((UnityEngine.Object)this.targetBloxCache[this.list.index] == (UnityEngine.Object)null))
			{
				Blox _ = this.targetBloxCache[this.list.index];
				plyTextInputWiz.ShowWiz("Rename Blox", "", this.targetBloxCache[this.list.index].screenName, this.OnRenameBloxDef, null, 250f);
			}
		}

		private void OnRenameBloxDef(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text))
			{
				this.targetBloxCache[this.list.index].screenName = text;
				plyEdUtil.SetDirty(this.targetBloxCache[this.list.index]);
				BloxEd.SortBloxDefList();
				base.Repaint();
				BloxListWindow instance = BloxListWindow.Instance;
				if ((object)instance != null)
				{
					instance.Repaint();
				}
				BloxEditorWindow instance2 = BloxEditorWindow.Instance;
				if ((object)instance2 != null)
				{
					instance2.Repaint();
				}
			}
		}

		private void Save()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				plyEdUtil.SetDirty(this.Target);
			}
		}
	}
}
