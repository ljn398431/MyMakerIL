using BloxGameSystems;
using plyLibEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class ReorderableGroupedData<GroupT, ItemT> where GroupT : GroupedDataGroup<ItemT> where ItemT : GroupedDataItem
	{
		public Action<Rect, int, int> drawElementCallback;

		public Action onAddElement;

		public Action onRemoveElement;

		public Action onReorder;

		public Action onSelectElement;

		public Action onDoubleClickElement;

		public Action onGroupChanged;

		private ReorderableList orderList;

		private bool displayAddButton;

		private bool displayRemoveButton;

		private plyReorderableList.Button[] extraButtons;

		private GroupedData<GroupT, ItemT> groupedData;

		private List<ItemT> fakeList = new List<ItemT>();

		private List<ItemT> currList;

		private GUIContent[] groupLabels;

		private Vector2 scroll = Vector2.zero;

		private static readonly GUIContent GC_Add = new GUIContent(Ico._add, "Add an entry");

		private static readonly GUIContent GC_Remove = new GUIContent(Ico._remove, "Remove selected entry");

		public int index
		{
			get
			{
				return this.orderList.index;
			}
			set
			{
				this.orderList.index = value;
			}
		}

		public IList list
		{
			get
			{
				return this.orderList.list;
			}
			set
			{
				this.orderList.list = value;
			}
		}

		public float elementHeight
		{
			get
			{
				return this.orderList.elementHeight;
			}
			set
			{
				this.orderList.elementHeight = value;
			}
		}

		public int currGroupIdx
		{
			get;
			private set;
		}

		public ReorderableGroupedData(GroupedData<GroupT, ItemT> data, bool draggable, bool displayAddButton, bool displayRemoveButton, bool showButtonsOnBottom, bool showExtraButtonsOnBottom, plyReorderableList.Button[] extraButtons = null)
		{
			Type typeFromHandle = typeof(GroupedDataItem);
			this.displayAddButton = displayAddButton;
			this.displayRemoveButton = displayRemoveButton;
			this.extraButtons = extraButtons;
			this.currGroupIdx = -1;
			this.groupedData = data;
			this.currList = this.fakeList;
			this.orderList = new ReorderableList(this.currList, typeFromHandle, draggable, false, false, false);
			this.orderList.headerHeight = 0f;
			this.orderList.footerHeight = 0f;
			this.orderList.drawHeaderCallback = this.FakeDrawHeader;
			this.orderList.drawElementCallback = this.DrawElement;
			this.orderList.onAddCallback = this.OnAdd;
			this.orderList.onRemoveCallback = this.OnRemove;
			this.orderList.onReorderCallback = this.OnReorder;
			this.orderList.onSelectCallback = this.OnSelect;
			this.RefreshGroupLabels();
		}

		public void DoLayoutList()
		{
			this.DrawHeader(GUILayoutUtility.GetRect(1f, 18f, GUILayout.ExpandWidth(true)));
			this.scroll = GUILayout.BeginScrollView(this.scroll);
			this.orderList.DoLayoutList();
			GUILayout.EndScrollView();
		}

		public void DoLayoutList(float width)
		{
			EditorGUILayout.BeginVertical(GUILayout.Width(width), GUILayout.ExpandHeight(true));
			this.DrawHeader(GUILayoutUtility.GetRect(width, 18f));
			this.scroll = GUILayout.BeginScrollView(this.scroll);
			this.orderList.DoLayoutList();
			GUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		private void FakeDrawHeader(Rect rect)
		{
		}

		private void DrawHeader(Rect rect)
		{
			ReorderableList.Defaults defaultBehaviours = ReorderableList.defaultBehaviours;
			if (defaultBehaviours != null)
			{
				defaultBehaviours.DrawHeaderBackground(rect);
			}
			Rect position = rect;
			int num = 20;
			GUIStyle rL_ListButton = plyEdGUI.Styles.RL_ListButton;
			Rect position2 = rect;
			position2.x = position2.xMax;
			Vector2 vector;
			if (((this.extraButtons != null) ? this.extraButtons.Length : 0) != 0)
			{
				for (int i = 0; i < this.extraButtons.Length; i++)
				{
					vector = rL_ListButton.CalcSize(this.extraButtons[i].label);
					if (vector.x < (float)num)
					{
						vector.x = (float)num;
					}
					position2.x -= vector.x;
					position2.width = vector.x;
					position.width -= position2.width;
					GUI.enabled = (!this.extraButtons[i].requireSelected || this.orderList.index >= 0);
					if (GUI.Button(position2, this.extraButtons[i].label, rL_ListButton))
					{
						Action callback = this.extraButtons[i].callback;
						if (callback != null)
						{
							callback();
						}
					}
					GUI.enabled = true;
				}
			}
			if (this.displayRemoveButton)
			{
				vector = rL_ListButton.CalcSize(ReorderableGroupedData<GroupT, ItemT>.GC_Remove);
				if (vector.x < (float)num)
				{
					vector.x = (float)num;
				}
				position2.x -= vector.x;
				position2.width = vector.x;
				position.width -= position2.width;
				GUI.enabled = (this.orderList.index >= 0);
				if (GUI.Button(position2, ReorderableGroupedData<GroupT, ItemT>.GC_Remove, rL_ListButton))
				{
					this.OnRemove((ReorderableList)null);
				}
				GUI.enabled = true;
			}
			if (this.displayAddButton)
			{
				vector = rL_ListButton.CalcSize(ReorderableGroupedData<GroupT, ItemT>.GC_Add);
				if (vector.x < (float)num)
				{
					vector.x = (float)num;
				}
				position2.x -= vector.x;
				position2.width = vector.x;
				position.width -= position2.width;
				if (GUI.Button(position2, ReorderableGroupedData<GroupT, ItemT>.GC_Add, rL_ListButton))
				{
					this.OnAdd((ReorderableList)null);
				}
			}
			int currGroupIdx = this.currGroupIdx;
			this.currGroupIdx = EditorGUI.Popup(position, this.currGroupIdx, this.groupLabels, EditorStyles.toolbarPopup);
			if (this.currGroupIdx >= this.groupedData.groups.Count)
			{
				int num2 = this.currGroupIdx - this.groupedData.groups.Count;
				if (this.groupedData.groups.Count == 0)
				{
					num2++;
				}
				this.currGroupIdx = currGroupIdx;
				if (num2 == 1)
				{
					plyTextInputWiz.ShowWiz("Add Group", "Enter a unique name for this group", "", this.AddGroup, null, 250f);
				}
				if (num2 == 3)
				{
					plyTextInputWiz.ShowWiz("Rename Group", "Enter a unique name for this group", ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.currGroupIdx]).ident, this.RenameGroup, null, 250f);
				}
				if (num2 == 2 && EditorUtility.DisplayDialog("Grouped List", "Removing this group will also remove all entries of this group. This can't be undone. Are you sure?", "Yes", "Cancel"))
				{
					this.groupedData.groups.RemoveAt(this.currGroupIdx);
					Action obj = this.onGroupChanged;
					if (obj != null)
					{
						obj();
					}
					this.currGroupIdx--;
					if (this.currGroupIdx < 0 && this.groupedData.groups.Count > 0)
					{
						this.currGroupIdx = 0;
					}
					this.currList = ((this.currGroupIdx < 0) ? this.fakeList : ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.currGroupIdx]).items);
					this.orderList.list = this.currList;
					this.RefreshGroupLabels();
				}
			}
			else if (this.currGroupIdx != currGroupIdx)
			{
				this.currList = ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.currGroupIdx]).items;
				this.orderList.list = this.currList;
				this.orderList.index = -1;
			}
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			Action<Rect, int, int> obj = this.drawElementCallback;
			if (obj != null)
			{
				obj(rect, this.currGroupIdx, index);
			}
		}

		private void OnAdd(ReorderableList l)
		{
			if (this.currGroupIdx < 0 || this.currList == this.fakeList)
			{
				plyTextInputWiz.ShowWiz("Add Group", "Enter a unique name for this group", "", this.AddGroup, null, 250f);
			}
			else
			{
				Action obj = this.onAddElement;
				if (obj != null)
				{
					obj();
				}
			}
		}

		private void OnAddMenu(object userData)
		{
			if (userData != null)
			{
				((Action)userData)();
			}
		}

		private void OnRemove(ReorderableList l)
		{
			Action obj = this.onRemoveElement;
			if (obj != null)
			{
				obj();
			}
		}

		private void OnReorder(ReorderableList l)
		{
			Action obj = this.onReorder;
			if (obj != null)
			{
				obj();
			}
		}

		private void OnSelect(ReorderableList list)
		{
			Action obj = this.onSelectElement;
			if (obj != null)
			{
				obj();
			}
			if (Event.current.clickCount >= 2 && this.onDoubleClickElement != null)
			{
				this.onDoubleClickElement();
			}
		}

		private void RefreshGroupLabels()
		{
			if (this.groupedData.groups.Count > 0)
			{
				if (this.currGroupIdx < 0)
				{
					this.currGroupIdx = 0;
					this.currList = ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.currGroupIdx]).items;
					this.orderList.list = this.currList;
				}
				this.groupLabels = new GUIContent[this.groupedData.groups.Count + 4];
				for (int i = 0; i < this.groupedData.groups.Count; i++)
				{
					this.groupLabels[i] = new GUIContent(((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[i]).ident);
				}
				int count = this.groupedData.groups.Count;
				this.groupLabels[count] = new GUIContent(" ");
				this.groupLabels[count + 1] = new GUIContent("Add New Group");
				this.groupLabels[count + 2] = new GUIContent("Remove Group");
				this.groupLabels[count + 3] = new GUIContent("Rename Group");
			}
			else
			{
				if (this.currGroupIdx >= 0 || this.currList != this.fakeList)
				{
					this.currGroupIdx = -1;
					this.currList = this.fakeList;
					this.orderList.list = this.currList;
				}
				this.groupLabels = new GUIContent[1]
				{
					new GUIContent("Add New Group")
				};
			}
		}

		private void AddGroup(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text))
			{
				if (!this.groupedData.GroupNameIsUnique(text))
				{
					EditorUtility.DisplayDialog("Add Group", "The group name must be unique", "OK");
				}
				else
				{
					this.groupedData.CreateGroup(text);
					Action obj = this.onGroupChanged;
					if (obj != null)
					{
						obj();
					}
					this.currGroupIdx = this.groupedData.groups.Count - 1;
					this.currList = ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.currGroupIdx]).items;
					this.orderList.list = this.currList;
					this.RefreshGroupLabels();
				}
			}
		}

		private void RenameGroup(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text))
			{
				if (this.currGroupIdx >= 0 && ((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.currGroupIdx]).ident.Equals(text))
					return;
				if (!this.groupedData.GroupNameIsUnique(text))
				{
					EditorUtility.DisplayDialog("Rename Group", "The group name must be unique", "OK");
				}
				else
				{
					((GroupedDataGroup<ItemT>)(object)this.groupedData.groups[this.currGroupIdx]).ident = text;
					Action obj = this.onGroupChanged;
					if (obj != null)
					{
						obj();
					}
					this.RefreshGroupLabels();
				}
			}
		}
	}
}
