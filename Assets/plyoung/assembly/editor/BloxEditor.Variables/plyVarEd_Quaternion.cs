using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Quaternion), "Quaternion", Order = 44)]
	public class plyVarEd_Quaternion : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Quaternion";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(Quaternion)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Quaternion")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			plyVar_Quaternion plyVar_Quaternion = (plyVar_Quaternion)target.ValueHandler;
			plyVar_Quaternion.storedValue = plyEdGUI.QuaternionField(rect, plyVar_Quaternion.storedValue);
			return false;
		}
	}
}
