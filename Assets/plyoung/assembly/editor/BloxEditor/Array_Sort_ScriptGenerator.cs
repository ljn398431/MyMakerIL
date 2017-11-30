using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Array_Sort_Block))]
	public class Array_Sort_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(Array));
			if (codeExpression == null)
			{
				return null;
			}
			return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ArraySortRes", codeExpression);
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(Array));
			if (codeExpression == null)
			{
				return false;
			}
			statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ArraySort", codeExpression)));
			return true;
		}
	}
}
