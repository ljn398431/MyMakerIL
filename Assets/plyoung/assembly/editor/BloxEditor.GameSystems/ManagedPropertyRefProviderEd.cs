using BloxEditor.Databinding;
using BloxEngine.Databinding;
using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[plyCustomEd(typeof(ManagedPropertyRefProvider), Name = "Managed Property", opt = 1)]
	public class ManagedPropertyRefProviderEd : DataProviderEd
	{
		private static readonly GUIContent GC_GetVar = new GUIContent("Get");

		private static readonly GUIContent GC_SetVar = new GUIContent("Set");

		private static readonly GUIContent GC_ToVal = new GUIContent("to");

		private static readonly GUIContent GC_ToValBindWin = new GUIContent("Value Getter");

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			return (float)((EditorGUIUtility.singleLineHeight + 2.0) * (float)((!isSetter) ? 1 : 2));
		}

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			ManagedPropertyRefProvider managedPropertyRefProvider = target as ManagedPropertyRefProvider;
			EditorGUIUtility.labelWidth = 30f;
			rect.height = EditorGUIUtility.singleLineHeight;
			Rect rect2 = rect;
			rect2.width = EditorGUIUtility.labelWidth;
			GUI.Label(rect2, isSetter ? ManagedPropertyRefProviderEd.GC_SetVar : ManagedPropertyRefProviderEd.GC_GetVar);
			rect2.x += EditorGUIUtility.labelWidth;
			rect2.width = rect.width - EditorGUIUtility.labelWidth;
			managedPropertyRefProvider.propertyName = plyEdGUI.PopupTextField(rect2, managedPropertyRefProvider.propertyName, PropertiesManagerEd.PropertyLabels, false);
			if (isSetter)
			{
				rect.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
				managedPropertyRefProvider.valSetterSource = DataProviderEd.DataBindingValueSourceField(rect, ManagedPropertyRefProviderEd.GC_ToVal, managedPropertyRefProvider.valSetterSource, ManagedPropertyRefProviderEd.GC_ToValBindWin, managedPropertyRefProvider);
			}
		}

		public override SimpleParamInfo[] GetSetterParams(DataProvider target)
		{
			ManagedProperty property = PropertiesManagerEd.GetProperty((target as ManagedPropertyRefProvider).propertyName);
			if (property != null && property.setter != null)
			{
				return DataProviderEd.GetBindParams(property.setter);
			}
			return new SimpleParamInfo[0];
		}
	}
}
