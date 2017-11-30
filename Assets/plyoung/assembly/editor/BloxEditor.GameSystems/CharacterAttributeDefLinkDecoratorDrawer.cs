using BloxGameSystems;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	[CustomPropertyDrawer(typeof(CharacterAttributeDefLinkAttribute))]
	public class CharacterAttributeDefLinkDecoratorDrawer : DecoratorDrawer
	{
		private static readonly GUIContent GC_Link = new GUIContent("(define attributes)");

		public override float GetHeight()
		{
			if (((CharacterAttributeDefLinkAttribute)base.attribute).Header == null)
			{
				return 16f;
			}
			return 24f;
		}

		public override void OnGUI(Rect r)
		{
			CharacterAttributeDefLinkAttribute characterAttributeDefLinkAttribute = (CharacterAttributeDefLinkAttribute)base.attribute;
			if (characterAttributeDefLinkAttribute.Header != null)
			{
				r.y += 8f;
				r = EditorGUI.IndentedRect(r);
				GUI.Label(r, characterAttributeDefLinkAttribute.Header, EditorStyles.boldLabel);
				r.x += EditorGUIUtility.labelWidth;
				r.width -= EditorGUIUtility.labelWidth;
				r.y -= 5f;
			}
			if (GUI.Button(r, CharacterAttributeDefLinkDecoratorDrawer.GC_Link, plyEdGUI.Styles.Link))
			{
				MainEditorWindow.Show_MainEditorWindow("blox-main-ed", "attributes");
			}
		}
	}
}
