using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_String), "String", Order = 13)]
	public class plyVarEd_String : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "String";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(string)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("String")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_String plyVar_String = (plyVar_String)target.ValueHandler;
			plyVar_String.storedValue = EditorGUI.TextField(rect, plyVar_String.storedValue);
			return false;
		}
	}
}
