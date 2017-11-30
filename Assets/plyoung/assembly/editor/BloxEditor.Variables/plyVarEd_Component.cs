using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_Component), "Component", Order = 101)]
	public class plyVarEd_Component : plyVarEd
	{
		private static List<string> exludeNamespaces = new List<string>
		{
			"UnityEditor",
			"plyLib",
			"plyLibEditor",
			"BloxEngine",
			"BloxEditor",
			"BloxGenerated"
		};

		private static List<Type> exludeTypes = null;

		private plyVar lastTarget;

		private int selected = -1;

		public override string VarTypeName(plyVar target)
		{
			return target.variableType.Name;
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = plyEdUtil.GetDerivedTypes(typeof(Component), plyVarEd_Component.exludeNamespaces, plyVarEd_Component.exludeTypes);
			base._handleTypes.Sort((Type a, Type b) => a.FullName.CompareTo(b.FullName));
			base._handleTypeNames = new GUIContent[base._handleTypes.Count];
			for (int i = 0; i < base._handleTypes.Count; i++)
			{
				base._handleTypeNames[i] = new GUIContent(base._handleTypes[i].FullName.Replace(".", "/"));
			}
		}

		public override bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			if (target == objRefProxy && (target.objRefs == null || target.objRefs.Length == 0))
			{
				target.objRefs = new UnityEngine.Object[1];
			}
			objRefProxy.objRefs[objRefProxyIdx] = EditorGUI.ObjectField(rect, objRefProxy.objRefs[objRefProxyIdx], target.variableType, isOnSceneObject);
			return false;
		}

		public override bool DrawCreateWizard(plyVar target)
		{
			plyVar_Component plyVar_Component = (plyVar_Component)target.ValueHandler;
			if (target != this.lastTarget)
			{
				target = this.lastTarget;
				this.selected = -1;
				if (plyVar_Component.variableType != typeof(Component))
				{
					int num = 0;
					while (num < base.handleTypes.Count)
					{
						if (base.handleTypes[num] != plyVar_Component.variableType)
						{
							num++;
							continue;
						}
						this.selected = num;
						break;
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			this.selected = EditorGUILayout.Popup(this.selected, base.handleTypeNames);
			if (EditorGUI.EndChangeCheck())
			{
				plyVar_Component.SetStoredType(base.handleTypes[this.selected]);
			}
			return true;
		}
	}
}
