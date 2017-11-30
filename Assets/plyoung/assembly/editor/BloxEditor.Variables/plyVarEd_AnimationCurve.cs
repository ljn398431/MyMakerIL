using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_AnimationCurve), "AnimationCurve", Order = 50)]
	public class plyVarEd_AnimationCurve : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "AnimationCurve";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(AnimationCurve)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("AnimationCurve")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_AnimationCurve plyVar_AnimationCurve = (plyVar_AnimationCurve)target.ValueHandler;
			plyVar_AnimationCurve.storedValue = EditorGUI.CurveField(rect, plyVar_AnimationCurve.storedValue);
			return false;
		}
	}
}
