using BloxEngine;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	public class BloxListPopup : EditorWindow
	{
		private static GUIContent GC_DrawElement = new GUIContent(Ico._blox);

		private static GUIContent GC_Head = new GUIContent("Select a definition");

		private plyEdGUI.ListOps listOps;

		private Vector2 scroll;

		private int selectedBloxIdx = -1;

		private BloxContainer target;

		private Action<string, object[]> onAddCallback;

		private bool lostFocus;

		private object[] args;

		public static void Show_BloxListPopup(BloxContainer target, Action<string, object[]> onAddCallback, object[] args = null)
		{
			BloxListPopup window = EditorWindow.GetWindow<BloxListPopup>(true, "Blox Definitions", true);
			Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxListWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			window.titleContent = new GUIContent("Blox Definitions", image);
			window.target = target;
			window.onAddCallback = onAddCallback;
			window.args = args;
			window.minSize = new Vector2(200f, 250f);
			window.ShowUtility();
		}

		protected void OnEnable()
		{
			Texture2D image = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_mono" + (plyEdGUI.IsDarkSkin() ? "_p" : "") + ".png", typeof(BloxListWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			base.titleContent = new GUIContent("Blox Definitions", image);
			BloxEd.LoadBloxGlobal();
			if (this.listOps == null)
			{
				this.listOps = new plyEdGUI.ListOps
				{
					emptyMsg = "No Blox defined",
					drawBackground = false,
					onDrawElement = this.DrawElement,
					canAdd = false,
					canDuplicate = false,
					canRemove = false,
					canChangePosition = false
				};
			}
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
		}

		protected void OnGUI()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				base.Close();
				GUIUtility.ExitGUI();
			}
			else
			{
				plyEdGUI.List<Blox>(ref this.selectedBloxIdx, BloxEd.BloxGlobalObj.bloxDefs, ref this.scroll, this.listOps, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal(plyEdGUI.Styles.BottomBar);
				GUILayout.FlexibleSpace();
				if (this.selectedBloxIdx < 0 || ((UnityEngine.Object)this.target != (UnityEngine.Object)null && this.target.bloxIdents.Contains(BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx].ident)))
				{
					GUI.enabled = false;
				}
				else
				{
					GUI.enabled = true;
				}
				if (GUILayout.Button("Accept", GUILayout.Width(80f)))
				{
					if ((UnityEngine.Object)this.target != (UnityEngine.Object)null)
					{
						this.target.bloxIdents.Add(BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx].ident);
						plyEdUtil.SetDirty(this.target);
					}
					this.onAddCallback(BloxEd.BloxGlobalObj.bloxDefs[this.selectedBloxIdx].ident, this.args);
					base.Close();
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
		}

		private void DrawElement(Rect rect, int index, bool selected)
		{
			if (Event.current.type == EventType.Repaint)
			{
				BloxListPopup.GC_DrawElement.text = Ico._blox + " " + BloxEd.BloxGlobalObj.bloxDefs[index].screenName;
				plyEdGUI.Styles.ListElement.Draw(rect, BloxListPopup.GC_DrawElement, selected, selected, selected, true);
			}
		}
	}
}
