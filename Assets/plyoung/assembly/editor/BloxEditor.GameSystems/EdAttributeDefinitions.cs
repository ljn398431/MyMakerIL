using BloxEditor.Databinding;
using BloxGameSystems;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class EdAttributeDefinitions : GroupedDataEd<CharacterAttributeDefsGroup, CharacterAttributeDef>
	{
		private static readonly GUIContent GC_AttrInitialValue = new GUIContent("Initial Value", "The initial/ base Value of the Attribute");

		private static readonly GUIContent GC_AttrInitialMax = new GUIContent("Initial Max", "The initial/ base Max Value of the Attribute");

		private static readonly GUIContent GC_AttrInitValMax = new GUIContent("Reset Value=Max", "Should Value be reset to Max when Max changes?");

		private static readonly GUIContent GC_AttrButtonLabel = new GUIContent();

		private static readonly GUIContent GC_AttrModifiers = new GUIContent(" - Modifiers");

		private static readonly GUIContent GC_PropertyGetter2 = new GUIContent("Value getter/ reader");

		private static readonly GUIContent GC_PropertySetter2 = new GUIContent("Value setter/ writer");

		protected override GroupedData<CharacterAttributeDefsGroup, CharacterAttributeDef> groupedData
		{
			get
			{
				return BloxEdGlobal.AttributeDefs;
			}
		}

		public EdAttributeDefinitions() : base("Attribute Definitions", "blox-attributes")
		{
		}

		protected override void OnFocus()
		{
		}

		protected override void DrawBeforeList()
		{
			EditorGUILayout.HelpBox("This system is experimental and may undergo major changes", MessageType.Warning);
		}

		protected override void DrawSelected(CharacterAttributeDef def)
		{
			def.initialValue = EditorGUILayout.FloatField(EdAttributeDefinitions.GC_AttrInitialValue, def.initialValue);
			DataProviderEd.DataBindingField(EdAttributeDefinitions.GC_AttrModifiers, def.valueModifier, BloxEdGlobal.AttributeDefs, false, null);
			def.initialMaxVal = EditorGUILayout.FloatField(EdAttributeDefinitions.GC_AttrInitialMax, def.initialMaxVal);
			DataProviderEd.DataBindingField(EdAttributeDefinitions.GC_AttrModifiers, def.maxValModifier, BloxEdGlobal.AttributeDefs, false, null);
			def.resetValueToMax = EditorGUILayout.Toggle(EdAttributeDefinitions.GC_AttrInitValMax, def.resetValueToMax);
			EditorGUILayout.Space();
		}
	}
}
