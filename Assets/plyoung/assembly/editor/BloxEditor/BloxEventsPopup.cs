using plyLibEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	public class BloxEventsPopup : PopupWindowContent
	{
		private static readonly GUIContent GC_LoadingEvents = new GUIContent("loading events");

		private static readonly GUIContent GC_SelectEvent = new GUIContent("Click on an Event to add it");

		private const float LeftWidth = 200f;

		private const float RightWidth = 450f;

		private const float Height = 300f;

		private static readonly Vector2 Size = new Vector2(650f, 300f);

		private plyEdCoroutine loader;

		private plyEdTreeView<BloxEventDef> treeView;

		private bool treeLoading = true;

		private Vector2 scroll;

		public void OnThemeChanged()
		{
			plyEdCoroutine obj = this.loader;
			if (obj != null)
			{
				obj.Stop();
			}
			this.loader = null;
			this.treeView = null;
		}

		public override void OnOpen()
		{
			base.OnOpen();
			if (this.loader == null)
			{
				this.treeLoading = true;
				this.loader = plyEdCoroutine.Start(this.LoadEventDefs(), true);
			}
			else if (this.treeView != null)
			{
				this.treeView.Reset();
			}
		}

		public override void OnClose()
		{
			base.OnClose();
		}

		public override Vector2 GetWindowSize()
		{
			return BloxEventsPopup.Size;
		}

		public override void OnGUI(Rect rect)
		{
			if (this.treeLoading)
			{
				EditorGUILayout.Space();
				plyEdGUI.DrawSpinner(BloxEventsPopup.GC_LoadingEvents, true, true);
				base.editorWindow.Repaint();
				this.loader.DoUpdate();
				if (this.treeView != null && Event.current.type == EventType.Repaint)
				{
					this.treeLoading = false;
				}
			}
			else
			{
				if (this.treeView.Initialize())
				{
					this.treeView.onItemSelected = this.OnTreeItemSelected;
					this.treeView.canMark = false;
					this.treeView.drawMode = plyEdTreeViewDrawMode.List;
					this.treeView.drawBackground = true;
				}
				EditorGUILayout.BeginHorizontal();
				this.treeView.editorWindow = base.editorWindow;
				this.treeView.DrawLayout(200f);
				EditorGUILayout.BeginVertical(GUILayout.Width(450f));
				Rect rect2 = GUILayoutUtility.GetRect(0f, 25f, BloxEdGUI.Styles.EventsPopupDocsHead);
				if (this.treeView.selected != null && !this.treeView.selected.hasChildren)
				{
					GUI.Label(rect2, this.treeView.selected.data.name, BloxEdGUI.Styles.EventsPopupDocsHead);
				}
				else
				{
					GUI.Label(rect2, GUIContent.none, BloxEdGUI.Styles.EventsPopupDocsHead);
				}
				this.scroll = EditorGUILayout.BeginScrollView(this.scroll);
				if (this.treeView.selected != null && !this.treeView.selected.hasChildren)
				{
					BloxEd.Instance.DrawBloxDoc(this.treeView.selected.data, true, base.editorWindow);
				}
				else
				{
					GUILayout.Label(BloxEventsPopup.GC_SelectEvent, plyEdGUI.Styles.WordWrappedLabel_RT);
				}
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void OnTreeItemSelected(plyEdTreeItem<BloxEventDef> item)
		{
			if (item.data != null)
			{
				BloxEditorWindow.Instance.AddEvent(item.data);
				base.editorWindow.Close();
			}
		}

		private IEnumerator LoadEventDefs()
        {
            Debug.Log("LoadEventDefs ", "BloxEditor.BloxEventsPopup", Color.green);
            BloxEd.Instance.LoadEventDefs();
			while (BloxEd.Instance.EventDefsLoading)
			{
				yield return (object)null;
			}
			plyEdTreeItem<BloxEventDef> treeRoot = new plyEdTreeItem<BloxEventDef>
			{
				children = new List<plyEdTreeItem<BloxEventDef>>()
			};
			int count = 0;
			int countBeforeYield = 20;
			List<BloxEventDef>.Enumerator enumerator = BloxEd.Instance.eventDefs.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BloxEventDef current = enumerator.Current;
					string[] array = current.ident.Split('/');
					plyEdTreeItem<BloxEventDef> plyEdTreeItem = treeRoot;
					for (int i = 0; i < array.Length - 1; i++)
					{
						string text = array[i];
						bool flag = false;
						if (plyEdTreeItem.children == null)
						{
							plyEdTreeItem.children = new List<plyEdTreeItem<BloxEventDef>>();
						}
						foreach (plyEdTreeItem<BloxEventDef> child in plyEdTreeItem.children)
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
							plyEdTreeItem<BloxEventDef> plyEdTreeItem2 = new plyEdTreeItem<BloxEventDef>
							{
								label = text
							};
							plyEdTreeItem.AddChild(plyEdTreeItem2);
							plyEdTreeItem = plyEdTreeItem2;
							if (plyEdTreeItem2.parent == treeRoot)
							{
								plyEdTreeItem2.icon = BloxEdGUI.Instance.folderIcon;
							}
						}
					}
					if (plyEdTreeItem.children == null)
					{
						plyEdTreeItem.children = new List<plyEdTreeItem<BloxEventDef>>();
					}
					plyEdTreeItem.children.Add(new plyEdTreeItem<BloxEventDef>
					{
						icon = ((current.iconName == null) ? null : BloxEdGUI.Instance.namedIcons[current.iconName]),
						label = array[array.Length - 1],
						data = current
					});
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
			enumerator = default(List<BloxEventDef>.Enumerator);
			this.treeView = new plyEdTreeView<BloxEventDef>(null, treeRoot, BloxEdGUI.Instance.folderIcon, "Events");
		}
	}
}
