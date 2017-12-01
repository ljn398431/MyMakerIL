using BloxEngine;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
    /// <summary>
    /// blox ап╠М 
    /// </summary>
	public class BloxListWindow : EditorWindow
	{
		private static readonly GUIContent GC_Add = new GUIContent(Ico._add + "New", "Add new Blox Definition");

		private static readonly GUIContent GC_Duplicate = new GUIContent(Ico._plus_one, "Duplicate selected Blox Definition");

		private static readonly GUIContent GC_Remove = new GUIContent(Ico._delete, "Delete selected Blox Definition");

		private static GUIContent GC_DrawElement = new GUIContent(Ico._blox);

		public static BloxListWindow Instance;

		private plyEdGUI.ListOps listOps;

		private Vector2 scroll;

		private int selectedBloxIdx = -1;

		public static void Show_BloxListWindow()
		{
            Debug.Log(" Show_BloxListWindow", "BloxListWindow", UnityEngine.Color.yellow);
            BloxListWindow obj = BloxListWindow.Instance = EditorWindow.GetWindow<BloxListWindow>("BloxDefs");
			Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxListWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			obj.titleContent = new GUIContent("BloxDefs", image);
		}

		protected void OnEnable()
		{
            Debug.Log("OnEnable", "BloxListWindow", UnityEngine.Color.yellow);
            BloxListWindow.Instance = this;
			Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxListWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			base.titleContent = new GUIContent("BloxDefs", image);
			BloxEd.LoadBloxGlobal();
			if (this.listOps == null)
			{
				this.listOps = new plyEdGUI.ListOps
				{
					emptyMsg = "No Blox defined",
					drawBackground = false,
					onDrawElement = this.DrawElement,
					onAction = this.ListAction,
					extraButtons = new plyEdGUI.ListOpsExtraToolbarButton[2]
					{
						new plyEdGUI.ListOpsExtraToolbarButton
						{
							label = new GUIContent(Ico._rename, "Rename selected Blox Definition"),
							callback = delegate
							{
								plyTextInputWiz.ShowWiz("Rename Blox", "", BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx].screenName, this.OnRenameBloxDef, null, 250f);
							},
							enabled = (() => this.selectedBloxIdx >= 0)
						},
						new plyEdGUI.ListOpsExtraToolbarButton
						{
							label = new GUIContent(Ico._blox + "Edit", "Edit in Blox Editor"),
							callback = delegate
							{
								BloxEditorWindow.Show_BloxEditorWindow(BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx]);
							},
							enabled = (() => this.selectedBloxIdx >= 0)
						}
					},
					canAdd = true,
					canDuplicate = false,
					canRemove = true,
					canChangePosition = false,
					addLabel = BloxListWindow.GC_Add,
					removeLabel = BloxListWindow.GC_Remove,
					duplicateLabel = BloxListWindow.GC_Duplicate
				};
			}
		}

		protected void OnFocus()
		{
			BloxListWindow.Instance = this;
		}

		public Blox CurrentlySelectedBlox()
		{
			if (this.selectedBloxIdx >= 0 && this.selectedBloxIdx < BloxEd.BloxGlobalObj.bloxDefs.Count)
			{
				return BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx];
			}
			return null;
		}

		public void SetSelectBlox(Blox bd)
		{
			if ((Object)bd != (Object)null)
			{
				this.selectedBloxIdx = BloxEd.BloxGlobalObj.bloxDefs.IndexOf(bd);
				base.Repaint();
			}
		}

		protected void OnGUI()
		{
			this.DrawBloxList();
		}

		private void DrawElement(Rect rect, int index, bool selected)
		{
			if (Event.current.type == EventType.Repaint)
			{
				BloxListWindow.GC_DrawElement.text = Ico._blox + " " + BloxEd.BloxGlobalObj.bloxDefs[index].screenName;
				plyEdGUI.Styles.ListElement.Draw(rect, BloxListWindow.GC_DrawElement, selected, selected, selected, selected);
			}
		}

		private int ListAction(plyEdGUI.ListOps.ListAction act)
		{
			switch (act)
			{
			case plyEdGUI.ListOps.ListAction.DoAdd:
				BloxEd.CreateNewBloxDef();
				this.selectedBloxIdx = -1;
				break;
			case plyEdGUI.ListOps.ListAction.DoRemoveSelected:
				if (EditorUtility.DisplayDialog("Blox", "Delete Blox Definition. This can't be undone. Are you sure?", "Yes", "Cancel"))
				{
					BloxEd.DeleteBloxDef(BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx]);
					BloxEditorWindow instance = BloxEditorWindow.Instance;
					if ((object)instance != null)
					{
						instance.Repaint();
					}
					plyEdUtil.RepaintInspector(typeof(BloxContainer));
				}
				break;
			}
			return -1;
		}

		private void OnRenameBloxDef(plyTextInputWiz wiz)
		{
			string text = wiz.text;
			wiz.Close();
			if (!string.IsNullOrEmpty(text))
			{
				BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx].screenName = text;
				plyEdUtil.SetDirty(BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx]);
				BloxEd.SortBloxDefList();
				base.Repaint();
				BloxEditorWindow instance = BloxEditorWindow.Instance;
				if ((object)instance != null)
				{
					instance.Repaint();
				}
				plyEdUtil.RepaintInspector(typeof(BloxContainer));
			}
		}

		public void DrawBloxList()
		{
			int num = plyEdGUI.List<Blox>(ref this.selectedBloxIdx, BloxEd.BloxGlobalObj.bloxDefs, ref this.scroll, this.listOps, new GUILayoutOption[0]);
			switch (num)
			{
			case 1:
				if (BloxEdGlobal.DoubleClickOpenBloxDef)
					break;
				goto case 2;
			case 2:
				if (this.selectedBloxIdx >= 0 && this.selectedBloxIdx < BloxEd.BloxGlobalObj.bloxDefs.Count)
				{
					BloxEditorWindow.Show_BloxEditorWindow(BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx]);
				}
				return;
			}
			if (num >= 3)
			{
				num -= 3;
				plyEdGUI.ClearFocus();
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[0];
				DragAndDrop.paths = null;
				DragAndDrop.SetGenericData("BloxDefinition", BloxEd.BloxGlobalObj.bloxDefs[num]);
				DragAndDrop.StartDrag(BloxEd.BloxGlobalObj.bloxDefs[num].screenName);
				Event.current.Use();
			}
		}
	}
}
