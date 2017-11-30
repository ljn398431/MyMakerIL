using BloxEngine;
using BloxEngine.Databinding;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	[plyCustomEd(typeof(CurveDataMapProvider), Name = "Curve Data Map")]
	public class CurveDataMapProviderEd : DataProviderEd
	{
		private static readonly GUIContent GC_Param1 = new GUIContent("Input Value");

		private static readonly GUIContent GC_Curve = new GUIContent("Curve");

		private static readonly GUIContent GC_Rounding = new GUIContent("Rounding", "Should the result be rounded to nearest whole number?");

		private static readonly GUIContent GC_Label = new GUIContent();

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			return (float)((EditorGUIUtility.singleLineHeight + 2.0) * 3.0);
		}

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			CurveDataMapProvider curveDataMapProvider = target as CurveDataMapProvider;
			CurveDataMapProviderEd.GC_Label.text = DataProviderEd.GetDataBindingLabel(curveDataMapProvider.param1.dataprovider);
			rect.height = EditorGUIUtility.singleLineHeight;
			DataProviderEd.DataBindingField(rect, CurveDataMapProviderEd.GC_Param1, curveDataMapProvider.param1, curveDataMapProvider, false, null);
			rect.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			curveDataMapProvider.curve = EditorGUI.CurveField(rect, CurveDataMapProviderEd.GC_Curve, curveDataMapProvider.curve);
			rect.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			curveDataMapProvider.roundingOpt = (RoundingOption)EditorGUI.EnumPopup(rect, CurveDataMapProviderEd.GC_Rounding, (Enum)(object)curveDataMapProvider.roundingOpt);
		}
	}
}
