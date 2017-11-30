using BloxEngine;
using System.CodeDom;
using System.Collections;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(List_Clone_Block))]
	public class List_Clone_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(IList));
			if (codeExpression == null)
			{
				return null;
			}
			return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ListClone", codeExpression);
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			return false;
		}
	}
}
