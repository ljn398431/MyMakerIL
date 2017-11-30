using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Loop_Block))]
	public class Loop_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression codeExpression = (bdi.paramBlocks[1] == null) ? new CodePrimitiveExpression(0) : BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], typeof(int));
			if (codeExpression == null)
			{
				throw new Exception("Error: startVal");
			}
			CodeExpression codeExpression2 = (bdi.paramBlocks[2] == null) ? new CodePrimitiveExpression(0) : BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[2], typeof(int));
			if (codeExpression2 == null)
			{
				throw new Exception("Error: endVal");
			}
			CodeStatement codeStatement = null;
			CodeExpression codeExpression3 = null;
			CodeStatement codeStatement2 = null;
			if (bdi.paramBlocks[0] == null)
			{
				string cleanEventVariableName = BloxScriptGenerator.GetCleanEventVariableName("", true);
				statements.Add(new CodeVariableDeclarationStatement(typeof(int), cleanEventVariableName, new CodePrimitiveExpression(0)));
				CodeVariableReferenceExpression left = new CodeVariableReferenceExpression(cleanEventVariableName);
				codeStatement = new CodeAssignStatement(left, codeExpression);
				codeExpression3 = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.LessThan, codeExpression2);
				codeStatement2 = new CodeAssignStatement(left, new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));
			}
			else
			{
				codeStatement = Variable_ScriptGenerator.CreateVarValueSetStatement(bdi.paramBlocks[0], codeExpression);
				if (codeStatement == null)
				{
					throw new Exception("Error: initSt");
				}
				CodeExpression codeExpression4 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(int));
				if (codeExpression4 == null)
				{
					throw new Exception("error: getVarVal");
				}
				codeExpression3 = new CodeBinaryOperatorExpression(codeExpression4, CodeBinaryOperatorType.LessThan, codeExpression2);
				codeStatement2 = Variable_ScriptGenerator.CreateVarValueSetStatement(bdi.paramBlocks[0], new CodeBinaryOperatorExpression(codeExpression4, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));
				if (codeExpression4 == null)
				{
					throw new Exception("error: incSt");
				}
			}
			CodeStatement[] array = BloxScriptGenerator.CreateChildBlockCodeStatements(bdi);
			if (array == null)
			{
				throw new Exception("Error: childStatements");
			}
			statements.Add(new CodeIterationStatement(codeStatement, codeExpression3, codeStatement2, array));
			return true;
		}
	}
}
