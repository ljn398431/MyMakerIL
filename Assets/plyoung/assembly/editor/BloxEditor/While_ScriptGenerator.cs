using BloxEngine;
using System;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(While_Block))]
	public class While_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(bool));
			if (codeExpression == null)
			{
				throw new Exception("error: condition");
			}
			CodeStatement[] array = BloxScriptGenerator.CreateChildBlockCodeStatements(bdi);
			if (array == null)
			{
				throw new Exception("Error: childStatements");
			}
			statements.Add(new CodeIterationStatement(new CodeSnippetStatement(""), codeExpression, new CodeSnippetStatement(""), array));
			return true;
		}
	}
}
