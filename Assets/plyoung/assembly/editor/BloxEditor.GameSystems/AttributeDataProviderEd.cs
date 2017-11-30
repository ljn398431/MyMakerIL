using BloxEditor.Databinding;
using BloxEngine.Databinding;
using BloxGameSystems;
using plyLibEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[plyCustomEd(typeof(AttributeDataProvider), Name = "Attribute", opt = 1)]
	public class AttributeDataProviderEd : DataProviderEd
	{
		private static readonly GUIContent GC_GetAtt = new GUIContent("Get");

		private static readonly GUIContent GC_SetAtt = new GUIContent("Set");

		private static readonly GUIContent GC_Of = new GUIContent("of");

		private static readonly GUIContent GC_Param1 = new GUIContent("Attributes owner");

		private static readonly GUIContent GC_ToVal = new GUIContent("to");

		private static readonly GUIContent GC_ToValBindWin = new GUIContent("Value Getter");

		private static readonly Type[] limitTypes = new Type[1]
		{
			typeof(ICharacterAttributesOwner)
		};

		public override string Label(DataProvider target)
		{
			AttributeDataProvider attributeDataProvider = target as AttributeDataProvider;
			if (attributeDataProvider.attId >= 0)
			{
				CharacterAttributeDef item = BloxEdGlobal.AttributeDefs.GetItem(attributeDataProvider.attId);
				if (item != null)
				{
					return string.Format("Attribute: {0}.{1}", item.ident, attributeDataProvider.attValType);
				}
			}
			return base.nfo.Name;
		}

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			return (float)((EditorGUIUtility.singleLineHeight + 2.0) * (float)(isSetter ? 3 : 2));
		}

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			AttributeDataProvider attributeDataProvider = target as AttributeDataProvider;
			EditorGUIUtility.labelWidth = 30f;
			float num = (float)((rect.width - EditorGUIUtility.labelWidth - 2.0) / 2.0);
			Rect rect2 = rect;
			rect2.height = EditorGUIUtility.singleLineHeight;
			rect2.width = EditorGUIUtility.labelWidth;
			GUI.Label(rect2, isSetter ? AttributeDataProviderEd.GC_SetAtt : AttributeDataProviderEd.GC_GetAtt);
			rect2.x += EditorGUIUtility.labelWidth;
			rect2.width = num;
			attributeDataProvider.attId = plyEdGUI.IdxIdConvertedPopup(rect2, attributeDataProvider.attId, BloxEdGlobal.AttributeDefs.Labels(), BloxEdGlobal.AttributeDefs);
			rect2.x += (float)(num + 2.0);
			attributeDataProvider.attValType = (AttributeDataProvider.ValueType)EditorGUI.EnumPopup(rect2, (Enum)(object)attributeDataProvider.attValType);
			rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			rect2.x = rect.x;
			rect2.width = EditorGUIUtility.labelWidth;
			GUI.Label(rect2, AttributeDataProviderEd.GC_Of);
			rect2.x += EditorGUIUtility.labelWidth;
			rect2.width = ((attributeDataProvider.attSource == AttributeDataProvider.ValueSource.Caller) ? (rect.width - EditorGUIUtility.labelWidth) : num);
			attributeDataProvider.attSource = (AttributeDataProvider.ValueSource)EditorGUI.EnumPopup(rect2, (Enum)(object)attributeDataProvider.attSource);
			if (attributeDataProvider.attSource == AttributeDataProvider.ValueSource.Target)
			{
				rect2.x += (float)(num + 2.0);
				DataProviderEd.DataBindingField(rect2, null, attributeDataProvider.attOwnerBind, attributeDataProvider, false, AttributeDataProviderEd.limitTypes);
			}
			if (isSetter)
			{
				rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
				rect2.x = rect.x;
				rect2.width = rect.width;
				attributeDataProvider.valSetterSource = DataProviderEd.DataBindingValueSourceField(rect2, AttributeDataProviderEd.GC_ToVal, attributeDataProvider.valSetterSource, AttributeDataProviderEd.GC_ToValBindWin, attributeDataProvider);
			}
		}

		public override SimpleParamInfo[] GetSetterParams(DataProvider target)
		{
			return new SimpleParamInfo[1]
			{
				new SimpleParamInfo
				{
					name = "Set",
					typeName = "Float",
					type = typeof(float)
				}
			};
		}
	}
}
