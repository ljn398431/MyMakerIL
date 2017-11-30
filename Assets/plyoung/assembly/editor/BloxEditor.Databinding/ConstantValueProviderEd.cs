using BloxEditor.Variables;
using BloxEngine.Databinding;
using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	[plyCustomEd(typeof(ConstantValueProvider), Name = "Constant Value")]
	public class ConstantValueProviderEd : DataProviderEd
	{
		private static readonly GUIContent GC_ValueType = new GUIContent("Value Type");

		private static GUIContent[] varEdLabels = null;

		private static List<plyVarEd> varEditors = null;

		private int varTypeIdx = -1;

		private plyVarEd currVarEd;

		private Type selectedEdType;

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			ConstantValueProvider constantValueProvider = (ConstantValueProvider)target;
			this.Init(constantValueProvider);
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginChangeCheck();
			this.varTypeIdx = EditorGUI.Popup(rect, ConstantValueProviderEd.GC_ValueType, this.varTypeIdx, ConstantValueProviderEd.varEdLabels);
			if (EditorGUI.EndChangeCheck())
			{
				this.currVarEd = ConstantValueProviderEd.varEditors[this.varTypeIdx];
				constantValueProvider.constant = plyVar.Create(this.currVarEd.nfo.TargetType);
				constantValueProvider.constant.name = "constant";
			}
			if (this.currVarEd != null && constantValueProvider.constant != null)
			{
				rect.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
				rect.x += EditorGUIUtility.labelWidth;
				rect.width -= EditorGUIUtility.labelWidth;
				this.currVarEd.DrawEditor(rect, false, constantValueProvider.constant, constantValueProvider.constant, 0);
			}
		}

		public override string Label(DataProvider target)
		{
			object value = ((ConstantValueProvider)target).constant.GetValue();
			if (value != null)
			{
				return value.ToString();
			}
			return "null";
		}

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			return (float)(EditorGUIUtility.singleLineHeight * 2.0 + 2.0);
		}

		public override SimpleParamInfo[] GetSetterParams(DataProvider target)
		{
			return new SimpleParamInfo[0];
		}

		private void Init(ConstantValueProvider Target)
		{
			if (ConstantValueProviderEd.varEdLabels == null || ConstantValueProviderEd.varEditors == null)
			{
				ConstantValueProviderEd.varEditors = new List<plyVarEd>();
				plyVariablesEditor.LoadVarEds();
				foreach (plyVarEd value in plyVariablesEditor.editors.Values)
				{
					if (!((plyVarEdAttribute)value.nfo).UsesAdvancedEditor)
					{
						ConstantValueProviderEd.varEditors.Add(value);
					}
				}
				ConstantValueProviderEd.varEdLabels = new GUIContent[ConstantValueProviderEd.varEditors.Count];
				for (int i = 0; i < ConstantValueProviderEd.varEditors.Count; i++)
				{
					ConstantValueProviderEd.varEdLabels[i] = new GUIContent(((plyVarEdAttribute)ConstantValueProviderEd.varEditors[i].nfo).VarTypeName);
				}
			}
			if (this.currVarEd == null)
			{
				this.selectedEdType = Target.constant.ValueHandler.GetType();
				for (int j = 0; j < ConstantValueProviderEd.varEditors.Count; j++)
				{
					plyVarEdAttribute plyVarEdAttribute = (plyVarEdAttribute)ConstantValueProviderEd.varEditors[j].nfo;
					if (this.selectedEdType == plyVarEdAttribute.TargetType)
					{
						this.currVarEd = ConstantValueProviderEd.varEditors[j];
						this.varTypeIdx = j;
						break;
					}
				}
			}
			if (this.varTypeIdx < 0)
			{
				this.varTypeIdx = 0;
				this.currVarEd = ConstantValueProviderEd.varEditors[0];
				Target.constant = plyVar.Create(this.currVarEd.nfo.TargetType);
				Target.constant.name = "constant";
			}
		}
	}
}
