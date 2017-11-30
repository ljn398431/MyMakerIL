using plyLibEditor;
using System;

namespace BloxEditor.GameSystems
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class SkillLogicEdAttribute : plyCustomEdAttribute
	{
		public SkillLogicEdAttribute(Type targeType, string Name) : base(targeType)
		{
			base.Name = Name;
		}
	}
}
