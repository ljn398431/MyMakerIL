using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(WaitForEndOfFrame_Block))]
	public class WaitForEndOfFrame_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("yield return new UnityEngine.WaitForEndOfFrame()")));
			return true;
		}
	}
}
