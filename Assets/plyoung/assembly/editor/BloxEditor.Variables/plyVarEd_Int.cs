using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Int), "Integer", Order = 11)]
	public class plyVarEd_Int : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Integer";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(int)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Integer")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Int plyVar_Int = (plyVar_Int)target.ValueHandler;
			plyVar_Int.storedValue = EditorGUI.IntField(rect, plyVar_Int.storedValue);
			return false;
		}
	}
}
