using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Break_Block))]
	public class Break_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			if (bdi.parentBlock != null && bdi.parentBlock.IsOrInLoop())
			{
				statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("break")));
				return true;
			}
			statements.Add(new CodeMethodReturnStatement());
			return true;
		}
	}
}
