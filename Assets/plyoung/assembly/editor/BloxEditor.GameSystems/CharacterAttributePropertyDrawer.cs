using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[CustomPropertyDrawer(typeof(CharacterAttributeAttribute))]
	public class CharacterAttributePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect r, SerializedProperty property, GUIContent label)
		{
			CharacterAttributeAttribute characterAttributeAttribute = (CharacterAttributeAttribute)base.attribute;
			GUI.Label(r, characterAttributeAttribute.Label);
			r.x += EditorGUIUtility.labelWidth;
			r.width -= EditorGUIUtility.labelWidth;
			property.intValue = plyEdGUI.IdxIdConvertedPopup(r, property.intValue, BloxEdGlobal.AttributeDefs.Labels(), BloxEdGlobal.AttributeDefs);
		}
	}
}
