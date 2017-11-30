using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Type_Block))]
	public class Type_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			Type_Block type_Block = (Type_Block)bdi.b;
			return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "FindType", new CodePrimitiveExpression(type_Block.typeName));
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			return false;
		}
	}
}
