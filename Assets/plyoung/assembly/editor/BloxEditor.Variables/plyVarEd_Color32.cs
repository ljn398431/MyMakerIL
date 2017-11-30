using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Color32), "Color32", Order = 31)]
	public class plyVarEd_Color32 : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Color32";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Color32)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Color32")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Color32 plyVar_Color = (plyVar_Color32)target.ValueHandler;
			plyVar_Color.storedValue = EditorGUI.ColorField(rect, plyVar_Color.storedValue);
			return false;
		}
	}
}
