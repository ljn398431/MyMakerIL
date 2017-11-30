using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(GotoFirst_Block))]
	public class GotoFirst_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			if (bdi.parentBlock != null && bdi.parentBlock.IsOrInLoop())
			{
				statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("continue")));
				return true;
			}
			statements.Add(BloxScriptGenerator.CreateGotoTopStatement());
			return true;
		}
	}
}
