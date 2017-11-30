using BloxEngine.Variables;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEditor.Variables
{
	public class plyVarEd : plyCustomEd
	{
		protected List<Type> _handleTypes;

		protected GUIContent[] _handleTypeNames;

		public List<Type> handleTypes
		{
			get
			{
				if (this._handleTypes == null)
				{
					this.InitHandleTypes();
				}
				return this._handleTypes;
			}
		}

		public GUIContent[] handleTypeNames
		{
			get
			{
				if (this._handleTypeNames == null)
				{
					this.InitHandleTypes();
				}
				return this._handleTypeNames;
			}
		}

		public virtual string VarTypeName(plyVar target)
		{
			return "";
		}

		public virtual void InitHandleTypes()
		{
		}

		public virtual bool DrawCreateWizard(plyVar target)
		{
			return true;
		}

		public virtual bool DrawEditor(Rect rect, bool isOnSceneObject, plyVar target, plyVar objRefProxy, int objRefProxyIdx)
		{
			return false;
		}

		public virtual bool DrawAdvancedEditor(plyVar target, bool isOnSceneObject, Action saveCallback)
		{
			return false;
		}
	}
}
