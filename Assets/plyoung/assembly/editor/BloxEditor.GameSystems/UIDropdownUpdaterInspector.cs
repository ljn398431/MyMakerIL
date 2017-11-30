using BloxEditor.Databinding;
using BloxGameSystems;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[CustomEditor(typeof(UIDropdownUpdater))]
	public class UIDropdownUpdaterInspector : UIElementUpdaterInspector
	{
		private static readonly GUIContent GC_LabelsSource = new GUIContent("Options Source", "Where the options of the dropdown will come from. The source must return a list or array of string values. Leave this empty/none to use the options you defined in the dropdown element's inspector.");

		private static readonly GUIContent GC_Helper = new GUIContent("none");

		private static readonly Type[] limitTypes = new Type[2]
		{
			typeof(List<string>),
			typeof(string[])
		};

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			UIDropdownUpdater uIDropdownUpdater = (UIDropdownUpdater)base.target;
			DataProviderEd.DataBindingField(UIDropdownUpdaterInspector.GC_LabelsSource, uIDropdownUpdater.labelsSource, uIDropdownUpdater, false, UIDropdownUpdaterInspector.limitTypes);
			if (GUI.changed)
			{
				plyEdUtil.SetDirty(uIDropdownUpdater);
				GUI.changed = false;
			}
		}
	}
}
