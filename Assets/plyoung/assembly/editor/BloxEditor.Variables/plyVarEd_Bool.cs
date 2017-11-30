using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Bool), "Boolean", Order = 10)]
	public class plyVarEd_Bool : plyVarEd
	{
		private static readonly GUIContent GC_True = new GUIContent(" True");

		private static readonly GUIContent GC_False = new GUIContent(" False");

		public override string VarTypeName(plyVar target)
		{
			return "Boolean";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(bool)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Boolean")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Bool plyVar_Bool = (plyVar_Bool)target.ValueHandler;
			plyVar_Bool.storedValue = EditorGUI.ToggleLeft(rect, plyVar_Bool.storedValue ? plyVarEd_Bool.GC_True : plyVarEd_Bool.GC_False, plyVar_Bool.storedValue);
			return false;
		}
	}
}
