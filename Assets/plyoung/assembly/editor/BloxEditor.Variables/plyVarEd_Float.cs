using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Float), "Float", Order = 12)]
	public class plyVarEd_Float : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Float";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(float)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Float")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Float plyVar_Float = (plyVar_Float)target.ValueHandler;
			plyVar_Float.storedValue = EditorGUI.FloatField(rect, plyVar_Float.storedValue);
			return false;
		}
	}
}
