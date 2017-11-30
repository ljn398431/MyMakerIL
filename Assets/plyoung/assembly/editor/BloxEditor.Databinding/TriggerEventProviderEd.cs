using BloxEngine;
using BloxEngine.Databinding;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	[plyCustomEd(typeof(TriggerEventDataProvider), Name = "Blox Event", opt = 1)]
	public class TriggerEventProviderEd : DataProviderEd
	{
		private static readonly GUIContent GC_VarHead = new GUIContent("Value in Variable");

		private static readonly GUIContent GC_ObjHead = new GUIContent("of Object");

		private static readonly GUIContent GC_EventHead = new GUIContent("after Event");

		private static readonly GUIContent GC_BloxName = new GUIContent("-select-");

		private static readonly GUIContent GC_ObjName = new GUIContent("object name");

		private static readonly GUIContent GC_TagName = new GUIContent("tag name");

		private static readonly GUIContent GC_ParamsHead = new GUIContent("params");

		private static readonly GUIContent GC_Param = new GUIContent("with param0 = ");

		private static readonly GUIContent GC_ToValBindWin = new GUIContent("Value Getter");

		public override string Label(DataProvider target)
		{
			TriggerEventDataProvider triggerEventDataProvider = target as TriggerEventDataProvider;
			return string.Format("{{b}} {0} after Event:{1}", triggerEventDataProvider.varName, triggerEventDataProvider.triggerBloxEvent);
		}

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			TriggerEventDataProvider triggerEventDataProvider = target as TriggerEventDataProvider;
			return (float)((EditorGUIUtility.singleLineHeight + 2.0) * (float)(triggerEventDataProvider.valSetterSources.Length + 4));
		}

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			TriggerEventDataProvider triggerEventDataProvider = target as TriggerEventDataProvider;
			EditorGUIUtility.labelWidth = 110f;
			rect.height = EditorGUIUtility.singleLineHeight;
			Rect rect2 = rect;
			float num = 0f;
			rect2.width = EditorGUIUtility.labelWidth;
			GUI.Label(rect2, TriggerEventProviderEd.GC_VarHead);
			rect2.x += EditorGUIUtility.labelWidth;
			rect2.width = (num = (float)((rect.width - (rect2.width + 6.0)) / 2.0));
			triggerEventDataProvider.varName = EditorGUI.TextField(rect2, triggerEventDataProvider.varName);
			Blox blox = null;
			if (!string.IsNullOrEmpty(triggerEventDataProvider.bloxIdent))
			{
				blox = BloxEd.GetBloxDef(triggerEventDataProvider.bloxIdent);
				if ((UnityEngine.Object)blox == (UnityEngine.Object)null)
				{
					triggerEventDataProvider.bloxIdent = "";
				}
				else
				{
					TriggerEventProviderEd.GC_BloxName.text = blox.screenName;
				}
			}
			TriggerEventProviderEd.GC_BloxName.text = (((UnityEngine.Object)blox == (UnityEngine.Object)null) ? "-select-" : blox.screenName);
			rect2.x += (float)(rect2.width + 3.0);
			if (GUI.Button(rect2, TriggerEventProviderEd.GC_BloxName))
			{
				BloxListPopup.Show_BloxListPopup(null, this.OnBloxSelected, new object[1]
				{
					triggerEventDataProvider
				});
			}
			rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			rect2.x = rect.x;
			rect2.width = EditorGUIUtility.labelWidth;
			GUI.Label(rect2, TriggerEventProviderEd.GC_ObjHead);
			rect2.x += EditorGUIUtility.labelWidth;
			rect2.width = num;
			triggerEventDataProvider.sourceObjType = (TriggerEventDataProvider.DataSourceOject)EditorGUI.EnumPopup(rect2, (Enum)(object)triggerEventDataProvider.sourceObjType);
			if (triggerEventDataProvider.sourceObjType != TriggerEventDataProvider.DataSourceOject.Owner)
			{
				rect2.x += (float)(rect2.width + 3.0);
				triggerEventDataProvider.objNameOrTag = plyEdGUI.InlineLabelTextField(rect2, (triggerEventDataProvider.sourceObjType == TriggerEventDataProvider.DataSourceOject.WithName) ? TriggerEventProviderEd.GC_ObjName : TriggerEventProviderEd.GC_TagName, triggerEventDataProvider.objNameOrTag);
			}
			rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			rect2.x = rect.x;
			rect2.width = EditorGUIUtility.labelWidth;
			GUI.Label(rect2, TriggerEventProviderEd.GC_EventHead);
			rect2.x += EditorGUIUtility.labelWidth;
			rect2.width = rect.width - EditorGUIUtility.labelWidth;
			triggerEventDataProvider.triggerBloxEvent = EditorGUI.TextField(rect2, triggerEventDataProvider.triggerBloxEvent);
			rect2.x = rect.x;
			rect2.width = rect.width;
			for (int i = 0; i < triggerEventDataProvider.valSetterSources.Length; i++)
			{
				rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
				TriggerEventProviderEd.GC_Param.text = string.Format("with param{0} = ", i);
				triggerEventDataProvider.valSetterSources[i] = DataProviderEd.DataBindingValueSourceField(rect2, TriggerEventProviderEd.GC_Param, triggerEventDataProvider.valSetterSources[i], TriggerEventProviderEd.GC_ToValBindWin, triggerEventDataProvider);
			}
			rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			rect2.x = (float)(rect.xMax - 103.0);
			rect2.width = 50f;
			GUI.Label(rect2, TriggerEventProviderEd.GC_ParamsHead);
			rect2.x += 53f;
			rect2.width = 25f;
			if (GUI.Button(rect2, Ico._add, plyEdGUI.Styles.MiniButtonLeft))
			{
				DataBindingValueSource item = new DataBindingValueSource();
				ArrayUtility.Add<DataBindingValueSource>(ref triggerEventDataProvider.valSetterSources, item);
				plyEdUtil.SetDirty(triggerEventDataProvider);
			}
			rect2.x += 25f;
			GUI.enabled = (triggerEventDataProvider.valSetterSources.Length != 0);
			if (GUI.Button(rect2, Ico._remove, plyEdGUI.Styles.MiniButtonRight))
			{
				DataBinding databind = triggerEventDataProvider.valSetterSources[triggerEventDataProvider.valSetterSources.Length - 1].databind;
				DataProviderEd.DestroyDataprovider((databind != null) ? databind.dataprovider : null);
				ArrayUtility.RemoveAt<DataBindingValueSource>(ref triggerEventDataProvider.valSetterSources, triggerEventDataProvider.valSetterSources.Length - 1);
				plyEdUtil.SetDirty(triggerEventDataProvider);
			}
			GUI.enabled = true;
		}

		private void OnBloxSelected(string bloxIdent, object[] args)
		{
			TriggerEventDataProvider obj = args[0] as TriggerEventDataProvider;
			obj.bloxIdent = bloxIdent;
			plyEdUtil.SetDirty(obj);
		}
	}
}
