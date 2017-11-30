using plyLibEditor;
using System.CodeDom;

namespace BloxEditor
{
	public abstract class BloxBlockScriptGenerator : plyCustomEd
	{
		public abstract CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi);

		public abstract bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements);
	}
}
