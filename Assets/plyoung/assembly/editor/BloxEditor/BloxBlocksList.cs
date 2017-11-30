using BloxEditor.Variables;
using BloxEngine;
using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	public class BloxBlocksList
	{
		private static BloxBlocksList _instance;

		private EditorWindow ed;

		private Blox blox;

		private float width;

		private plyEdCoroutine loader;

		private int currArea;

		private Vector2 scroll = Vector2.zero;

		private plyEdTreeView<BloxBlockDef> treeView;

		private plyVariablesEditor varEditor;

		private int selected = -1;

		private static readonly GUIContent[] GC_Area = new GUIContent[2]
		{
			new GUIContent(Ico._blox, "Blocks"),
			new GUIContent(Ico._blox_variable, "Blox Variables")
		};

		private static readonly GUIContent GC_Undock = new GUIContent(Ico._undock, "Undock from Blox Editor window");

		private static readonly GUIContent GC_Loading = new GUIContent("loading");

		private static readonly GUIContent GC_EmptyMessage = new GUIContent("No Blox Variables defined");

		private static readonly GUIContent GC_VarsHeading = new GUIContent("Variables");

		private static readonly GUIContent[] GC_ListButtons = new GUIContent[3]
		{
			new GUIContent(Ico._add, "Add new Variable"),
			new GUIContent(Ico._remove, "Remove selected Variable"),
			new GUIContent(Ico._rename, "Rename selected element")
		};

		private static GUIContent GC_VarName = new GUIContent();

		private static GUIContent GC_VarType = new GUIContent();

		public static BloxBlocksList Instance
		{
			get
			{
				return BloxBlocksList._instance ?? BloxBlocksList.Create();
			}
		}

		public static bool HasInstance
		{
			get
			{
				return BloxBlocksList._instance != null;
			}
		}

		public bool IsBuildingList
		{
			get
			{
				return this.treeView == null;
			}
		}

		private static BloxBlocksList Create()
		{
			BloxBlocksList._instance = new BloxBlocksList();
			BloxBlocksList._instance.varEditor = new plyVariablesEditor(null, plyVariablesType.Blox, null, true, false, false, BloxBlocksList._instance.Repaint, BloxBlocksList._instance.Save, null);
			BloxBlocksList._instance.varEditor.doFlexiSpace = true;
			BloxBlocksList._instance.loader = plyEdCoroutine.Start(BloxBlocksList._instance.Load(), true);
			return BloxBlocksList._instance;
		}

		public static void OnThemeChanged()
		{
			if (BloxBlocksList._instance != null && BloxBlocksList._instance.treeView != null)
			{
				BloxBlocksList.ReloadBlockDefs();
			}
		}

		public static void OnSettingsChanged()
		{
			if (BloxBlocksList._instance != null && BloxBlocksList._instance.treeView != null)
			{
				BloxBlocksList._instance.treeView.drawMode = BloxEdGlobal.BlocksListMode;
				BloxBlocksList._instance.treeView.CollapseAll();
				EditorWindow obj = BloxBlocksList._instance.ed;
				if ((object)obj != null)
				{
					obj.Repaint();
				}
			}
		}

		public static void ReloadBlockDefs()
		{
			if (BloxBlocksList._instance != null)
			{
				plyEdCoroutine obj = BloxBlocksList._instance.loader;
				if (obj != null)
				{
					obj.Stop();
				}
				BloxBlocksList._instance.treeView = null;
				BloxBlocksList._instance.loader = plyEdCoroutine.Start(BloxBlocksList._instance.Load(), true);
				EditorWindow obj2 = BloxBlocksList._instance.ed;
				if ((object)obj2 != null)
				{
					obj2.Repaint();
				}
			}
		}

		public bool DoGUI(EditorWindow ed, float width)
		{
			bool result = false;
			this.ed = ed;
			this.width = width;
			EditorGUILayout.BeginVertical(GUILayout.Width(width));
			EditorGUILayout.BeginHorizontal(plyEdGUI.Styles.Toolbar);
			if (this.treeView == null)
			{
				GUILayout.FlexibleSpace();
			}
			else
			{
				if (GUILayout.Button(BloxBlocksList.GC_Area[this.currArea], plyEdGUI.Styles.ToolbarButton))
				{
					this.currArea = ((this.currArea == 0) ? 1 : 0);
				}
				if (this.currArea == 0)
				{
					GUILayout.Space(5f);
					this.treeView.SearchString = plyEdGUI.SearchField(this.treeView.SearchString, BloxEdGlobal.DelayedSearch);
					GUILayout.Space(5f);
				}
				else
				{
					this.DoBloxVariablesToolbar();
				}
			}
			if (BloxEdGlobal.BlocksListDocked && GUILayout.Button(BloxBlocksList.GC_Undock, plyEdGUI.Styles.ToolbarButton))
			{
				result = true;
			}
			EditorGUILayout.EndHorizontal();
			if (this.treeView == null)
			{
				EditorGUILayout.Space();
				plyEdGUI.DrawSpinner(BloxBlocksList.GC_Loading, true, true);
				ed.Repaint();
			}
			else
			{
				if (this.treeView.Initialize())
				{
					this.treeView.onMouseOverItem = this.OnMouseHoverItem;
					this.treeView.canMark = false;
					this.treeView.drawMode = BloxEdGlobal.BlocksListMode;
				}
				if (this.currArea == 0)
				{
					this.treeView.editorWindow = ed;
					this.treeView.DrawLayout();
				}
				else
				{
					this.DoBloxVariables();
				}
			}
			EditorGUILayout.EndVertical();
			if (this.treeView == null && Event.current.type == EventType.Repaint)
			{
				BloxEd.Instance.DoUpdate();
				this.loader.DoUpdate();
			}
			return result;
		}

		public void DoUpdate()
		{
			if (this.treeView == null && this.loader != null)
			{
				BloxEd.Instance.DoUpdate();
				this.loader.DoUpdate();
			}
		}

		private void OnMouseHoverItem(plyEdTreeItem<BloxBlockDef> item)
		{
			BloxPropsPanel.Instance.SetShownDef((item != null && item.data != null) ? item.data : null);
		}

		private void OnMouseDragItem(plyEdTreeItem<BloxBlockDef> item)
		{
		}

		private IEnumerator Load()
		{
			plyVariablesEditor.LoadVarEds();
			BloxEd.Instance.LoadBlockDefs(true);
			while (BloxEd.Instance.BlockDefsLoading)
			{
				yield return (object)null;
			}
			plyEdTreeItem<BloxBlockDef> treeRoot = new plyEdTreeItem<BloxBlockDef>
			{
				children = new List<plyEdTreeItem<BloxBlockDef>>()
			};
			int count = 0;
			int countBeforeYield = 50;
			Dictionary<string, BloxBlockDef>.ValueCollection.Enumerator enumerator = BloxEd.Instance.blockDefs.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BloxBlockDef current = enumerator.Current;
					string[] array = current.ident.Split('/');
					if (string.IsNullOrEmpty(current.name))
					{
						current.name = array[array.Length - 1];
					}
					if ((UnityEngine.Object)current.icon == (UnityEngine.Object)null)
					{
						if (current.ident.StartsWith("UnityEngine"))
						{
							current.icon = BloxEdGUI.Instance.unityIcon;
						}
						else
						{
							current.icon = BloxEdGUI.Instance.bloxIcon;
						}
					}
					plyEdTreeItem<BloxBlockDef> plyEdTreeItem = treeRoot;
					plyEdTreeItem<BloxBlockDef> plyEdTreeItem2;
					for (int i = 0; i < array.Length - 1; i++)
					{
						string text = array[i];
						bool flag = false;
						if (plyEdTreeItem.children == null)
						{
							plyEdTreeItem.children = new List<plyEdTreeItem<BloxBlockDef>>();
						}
						foreach (plyEdTreeItem<BloxBlockDef> child in plyEdTreeItem.children)
						{
							if (child.label == text)
							{
								flag = true;
								plyEdTreeItem = child;
								break;
							}
						}
						if (!flag)
						{
							plyEdTreeItem2 = new plyEdTreeItem<BloxBlockDef>
							{
								label = text,
								searchLabel = text,
								order = ((plyEdTreeItem == treeRoot) ? current.order : plyEdTreeItem.order)
							};
							if (plyEdTreeItem2.searchLabel.Contains("("))
							{
								plyEdTreeItem2.searchLabel = plyEdTreeItem2.searchLabel.Substring(0, plyEdTreeItem2.searchLabel.LastIndexOf('('));
							}
							else if (plyEdTreeItem2.searchLabel.Contains(":"))
							{
								plyEdTreeItem2.searchLabel = plyEdTreeItem2.searchLabel.Substring(0, plyEdTreeItem2.searchLabel.LastIndexOf(':'));
							}
							if (plyEdTreeItem == treeRoot)
							{
								plyEdTreeItem2.icon = BloxEdGUI.Instance.folderIcon;
							}
							plyEdTreeItem.AddChild(plyEdTreeItem2);
							plyEdTreeItem = plyEdTreeItem2;
						}
					}
					if (plyEdTreeItem.children == null)
					{
						plyEdTreeItem.children = new List<plyEdTreeItem<BloxBlockDef>>();
					}
					plyEdTreeItem.children.Add(plyEdTreeItem2 = new plyEdTreeItem<BloxBlockDef>
					{
						icon = current.icon,
						label = current.name,
						searchLabel = current.name,
						order = current.order,
						data = current
					});
					if (plyEdTreeItem2.searchLabel.Contains("("))
					{
						plyEdTreeItem2.searchLabel = plyEdTreeItem2.searchLabel.Substring(0, plyEdTreeItem2.searchLabel.LastIndexOf('('));
					}
					else if (plyEdTreeItem2.searchLabel.Contains(":"))
					{
						plyEdTreeItem2.searchLabel = plyEdTreeItem2.searchLabel.Substring(0, plyEdTreeItem2.searchLabel.LastIndexOf(':'));
					}
					Texture2D texture2D = current.icon;
					if ((UnityEngine.Object)texture2D == (UnityEngine.Object)BloxEdGUI.Instance.bloxFadedIcon)
					{
						texture2D = AssetPreview.GetMiniTypeThumbnail(current.returnType);
						if ((UnityEngine.Object)texture2D == (UnityEngine.Object)null || texture2D.name == "DefaultAsset Icon")
						{
							texture2D = ((!current.returnType.FullName.StartsWith("UnityEngine")) ? BloxEdGUI.Instance.bloxIcon : BloxEdGUI.Instance.unityIcon);
						}
					}
					plyEdTreeItem.SetIconRecusriveUp(texture2D);
					count++;
					if (count >= countBeforeYield)
					{
						count = 0;
						yield return (object)null;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			enumerator = default(Dictionary<string, BloxBlockDef>.ValueCollection.Enumerator);
			this.treeView = new plyEdTreeView<BloxBlockDef>(null, treeRoot, BloxEdGUI.Instance.folderIcon, "Blocks");
			this.treeView.Sort();
		}

		private void SortChildren(plyEdTreeItem<BloxBlockDef> item)
		{
			if (item.hasChildren)
			{
				item.children.Sort(delegate(plyEdTreeItem<BloxBlockDef> a, plyEdTreeItem<BloxBlockDef> b)
				{
					bool flag = a.data == null || a.hasChildren;
					bool flag2 = b.data == null || b.hasChildren;
					if (flag & flag2)
					{
						return a.label.CompareTo(b.label);
					}
					if (flag)
					{
						return 1;
					}
					if (flag2)
					{
						return -1;
					}
					if (a.data.order == b.data.order)
					{
						return a.label.CompareTo(b.label);
					}
					return a.data.order.CompareTo(b.data.order);
				});
				for (int i = 0; i < item.children.Count; i++)
				{
					this.SortChildren(item.children[i]);
				}
			}
		}

		private void DoBloxVariablesToolbar()
		{
			if ((UnityEngine.Object)BloxEditorWindow.Instance != (UnityEngine.Object)null && (UnityEngine.Object)BloxEditorWindow.Instance.blox != (UnityEngine.Object)null)
			{
				if ((UnityEngine.Object)this.blox != (UnityEngine.Object)BloxEditorWindow.Instance.blox)
				{
					this.blox = BloxEditorWindow.Instance.blox;
					this.varEditor.SetTarget(this.blox.variables, null);
				}
			}
			else if ((UnityEngine.Object)this.blox != (UnityEngine.Object)null)
			{
				this.blox = null;
				this.varEditor.SetTarget(null, null);
			}
			this.selected = this.varEditor.SelectedIdx;
			GUILayout.Label(BloxBlocksList.GC_VarsHeading);
			GUI.enabled = ((UnityEngine.Object)this.blox != (UnityEngine.Object)null);
			if (GUILayout.Button(BloxBlocksList.GC_ListButtons[0], plyEdGUI.Styles.ToolbarButton))
			{
				plyVarCreateWiz.ShowWiz(this.CreateVariable, "Variable", null);
			}
			GUI.enabled = (this.selected >= 0 && (UnityEngine.Object)this.blox != (UnityEngine.Object)null);
			if (GUILayout.Button(BloxBlocksList.GC_ListButtons[1], plyEdGUI.Styles.ToolbarButton) && EditorUtility.DisplayDialog("Blox Variables", "Removing the Variable can't be undone. Are you sure?", "Yes", "Cancel"))
			{
				this.blox.variables.varDefs.RemoveAt(this.selected);
				this.selected--;
				if (this.selected <= -1)
				{
					this.selected = ((this.blox.variables.varDefs.Count <= 0) ? (-1) : 0);
				}
				this.Save();
				BloxContainerInspector instance = BloxContainerInspector.Instance;
				if ((object)instance != null)
				{
					instance.OnBloxVariablesChanged(this.blox.ident);
				}
			}
			GUI.enabled = (this.selected >= 0 && (UnityEngine.Object)this.blox != (UnityEngine.Object)null);
			if (GUILayout.Button(BloxBlocksList.GC_ListButtons[2], plyEdGUI.Styles.ToolbarButton))
			{
				plyTextInputWiz.ShowWiz("Rename Variable", "Enter a unique name", this.blox.variables.varDefs[this.selected].name, this.OnRenameVariable, null, 250f);
			}
			GUI.enabled = true;
			GUILayout.FlexibleSpace();
		}

		private void DoBloxVariables()
		{
			if ((UnityEngine.Object)this.blox == (UnityEngine.Object)null)
			{
				GUILayout.FlexibleSpace();
			}
			else
			{
				this.scroll = EditorGUILayout.BeginScrollView(this.scroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.scrollView);
				this.varEditor.DoLayout();
				EditorGUILayout.EndScrollView();
			}
		}

		private void CreateVariable(plyVarCreateWiz wiz)
		{
			plyVar var = wiz.var;
			wiz.Close();
			this.varEditor.VariableWasAdded();
			if (!string.IsNullOrEmpty(var.name))
			{
				if (plyEdUtil.StringIsUnique(this.blox.variables.varDefs, var.name))
				{
					var.ident = this.blox.variables.CreateVariableIdent();
					this.blox.variables.varDefs.Add(var);
					this.Save();
					BloxContainerInspector instance = BloxContainerInspector.Instance;
					if ((object)instance != null)
					{
						instance.OnBloxVariablesChanged(this.blox.ident);
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Variables", "The variable name must be unique.", "OK");
				}
			}
		}

		private void OnRenameVariable(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text) && !text.Equals(this.blox.variables.varDefs[this.selected].name))
			{
				if (plyEdUtil.StringIsUnique(this.blox.variables.varDefs, text))
				{
					this.blox.variables.varDefs[this.selected].name = text;
					this.Save();
					BloxContainerInspector instance = BloxContainerInspector.Instance;
					if ((object)instance != null)
					{
						instance.OnBloxVariablesChanged(this.blox.ident);
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Variables", "The variable name must be unique.", "OK");
				}
			}
			this.ed.Repaint();
		}

		private void Repaint()
		{
			EditorWindow obj = this.ed;
			if ((object)obj != null)
			{
				obj.Repaint();
			}
		}

		private void Save()
		{
			this.blox.variables._SetDirty();
			plyEdUtil.SetDirty(this.blox);
			this.ed.Repaint();
		}
	}
}
