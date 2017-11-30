using BloxEngine.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Variables
{
	[plyVarEd(typeof(plyVar_UnityObject), "UnityObject", Order = 103)]
	public class plyVarEd_UnityObject : plyVarEd
	{
		private static Type[] supported_types = new Type[16]
		{
			typeof(AudioClip),
			typeof(Component),
			typeof(GameObject),
			typeof(Material),
			typeof(Sprite),
			typeof(Texture2D),
			typeof(Texture3D),
			typeof(RenderTexture),
			typeof(Shader),
			typeof(LightmapParameters),
			typeof(Animation),
			typeof(RuntimeAnimatorController),
			typeof(AnimatorOverrideController),
			typeof(Avatar),
			typeof(PhysicMaterial),
			typeof(PhysicsMaterial2D)
		};

		private plyVar lastTarget;

		private int selected = -1;

		public override string VarTypeName(plyVar target)
		{
			return target.variableType.Name;
		}

		public override void InitHandleTypes()
		{
			base._handleTypes = new List<Type>();
			base._handleTypeNames = new GUIContent[plyVarEd_UnityObject.supported_types.Length];
			for (int i = 0; i < plyVarEd_UnityObject.supported_types.Length; i++)
			{
				base._handleTypes.Add(plyVarEd_UnityObject.supported_types[i]);
				base._handleTypeNames[i] = new GUIContent(plyVarEd_UnityObject.supported_types[i].Name);
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
			plyVar_UnityObject plyVar_UnityObject = (plyVar_UnityObject)target.ValueHandler;
			if (target != this.lastTarget)
			{
				target = this.lastTarget;
				this.selected = -1;
				if (plyVar_UnityObject.variableType != typeof(UnityEngine.Object))
				{
					int num = 0;
					while (num < base.handleTypes.Count)
					{
						if (base.handleTypes[num] != plyVar_UnityObject.variableType)
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
				plyVar_UnityObject.SetStoredType(base.handleTypes[this.selected]);
			}
			return true;
		}
	}
}
