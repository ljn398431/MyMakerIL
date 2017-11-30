using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_GameObject), "GameObject", Order = 102)]
	public class plyVarEd_GameObject : plyVarEd
	{
		public override string VarTypeName(plyVar target)
		{
			return "GameObject";
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>
			{
				typeof(GameObject)
			};
			base._handleTypeNames = new GUIContent[1]
			{
				new GUIContent("GameObject")
			};
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			if (target == objRefProxy && (target.objRefs == null || target.objRefs.Length == 0))
			{
				target.objRefs = new UnityEngine.Object[1];
			}
			objRefProxy.objRefs[objRefProxyIdx] = EditorGUI.ObjectField(rect, objRefProxy.objRefs[objRefProxyIdx], typeof(GameObject), isOnSceneObject);
			return false;
		}
	}
}
