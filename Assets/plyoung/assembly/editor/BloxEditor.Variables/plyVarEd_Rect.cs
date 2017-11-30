using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Rect), "Rect", Order = 40)]
	public class plyVarEd_Rect : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Rect";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Rect)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Rect")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Rect plyVar_Rect = (plyVar_Rect)target.ValueHandler;
			plyVar_Rect.storedValue = plyEdGUI.RectField(rect, plyVar_Rect.storedValue);
			return false;
		}
	}
}
