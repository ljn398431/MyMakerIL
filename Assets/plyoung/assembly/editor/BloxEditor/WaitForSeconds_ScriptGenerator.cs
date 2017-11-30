using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(WaitForSeconds_Block))]
	public class WaitForSeconds_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			string text = BloxScriptGenerator.CodeSnippetFromCodeExpression(BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(float)) ?? new CodePrimitiveExpression(0f));
			if (text == null)
			{
				return false;
			}
			statements.Add(new CodeExpressionStatement(new CodeSnippetExpression(string.Format("yield return new UnityEngine.WaitForSeconds({0})", text))));
			return true;
		}
	}
}
