using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Comment_Block))]
	public class Comment_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			Comment_Block comment_Block = (Comment_Block)bdi.b;
			if (!string.IsNullOrEmpty(comment_Block.message))
			{
				statements.Add(new CodeCommentStatement(comment_Block.message));
			}
			return true;
		}
	}
}
