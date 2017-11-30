using BloxEngine;
using System.CodeDom;
using System.Collections;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(List_RemoveAt_Block))]
	public class List_RemoveAt_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(IList));
			if (codeExpression == null)
			{
				return false;
			}
			CodeExpression codeExpression2 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(int)) ?? new CodePrimitiveExpression(0);
			CodeExpression expression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ListRemoveAt", codeExpression, codeExpression2);
			statements.Add(new CodeExpressionStatement(expression));
			return true;
		}
	}
}
