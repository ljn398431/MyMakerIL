using BloxEditor.Databinding;
using BloxEngine;
using BloxEngine.Databinding;
using BloxGameSystems;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(SplashScreensManager))]
	public class SplashScreensManagerInspector : Editor
	{
		private static readonly GUIContent GC_Head = new GUIContent("Screens");

		private static readonly GUIContent GC_PlayerSkip = new GUIContent(" player can skip");

		private static readonly GUIContent GC_Seconds = new GUIContent(" seconds");

		private static readonly GUIContent GC_AutoUnload = new GUIContent("Auto-unload", "Should the manager auto destroy the GameObject or Scene when it is done? This will happen after the events are triggered.");

		private static readonly GUIContent GC_MinShow = new GUIContent("Min show-time", "Force plash screens to be visible for at least this long.");

		private DataProviderEd comparisonCheckEd;

		private plyReorderableList screenList;

		private SplashScreensManager Target;

		private SerializedProperty p_onSpashScreenShown;

		private SerializedProperty p_onSpashScreenHidden;

		private SerializedProperty p_onSpashScreensDone;

		protected void OnEnable()
		{
			this.p_onSpashScreenShown = base.serializedObject.FindProperty("onSpashScreenShown");
			this.p_onSpashScreenHidden = base.serializedObject.FindProperty("onSpashScreenHidden");
			this.p_onSpashScreensDone = base.serializedObject.FindProperty("onSpashScreensDone");
			if (this.comparisonCheckEd == null)
			{
				this.comparisonCheckEd = DataProviderEd.factory.CreateEditor(typeof(ComparisonDataProvider));
			}
		}

		public override void OnInspectorGUI()
		{
			this.Target = (SplashScreensManager)base.target;
			if (this.screenList == null)
			{
				this.screenList = new plyReorderableList(this.Target.screens, typeof(SplashScreensManager.SplashScreen), true, true, true, true, false, false, false, null, null);
				this.screenList.elementHeight = (float)(3.0 * (EditorGUIUtility.singleLineHeight + 2.0) + 6.0);
				this.screenList.drawHeaderCallback = this.DrawListHeader;
				this.screenList.drawElementCallback = this.DrawElement;
				this.screenList.onAddElement = this.OnAdd;
				this.screenList.onRemoveElement = this.OnRemove;
				this.screenList.onReorder = this.OnReorder;
			}
			EditorGUILayout.Space();
			this.screenList.DoLayoutList();
			this.Target.autoUnloadWhenDone = (AutoUnloadOption)EditorGUILayout.EnumPopup(SplashScreensManagerInspector.GC_AutoUnload, (Enum)(object)this.Target.autoUnloadWhenDone);
			this.Target.minShowTime = EditorGUILayout.FloatField(SplashScreensManagerInspector.GC_MinShow, this.Target.minShowTime);
			EditorGUILayout.Space();
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.p_onSpashScreenShown);
			EditorGUILayout.PropertyField(this.p_onSpashScreenHidden);
			EditorGUILayout.PropertyField(this.p_onSpashScreensDone);
			base.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Space();
			if (GUI.changed)
			{
				plyEdUtil.SetDirty(this.Target);
				GUI.changed = false;
			}
		}

		private void DrawListHeader(Rect rect)
		{
			GUI.Label(rect, SplashScreensManagerInspector.GC_Head);
		}

		private void OnAdd()
		{
			this.Target.screens.Add(new SplashScreensManager.SplashScreen());
			plyEdUtil.SetDirty(this.Target);
		}

		private void OnRemove()
		{
			this.RemoveComparisonComponent(this.screenList.index);
			this.Target.screens.RemoveAt(this.screenList.index);
			plyEdUtil.SetDirty(this.Target);
		}

		private void OnReorder()
		{
			plyEdUtil.SetDirty(this.Target);
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			Rect rect2 = rect;
			rect2.y += 3f;
			rect2.height = EditorGUIUtility.singleLineHeight;
			this.Target.screens[index].target = (GameObject)EditorGUI.ObjectField(rect2, this.Target.screens[index].target, typeof(GameObject), true);
			rect2.y += (float)(rect2.height + 2.0);
			if (this.Target.screens[index].waitType == SplashScreensManager.SplashScreen.WaitType.Timeout)
			{
				this.RemoveComparisonComponent(index);
				rect2.width /= 2f;
				this.Target.screens[index].waitType = (SplashScreensManager.SplashScreen.WaitType)EditorGUI.EnumPopup(rect2, (Enum)(object)this.Target.screens[index].waitType);
				float x = GUI.skin.label.CalcSize(SplashScreensManagerInspector.GC_Seconds).x;
				rect2.x += (float)(rect2.width + 2.0);
				rect2.width -= (float)(2.0 + x);
				this.Target.screens[index].timeout = EditorGUI.FloatField(rect2, this.Target.screens[index].timeout);
				rect2.x += rect2.width;
				rect2.width = x;
				GUI.Label(rect2, SplashScreensManagerInspector.GC_Seconds);
			}
			else if (this.Target.screens[index].waitType == SplashScreensManager.SplashScreen.WaitType.WatchVariable)
			{
				if ((UnityEngine.Object)this.Target.screens[index].watchVariable == (UnityEngine.Object)null)
				{
					this.Target.screens[index].watchVariable = DataProviderEd.CreateDataprovider<ComparisonDataProvider>("", false);
					plyEdUtil.SetDirty(this.Target);
				}
				rect2.width = 18f;
				this.Target.screens[index].waitType = (SplashScreensManager.SplashScreen.WaitType)EditorGUI.EnumPopup(rect2, (Enum)(object)this.Target.screens[index].waitType);
				rect2.x += 19f;
				rect2.width = (float)(rect.width - 19.0);
				this.comparisonCheckEd.DrawEditor(rect2, this.Target.screens[index].watchVariable, false);
			}
			else if (this.Target.screens[index].waitType == SplashScreensManager.SplashScreen.WaitType.WaitScreenEndTrigger)
			{
				this.RemoveComparisonComponent(index);
				this.Target.screens[index].waitType = (SplashScreensManager.SplashScreen.WaitType)EditorGUI.EnumPopup(rect2, (Enum)(object)this.Target.screens[index].waitType);
			}
			rect2.x = rect.x;
			rect2.width = rect.width;
			rect2.y += (float)(rect2.height + 2.0);
			rect2.width = rect.width;
			this.Target.screens[index].playerCanSkip = EditorGUI.ToggleLeft(rect2, SplashScreensManagerInspector.GC_PlayerSkip, this.Target.screens[index].playerCanSkip);
		}

		private void RemoveComparisonComponent(int index)
		{
			if ((UnityEngine.Object)this.Target.screens[index].watchVariable != (UnityEngine.Object)null)
			{
				DataProviderEd.DestroyDataprovider(this.Target.screens[index].watchVariable);
				this.Target.screens[index].watchVariable = null;
				plyEdUtil.SetDirty(this.Target);
			}
		}
	}
}
