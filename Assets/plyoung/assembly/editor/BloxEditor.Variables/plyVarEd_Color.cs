using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Color), "Color", Order = 30)]
	public class plyVarEd_Color : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Color";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Color)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Color")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Color plyVar_Color = (plyVar_Color)target.ValueHandler;
			plyVar_Color.storedValue = EditorGUI.ColorField(rect, plyVar_Color.storedValue);
			return false;
		}
	}
}
