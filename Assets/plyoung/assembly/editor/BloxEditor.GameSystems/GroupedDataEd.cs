using BloxEditor.Variables;
using BloxEngine;
using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class GroupedDataEd<GroupT, ItemT> where GroupT : GroupedDataGroup<ItemT> where ItemT : GroupedDataItem
	{
		protected static float ColumnWidth = 280f;

		protected EditorWindow editorWindow;

		protected plyReorderableList.Button[] extraButtons;

		private ReorderableGroupedData<GroupT, ItemT> listEd;

		private plyVariablesEditor metaEd;

		private GroupedDataItem copiedMetaOwner;

		private GUIContent GC_Head;

		private string S_Help;

		private Vector2 scroll = Vector2.zero;

		protected virtual GroupedData<GroupT, ItemT> groupedData
		{
			get;
		}

		protected virtual void OnFocus()
		{
		}

		protected virtual void DrawSelected(ItemT item)
		{
		}

		protected virtual void DrawBeforeList()
		{
		}

		protected virtual float DrawSelectedWidth()
		{
			return GroupedDataEd<GroupT, ItemT>.ColumnWidth;
		}

		protected virtual bool DrawMetaInNextColumn()
		{
			return false;
		}

		public GroupedDataEd(string heading, string helpEntry)
		{
			this.GC_Head = (string.IsNullOrEmpty(heading) ? null : new GUIContent(Ico._ellipsis_v + " " + heading));
			this.S_Help = helpEntry;
		}

		public void OnSelected(EditorWindow editorWindow)
		{
			this.editorWindow = editorWindow;
			if (this.listEd == null)
			{
				if (this.extraButtons == null)
				{
					this.extraButtons = new plyReorderableList.Button[0];
				}
				ArrayUtility.Add<plyReorderableList.Button>(ref this.extraButtons, new plyReorderableList.Button
				{
					label = new GUIContent(Ico._rename, "Rename selected"),
					callback = this.Item_Rename,
					requireSelected = true
				});
				this.listEd = new ReorderableGroupedData<GroupT, ItemT>(this.groupedData, true, true, true, true, false, this.extraButtons);
				this.listEd.elementHeight = (float)(EditorGUIUtility.singleLineHeight + 6.0);
				this.listEd.drawElementCallback = this.Item_Draw;
				this.listEd.onAddElement = this.Item_Add;
				this.listEd.onRemoveElement = this.Item_Remove;
				this.listEd.onReorder = this.Save;
				this.listEd.onGroupChanged = this.Save;
			}
			if (this.metaEd == null)
			{
				this.metaEd = new plyVariablesEditor(null, plyVariablesType.Custom, null, true, true, true, editorWindow.Repaint, this.Save, new plyReorderableList.Button[1]
				{
					new plyReorderableList.Button
					{
						label = new GUIContent(Ico._copy, "Duplicate these properties"),
						callback = this.Meta_Duplicate,
						requireSelected = false
					}
				});
				this.metaEd.GC_Head = new GUIContent("Meta Properties");
				this.metaEd.nameString = "Property";
				this.metaEd.canStartDragDrop = false;
			}
			this.OnFocus();
		}

		public void Draw(EditorWindow editorWindow)
		{
			if (this.listEd != null)
			{
				this.editorWindow = editorWindow;
				if (this.GC_Head != null)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label(this.GC_Head, plyEdGUI.Styles.HeadLabel);
					if (this.S_Help != null)
					{
						plyEdHelpManager.Button(this.S_Help);
						EditorGUILayout.Space();
					}
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(10f);
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical(GUILayout.Width(170f));
				this.DrawBeforeList();
				this.listEd.DoLayoutList();
				EditorGUILayout.EndVertical();
				GUILayout.Space(5f);
				this.scroll = EditorGUILayout.BeginScrollView(this.scroll);
				this.DrawSelected();
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawSelected()
		{
			if (this.listEd.currGroupIdx >= 0 && this.listEd.index >= 0 && this.listEd.currGroupIdx < this.groupedData.groups.Count && this.listEd.index < ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items.Count)
			{
				GroupedDataItem groupedDataItem = (GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items[this.listEd.index];
				if (groupedDataItem != null)
				{
					EditorGUIUtility.labelWidth = 115f;
					if (this.DrawMetaInNextColumn())
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.BeginVertical(GUILayout.Width(this.DrawSelectedWidth()));
						this.DrawSelected((ItemT)groupedDataItem);
						EditorGUILayout.EndVertical();
						GUILayout.Space(5f);
						EditorGUILayout.BeginVertical(GUILayout.Width(GroupedDataEd<GroupT, ItemT>.ColumnWidth));
						this.metaEd.SetTarget(groupedDataItem.meta, null);
						this.metaEd.DoLayout();
						EditorGUILayout.EndVertical();
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
					}
					else
					{
						EditorGUILayout.BeginVertical(GUILayout.Width(this.DrawSelectedWidth()));
						this.DrawSelected((ItemT)groupedDataItem);
						this.metaEd.SetTarget(groupedDataItem.meta, null);
						this.metaEd.DoLayout();
						EditorGUILayout.EndVertical();
					}
					if (GUI.changed)
					{
						this.Save();
					}
				}
			}
		}

		public int CurrentGroupIdx()
		{
			return this.listEd.currGroupIdx;
		}

		public GroupT CurrentGroup()
		{
			if (this.listEd.currGroupIdx >= 0 && this.listEd.currGroupIdx < this.groupedData.groups.Count)
			{
				return this.groupedData.groups[this.listEd.currGroupIdx];
			}
			return null;
		}

		public void Save()
		{
			this.groupedData.EntriesDirty();
			plyEdUtil.SetDirty(this.groupedData);
		}

		private void Item_Draw(Rect rect, int groupIdx, int eleIndex)
		{
			rect.y += 2f;
			rect.height = EditorGUIUtility.singleLineHeight;
			GUI.Label(rect, ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[groupIdx]).items[eleIndex]).ident, plyEdGUI.Styles.RL_Element);
		}

		private void Item_Add()
		{
			plyTextInputWiz.ShowWiz("Add Entry", "Enter a unique name", "", this.Item_Add, null, 250f);
		}

		private void Item_Add(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text))
			{
				if (this.groupedData.ItemNameIsUnique(text))
				{
					this.groupedData.CreateItem(text, this.groupedData.groups[this.listEd.currGroupIdx]);
					this.Save();
				}
				else
				{
					EditorUtility.DisplayDialog("Add Entry", "The name must be unique.", "OK");
				}
			}
			this.editorWindow.Repaint();
		}

		private void Item_Remove()
		{
			if (EditorUtility.DisplayDialog("Add Entry", "Remove selected entry. This can't be undone. Are you sure?", "Yes", "Cancel"))
			{
				((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items.RemoveAt(this.listEd.index);
				this.Save();
			}
		}

		private void Item_Rename()
		{
			plyTextInputWiz.ShowWiz("Rename Entry", "Enter a unique name", ((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items[this.listEd.index]).ident, this.Item_Rename, null, 250f);
		}

		private void Item_Rename(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text) && !text.Equals(((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items[this.listEd.index]).ident))
			{
				if (this.groupedData.ItemNameIsUnique(text))
				{
					((GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items[this.listEd.index]).ident = text;
					this.Save();
				}
				else
				{
					EditorUtility.DisplayDialog("Rename Entry", "The name must be unique.", "OK");
				}
			}
			this.editorWindow.Repaint();
		}

		private void Meta_Duplicate()
		{
			if (this.listEd.index >= 0 && this.listEd.index < ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items.Count)
			{
				this.copiedMetaOwner = (GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items[this.listEd.index];
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("to Group"), false, this.OnDuplicateContextMenu, 1);
				genericMenu.AddItem(new GUIContent("to All"), false, this.OnDuplicateContextMenu, 2);
				genericMenu.ShowAsContext();
			}
		}

		private void OnDuplicateContextMenu(object arg)
		{
			switch ((int)arg)
			{
			case 1:
				if (EditorUtility.DisplayDialog("Duplicate Meta Properties", "Duplicate properties from " + this.copiedMetaOwner.ident + " to all entries in this group. This will make sure the other entries all have the same meta properties in the same order as this one. Are you sure?", "Yes", "Cancel"))
				{
					for (int k = 0; k < ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items.Count; k++)
					{
						GroupedDataItem groupedDataItem2 = (GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.listEd.currGroupIdx]).items[k];
						this.copiedMetaOwner.meta.DuplicateTo(groupedDataItem2.meta);
						this.Save();
						this.editorWindow.Repaint();
					}
				}
				break;
			case 2:
				if (EditorUtility.DisplayDialog("Duplicate Meta Properties", "Duplicate properties from " + this.copiedMetaOwner.ident + " to all entries in all groups. This will make sure the other entries all have the same meta properties in the same order as this one. Are you sure?", "Yes", "Cancel"))
				{
					for (int i = 0; i < this.groupedData.groups.Count; i++)
					{
						for (int j = 0; j < ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[i]).items.Count; j++)
						{
							GroupedDataItem groupedDataItem = (GroupedDataItem)(object)((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[i]).items[j];
							this.copiedMetaOwner.meta.DuplicateTo(groupedDataItem.meta);
							this.Save();
							this.editorWindow.Repaint();
						}
					}
				}
				break;
			}
		}
	}
}
