using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(EQ_Block))]
	[BloxBlockScriptGenerator(typeof(NEQ_Block))]
	[BloxBlockScriptGenerator(typeof(GT_Block))]
	[BloxBlockScriptGenerator(typeof(GTE_Block))]
	[BloxBlockScriptGenerator(typeof(LT_Block))]
	[BloxBlockScriptGenerator(typeof(LTE_Block))]
	[BloxBlockScriptGenerator(typeof(AND_Block))]
	[BloxBlockScriptGenerator(typeof(OR_Block))]
	[BloxBlockScriptGenerator(typeof(NOT_Block))]
	public class ComparisonOps_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			Type type = bdi.b.GetType();
			if (type == typeof(NOT_Block))
			{
				CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(bool));
				if (codeExpression == null)
				{
					throw new Exception("error: value");
				}
				return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "NOT", codeExpression);
			}
			CodeBinaryOperatorType codeBinaryOperatorType = CodeBinaryOperatorType.ValueEquality;
			if (type == typeof(GT_Block))
			{
				codeBinaryOperatorType = CodeBinaryOperatorType.GreaterThan;
			}
			else if (type == typeof(GTE_Block))
			{
				codeBinaryOperatorType = CodeBinaryOperatorType.GreaterThanOrEqual;
			}
			else if (type == typeof(LT_Block))
			{
				codeBinaryOperatorType = CodeBinaryOperatorType.LessThan;
			}
			else if (type == typeof(LTE_Block))
			{
				codeBinaryOperatorType = CodeBinaryOperatorType.LessThanOrEqual;
			}
			else if (type == typeof(AND_Block))
			{
				codeBinaryOperatorType = CodeBinaryOperatorType.BooleanAnd;
			}
			else if (type == typeof(OR_Block))
			{
				codeBinaryOperatorType = CodeBinaryOperatorType.BooleanOr;
			}
			CodeExpression codeExpression2 = (bdi.paramBlocks[0] == null) ? new CodePrimitiveExpression(null) : BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], (codeBinaryOperatorType == CodeBinaryOperatorType.BooleanAnd || codeBinaryOperatorType == CodeBinaryOperatorType.BooleanOr) ? typeof(bool) : null);
			CodeExpression codeExpression3 = (bdi.paramBlocks[1] == null) ? new CodePrimitiveExpression(null) : BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], (codeBinaryOperatorType == CodeBinaryOperatorType.BooleanAnd || codeBinaryOperatorType == CodeBinaryOperatorType.BooleanOr) ? typeof(bool) : null);
			if (codeExpression2 == null)
			{
				throw new Exception("error: left expression");
			}
			if (codeExpression3 == null)
			{
				throw new Exception("error: right expression");
			}
			if (type == typeof(NEQ_Block))
			{
				CodeExpression codeExpression4 = new CodeBinaryOperatorExpression(codeExpression2, CodeBinaryOperatorType.ValueEquality, codeExpression3);
				return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "NOT", codeExpression4);
			}
			return new CodeBinaryOperatorExpression(codeExpression2, codeBinaryOperatorType, codeExpression3);
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			return false;
		}
	}
}
