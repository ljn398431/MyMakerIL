using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(ADD_Block))]
	[BloxBlockScriptGenerator(typeof(SUB_Block))]
	[BloxBlockScriptGenerator(typeof(MUL_Block))]
	[BloxBlockScriptGenerator(typeof(DIV_Block))]
	[BloxBlockScriptGenerator(typeof(MOD_Block))]
	[BloxBlockScriptGenerator(typeof(B_AND_Block))]
	[BloxBlockScriptGenerator(typeof(B_OR_Block))]
	[BloxBlockScriptGenerator(typeof(B_XOR_Block))]
	[BloxBlockScriptGenerator(typeof(B_LS_Block))]
	[BloxBlockScriptGenerator(typeof(B_RS_Block))]
	public class MathsOps_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			CodeExpression codeExpression = (bdi.paramBlocks[0] == null) ? new CodePrimitiveExpression(null) : BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], null);
			CodeExpression codeExpression2 = (bdi.paramBlocks[1] == null) ? new CodePrimitiveExpression(null) : BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], null);
			if (codeExpression == null)
			{
				throw new Exception("error: left expression");
			}
			if (codeExpression2 == null)
			{
				throw new Exception("error: right expression");
			}
			Type type = bdi.b.GetType();
			CodeBinaryOperatorType op = CodeBinaryOperatorType.Add;
			if (type == typeof(SUB_Block))
			{
				op = CodeBinaryOperatorType.Subtract;
			}
			else if (type == typeof(MUL_Block))
			{
				op = CodeBinaryOperatorType.Multiply;
			}
			else if (type == typeof(DIV_Block))
			{
				op = CodeBinaryOperatorType.Divide;
			}
			else if (type == typeof(MOD_Block))
			{
				op = CodeBinaryOperatorType.Modulus;
			}
			else if (type == typeof(B_AND_Block))
			{
				op = CodeBinaryOperatorType.BitwiseAnd;
			}
			else if (type == typeof(B_OR_Block))
			{
				op = CodeBinaryOperatorType.BitwiseOr;
			}
			else
			{
				if (type == typeof(B_XOR_Block))
				{
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxMathsUtil)), "BitwiseXor", codeExpression, codeExpression2);
				}
				if (type == typeof(B_RS_Block))
				{
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxMathsUtil)), "RightShift", codeExpression, codeExpression2);
				}
				if (type == typeof(B_LS_Block))
				{
					return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxMathsUtil)), "LeftShift", codeExpression, codeExpression2);
				}
			}
			bdi.sgReturnType = ((bdi.paramBlocks[0] != null) ? bdi.paramBlocks[0].sgReturnType : ((bdi.paramBlocks[1] != null) ? bdi.paramBlocks[1].sgReturnType : typeof(object)));
			return new CodeBinaryOperatorExpression(codeExpression, op, codeExpression2);
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			return false;
		}
	}
}
