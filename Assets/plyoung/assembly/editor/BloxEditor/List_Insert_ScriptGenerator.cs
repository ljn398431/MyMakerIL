using BloxEngine;
using System.CodeDom;
using System.Collections;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(List_Insert_Block))]
	public class List_Insert_ScriptGenerator : BloxBlockScriptGenerator
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
			CodeExpression codeExpression2 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], null) ?? new CodePrimitiveExpression(null);
			CodeExpression codeExpression3 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], typeof(int)) ?? new CodePrimitiveExpression(0);
			CodeExpression expression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ListInsert", codeExpression, codeExpression2, codeExpression3);
			statements.Add(new CodeExpressionStatement(expression));
			return true;
		}
	}
}
