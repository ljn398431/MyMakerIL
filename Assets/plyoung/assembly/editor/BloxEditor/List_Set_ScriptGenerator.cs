using BloxEngine;
using System.CodeDom;
using System.Collections;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(List_Set_Block))]
	public class List_Set_ScriptGenerator : BloxBlockScriptGenerator
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
			CodeExpression codeExpression3 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], null) ?? new CodePrimitiveExpression(null);
			CodeExpression expression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ListSet", codeExpression, codeExpression3, codeExpression2);
			statements.Add(new CodeExpressionStatement(expression));
			return true;
		}
	}
}
