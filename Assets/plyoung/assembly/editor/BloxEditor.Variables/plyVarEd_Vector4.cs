using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Vector4), "Vector4", Order = 43)]
	public class plyVarEd_Vector4 : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Vector4";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Vector4)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Vector4")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Vector4 plyVar_Vector = (plyVar_Vector4)target.ValueHandler;
			plyVar_Vector.storedValue = plyEdGUI.Vector4Field(rect, plyVar_Vector.storedValue);
			return false;
		}
	}
}
