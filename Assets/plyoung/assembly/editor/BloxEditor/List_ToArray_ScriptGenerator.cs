using BloxEngine;
using System.CodeDom;
using System.Collections;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(List_ToArray_Block))]
	public class List_ToArray_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(IList));
			if (codeExpression == null)
			{
				return null;
			}
			return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ListToList", codeExpression);
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			return false;
		}
	}
}
