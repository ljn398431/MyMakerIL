using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Gradient), "Gradient", Order = 32)]
	public class plyVarEd_Gradient : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Gradient";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Gradient)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Gradient")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Gradient plyVar_Gradient = (plyVar_Gradient)target.ValueHandler;
			plyEdUtil.DrawGradientField(rect, plyVar_Gradient.storedValue);
			return false;
		}
	}
}
