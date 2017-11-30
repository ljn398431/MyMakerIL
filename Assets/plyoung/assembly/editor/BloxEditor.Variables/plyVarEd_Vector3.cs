using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Vector3), "Vector3", Order = 42)]
	public class plyVarEd_Vector3 : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Vector3";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Vector3)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Vector3")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Vector3 plyVar_Vector = (plyVar_Vector3)target.ValueHandler;
			plyVar_Vector.storedValue = EditorGUI.Vector3Field(rect, GUIContent.none, plyVar_Vector.storedValue);
			return false;
		}
	}
}
