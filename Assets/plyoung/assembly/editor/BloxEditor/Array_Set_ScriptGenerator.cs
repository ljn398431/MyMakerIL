using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Array_Set_Block))]
	public class Array_Set_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			if (!Array_Add_ScriptGenerator.IsValidContext(bdi.contextBlock))
			{
				return false;
			}
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(Array));
			if (codeExpression == null)
			{
				return false;
			}
			CodeExpression codeExpression2 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(int)) ?? new CodePrimitiveExpression(0);
			CodeExpression codeExpression3 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], null) ?? new CodePrimitiveExpression(null);
			CodeExpression valueExpr = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ArraySet", codeExpression, codeExpression3, codeExpression2);
			if (bdi.contextBlock.b.GetType() == typeof(Variable_Block))
			{
				statements.Add(Variable_ScriptGenerator.CreateVarValueSetStatement(bdi.contextBlock, valueExpr));
				return true;
			}
			statements.Add(BloxScriptGenerator.CreateMemberSetExpression(bdi.contextBlock, valueExpr, typeof(Array)));
			return true;
		}
	}
}
