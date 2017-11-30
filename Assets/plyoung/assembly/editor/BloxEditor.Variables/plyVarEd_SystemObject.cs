using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_SystemObject), "Any", Order = 9999)]
	public class plyVarEd_SystemObject : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "Any";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(object)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("Any")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			GUI.Label(rect, "Can't edit this variable type");
			return false;
		}
	}
}
