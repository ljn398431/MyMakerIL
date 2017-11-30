using BloxEngine;
using System;
using System.CodeDom;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(DoAfterTimeout_Block))]
	public class DoAfterTimeout_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			if (bdi.paramBlocks[0] != null && bdi.paramBlocks[0].b.GetType() == typeof(Variable_Block))
			{
				CodeExpression left = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(float));
				CodePrimitiveExpression codePrimitiveExpression = new CodePrimitiveExpression(0f);
				CodeStatement codeStatement = Variable_ScriptGenerator.CreateVarValueSetStatement(bdi.paramBlocks[0], codePrimitiveExpression);
				if (codeStatement == null)
				{
					throw new Exception("error: setVar0F");
				}
				CodeStatement codeStatement2 = Variable_ScriptGenerator.CreateVarValueSetStatement(bdi.paramBlocks[0], new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Time)), "deltaTime")));
				if (codeStatement2 == null)
				{
					throw new Exception("error: setVarDeltaTime");
				}
				CodeStatement[] array = BloxScriptGenerator.CreateChildBlockCodeStatements(bdi);
				if (array == null)
				{
					throw new Exception("error: childStatements");
				}
				ArrayUtility.Insert(ref array, 0, codeStatement);
				CodeStatement codeStatement3 = new CodeConditionStatement(new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.LessThanOrEqual, codePrimitiveExpression), array);
				CodeStatement value = new CodeConditionStatement(new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.GreaterThan, codePrimitiveExpression), codeStatement2, codeStatement3);
				statements.Add(value);
				return true;
			}
			Debug.LogError("Invalid variable type");
			return false;
		}
	}
}
