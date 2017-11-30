using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Vector2), "Vector2", Order = 41)]
	public class plyVarEd_Vector2 : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Vector2";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Vector2)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Vector2")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Vector2 plyVar_Vector = (plyVar_Vector2)target.ValueHandler;
			plyVar_Vector.storedValue = EditorGUI.Vector2Field(rect, GUIContent.none, plyVar_Vector.storedValue);
			return false;
		}
	}
}
