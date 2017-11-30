using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(WaitForFixedUpdate_Block))]
	public class WaitForFixedUpdate_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("yield return new UnityEngine.WaitForFixedUpdate()")));
			return true;
		}
	}
}
