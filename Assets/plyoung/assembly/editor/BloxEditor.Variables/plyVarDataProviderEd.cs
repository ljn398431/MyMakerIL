using BloxEditor.Databinding;
using BloxEngine;
using BloxEngine.Databinding;
using BloxEngine.Variables;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyCustomEd(typeof(plyVarDataProvider), Name = "Variable", opt = 1)]
	public class plyVarDataProviderEd : DataProviderEd
	{
		private static readonly GUIContent GC_VarName = new GUIContent("variable name");

		private static readonly GUIContent GC_ObjName = new GUIContent("object name");

		private static readonly GUIContent GC_TagName = new GUIContent("tag name");

		private static readonly GUIContent GC_BloxName = new GUIContent("-select-");

		private static readonly GUIContent GC_ToVal = new GUIContent("to");

		private static readonly GUIContent GC_Of = new GUIContent("of");

		private static readonly GUIContent GC_GetVar = new GUIContent("Get");

		private static readonly GUIContent GC_SetVar = new GUIContent("Set");

		private static readonly GUIContent GC_ToValBindWin = new GUIContent("Value Getter");

		public override string Label(DataProvider target)
		{
			plyVarDataProvider plyVarDataProvider = target as plyVarDataProvider;
			if (string.IsNullOrEmpty(plyVarDataProvider.varName))
			{
				return base.nfo.Name;
			}
			if (plyVarDataProvider.varType == plyVarDataProvider.VariableType.Global)
			{
				return "{g} " + plyVarDataProvider.varName;
			}
			if (plyVarDataProvider.varType == plyVarDataProvider.VariableType.Object)
			{
				return "{x} " + plyVarDataProvider.varName;
			}
			if (plyVarDataProvider.varType == plyVarDataProvider.VariableType.Blox)
			{
				return "{b} " + plyVarDataProvider.varName;
			}
			return base.nfo.Name;
		}

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			return (float)((EditorGUIUtility.singleLineHeight + 2.0) * (float)(isSetter ? 3 : 2));
		}

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			plyVarDataProvider plyVarDataProvider = target as plyVarDataProvider;
			EditorGUIUtility.labelWidth = 30f;
			rect.height = EditorGUIUtility.singleLineHeight;
			Rect rect2 = rect;
			float num = (float)((rect.width - (EditorGUIUtility.labelWidth + 3.0)) / 3.0);
			rect2.width = EditorGUIUtility.labelWidth;
			GUI.Label(rect2, isSetter ? plyVarDataProviderEd.GC_SetVar : plyVarDataProviderEd.GC_GetVar);
			rect2.x += EditorGUIUtility.labelWidth;
			rect2.width = num;
			plyVarDataProvider.varType = (plyVarDataProvider.VariableType)EditorGUI.EnumPopup(rect2, (Enum)(object)plyVarDataProvider.varType);
			rect2.x += (float)(num + 3.0);
			rect2.width = (float)(rect.width - (num + EditorGUIUtility.labelWidth + 3.0));
			plyVarDataProvider.varName = plyEdGUI.InlineLabelTextField(rect2, plyVarDataProviderEd.GC_VarName, plyVarDataProvider.varName);
			rect2.x += EditorGUIUtility.labelWidth;
			if (plyVarDataProvider.varType != 0)
			{
				rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
				rect2.x = rect.x;
				rect2.width = EditorGUIUtility.labelWidth;
				GUI.Label(rect2, plyVarDataProviderEd.GC_Of);
				rect2.x += EditorGUIUtility.labelWidth;
				rect2.width = num;
				plyVarDataProvider.sourceObjType = (plyVarDataProvider.DataSourceOject)EditorGUI.EnumPopup(rect2, (Enum)(object)plyVarDataProvider.sourceObjType);
				rect2.x += (float)(num + 3.0);
				rect2.width = (float)(rect.width - (num + EditorGUIUtility.labelWidth + 3.0));
				if (plyVarDataProvider.varType == plyVarDataProvider.VariableType.Blox)
				{
					rect2.width = (float)((rect2.width - 5.0) / 2.0);
				}
				if (plyVarDataProvider.sourceObjType != plyVarDataProvider.DataSourceOject.OfOwner)
				{
					plyVarDataProvider.objNameOrTag = plyEdGUI.InlineLabelTextField(rect2, (plyVarDataProvider.sourceObjType == plyVarDataProvider.DataSourceOject.ObjWithName) ? plyVarDataProviderEd.GC_ObjName : plyVarDataProviderEd.GC_TagName, plyVarDataProvider.objNameOrTag);
				}
				if (plyVarDataProvider.varType == plyVarDataProvider.VariableType.Blox)
				{
					Blox blox = null;
					if (!string.IsNullOrEmpty(plyVarDataProvider.bloxIdent))
					{
						blox = BloxEd.GetBloxDef(plyVarDataProvider.bloxIdent);
						if ((UnityEngine.Object)blox == (UnityEngine.Object)null)
						{
							plyVarDataProvider.bloxIdent = "";
						}
						else
						{
							plyVarDataProviderEd.GC_BloxName.text = blox.screenName;
						}
					}
					plyVarDataProviderEd.GC_BloxName.text = (((UnityEngine.Object)blox == (UnityEngine.Object)null) ? "-select-" : blox.screenName);
					rect2.x += (float)(rect2.width + 3.0);
					if (GUI.Button(rect2, plyVarDataProviderEd.GC_BloxName))
					{
						BloxListPopup.Show_BloxListPopup(null, this.OnBloxSelected, new object[1]
						{
							plyVarDataProvider
						});
					}
				}
			}
			if (isSetter)
			{
				rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
				rect2.x = rect.x;
				rect2.width = rect.width;
				plyVarDataProvider.valSetterSource = DataProviderEd.DataBindingValueSourceField(rect2, plyVarDataProviderEd.GC_ToVal, plyVarDataProvider.valSetterSource, plyVarDataProviderEd.GC_ToValBindWin, plyVarDataProvider);
			}
		}

		private void OnBloxSelected(string bloxIdent, object[] args)
		{
			plyVarDataProvider obj = args[0] as plyVarDataProvider;
			obj.bloxIdent = bloxIdent;
			plyEdUtil.SetDirty(obj);
		}

		public override SimpleParamInfo[] GetSetterParams(DataProvider target)
		{
			return new SimpleParamInfo[1]
			{
				new SimpleParamInfo
				{
					name = "Set",
					typeName = "Any",
					type = typeof(object)
				}
			};
		}
	}
}
