using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(IF_Block))]
	public class IF_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(bool));
			if (codeExpression == null)
			{
				throw new Exception("error: condition");
			}
			CodeStatement[] array = BloxScriptGenerator.CreateChildBlockCodeStatements(bdi);
			if (array == null)
			{
				throw new Exception("error: trueStatements");
			}
			CodeStatement[] array2 = this.CreateFalseStatements(bdi);
			if (array2 == null)
			{
				throw new Exception("error: falseStatements");
			}
			statements.Add(new CodeConditionStatement(codeExpression, array, array2));
			return true;
		}

		private CodeStatement[] CreateFalseStatements(BloxBlockEd bdi)
		{
			if (bdi.next != null && bdi.next.b.GetType() == typeof(ElseIF_Block))
			{
				bdi = bdi.next;
				CodeStatement[] array = BloxScriptGenerator.CreateChildBlockCodeStatements(bdi);
				if (array == null)
				{
					return null;
				}
				if (bdi.paramBlocks[0] != null)
				{
					CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(bool));
					if (codeExpression == null)
					{
						return null;
					}
					CodeStatement[] array2 = this.CreateFalseStatements(bdi);
					if (array2 == null)
					{
						return null;
					}
					return new CodeStatement[1]
					{
						new CodeConditionStatement(codeExpression, array, array2)
					};
				}
				return array;
			}
			return new CodeStatement[0];
		}
	}
}
