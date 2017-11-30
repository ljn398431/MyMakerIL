using BloxGameSystems;
using plyLibEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.GameSystems
{
	public class EdSkillDefinitions : GroupedDataEd<SkillDefsGroup, SkillDef>
	{
		private static readonly GUIContent GC_Logic = new GUIContent("Logic Driver", "The skill logic driver determines how the skill will act when activated");

		private static readonly GUIContent GC_ActivationCost = new GUIContent("Activation Cost", "The amount of mana, energy, action points or other resource needed to activate and use this skill");

		private static Dictionary<Type, SkillLogicEd> editors = null;

		private static GUIContent[] edLabels = new GUIContent[0];

		private int curr = -1;

		private SkillLogicEd currEd;

		private SkillDef selected;

		protected override GroupedData<SkillDefsGroup, SkillDef> groupedData
		{
			get
			{
				return BloxEdGlobal.SkillDefs;
			}
		}

		public EdSkillDefinitions() : base("Skill Definitions", "blox-skills")
		{
		}

		protected override void OnFocus()
		{
			if (EdSkillDefinitions.editors == null)
			{
				List<SkillLogicEd> list = plyCustomEd.CreateCustomEditorsList<SkillLogicEd>(typeof(SkillLogicEdAttribute));
				list.Sort((SkillLogicEd a, SkillLogicEd b) => ((SkillLogicEdAttribute)a.nfo).Name.CompareTo(((SkillLogicEdAttribute)b.nfo).Name));
				EdSkillDefinitions.editors = new Dictionary<Type, SkillLogicEd>();
				EdSkillDefinitions.edLabels = new GUIContent[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					EdSkillDefinitions.edLabels[i] = new GUIContent(((SkillLogicEdAttribute)list[i].nfo).Name);
					EdSkillDefinitions.editors.Add(list[i].nfo.TargetType, list[i]);
				}
			}
			this.curr = -1;
			this.currEd = null;
			this.selected = null;
		}

		protected override void DrawBeforeList()
		{
			EditorGUILayout.HelpBox("This system is experimental and may undergo major changes", MessageType.Warning);
		}

		protected override float DrawSelectedWidth()
		{
			return GroupedDataEd<SkillDefsGroup, SkillDef>.ColumnWidth;
		}

		protected override bool DrawMetaInNextColumn()
		{
			return true;
		}

		protected override void DrawSelected(SkillDef def)
		{
			if (this.selected != def)
			{
				this.curr = -1;
				this.currEd = null;
				this.selected = def;
				if ((UnityEngine.Object)def.skillLogicDef == (UnityEngine.Object)null || !EdSkillDefinitions.editors.TryGetValue(this.selected.skillLogicDef.GetType(), out this.currEd))
				{
					this.curr = 0;
					this.CreateSkillLogic();
				}
				else
				{
					int num = 0;
					while (num < EdSkillDefinitions.edLabels.Length)
					{
						if (!EdSkillDefinitions.edLabels[num].text.Equals(this.currEd.nfo.Name))
						{
							num++;
							continue;
						}
						this.curr = num;
						break;
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			this.curr = EditorGUILayout.Popup(EdSkillDefinitions.GC_Logic, this.curr, EdSkillDefinitions.edLabels);
			if (EditorGUI.EndChangeCheck())
			{
				this.CreateSkillLogic();
			}
			EditorGUILayout.Space();
			def.activationCost = EditorGUILayout.FloatField(EdSkillDefinitions.GC_ActivationCost, def.activationCost);
			EditorGUILayout.Space();
			if (this.curr >= 0 && this.currEd != null)
			{
				this.currEd.DrawEditor();
			}
		}

		private void CreateSkillLogic()
		{
			if (this.selected != null && this.curr >= 0)
			{
				string text = EdSkillDefinitions.edLabels[this.curr].text;
				Type t = null;
				foreach (SkillLogicEd value in EdSkillDefinitions.editors.Values)
				{
					if (value.nfo.Name.Equals(text))
					{
						t = value.nfo.TargetType;
						this.currEd = value;
						break;
					}
				}
				if ((UnityEngine.Object)this.selected.skillLogicDef != (UnityEngine.Object)null)
				{
					plyEdUtil.DeleteAsset(this.selected.skillLogicDef);
					this.selected.skillLogicDef = null;
				}
				plyEdUtil.CheckPath(BloxEdGlobal.ResourcesRoot + "Skills/");
				string fn = plyEdUtil.GenerateUniqueFilePath("", BloxEdGlobal.ResourcesRoot + "Skills/", ".asset");
				this.selected.skillLogicDef = (SkillLogic)plyEdUtil.LoadOrCreateAsset(t, fn, true);
				plyEdUtil.SetDirty(BloxEdGlobal.SkillDefs);
			}
		}
	}
}
