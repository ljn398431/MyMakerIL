using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Array_Reverse_Block))]
	public class Array_Reverse_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(Array));
			if (codeExpression == null)
			{
				return null;
			}
			return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ArrayReverseRes", codeExpression);
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(Array));
			if (codeExpression == null)
			{
				return false;
			}
			statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ArrayReverse", codeExpression)));
			return true;
		}
	}
}
