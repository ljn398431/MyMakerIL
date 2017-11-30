using BloxEngine;
using System.CodeDom;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Debug_Block))]
	public class Debug_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			Debug_Block debug_Block = (Debug_Block)bdi.b;
			string text = "UnityEngine.Debug.";
			if (((bdi.paramBlocks != null) ? bdi.paramBlocks.Length : 0) != 0)
			{
				switch (debug_Block.logType)
				{
				case Debug_Block.DebugBlockLogType.Log:
					text += "LogFormat";
					break;
				case Debug_Block.DebugBlockLogType.Warning:
					text += "WarningFormat";
					break;
				case Debug_Block.DebugBlockLogType.Error:
					text += "ErrorFormat";
					break;
				}
				CodeExpression[] array = new CodeExpression[bdi.paramBlocks.Length + 1];
				string text2 = debug_Block.message;
				for (int i = 0; i < bdi.paramBlocks.Length; i++)
				{
					text2 = text2 + ((i == 0) ? " " : ", ") + "{" + i + "}";
					array[i + 1] = (BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[i], null) ?? new CodePrimitiveExpression("null"));
				}
				array[0] = new CodePrimitiveExpression(text2);
				statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(null, text, array)));
				return true;
			}
			switch (debug_Block.logType)
			{
			case Debug_Block.DebugBlockLogType.Log:
				text += "Log";
				break;
			case Debug_Block.DebugBlockLogType.Warning:
				text += "Warning";
				break;
			case Debug_Block.DebugBlockLogType.Error:
				text += "Error";
				break;
			}
			CodeExpression[] parameters = new CodeExpression[1]
			{
				new CodePrimitiveExpression(debug_Block.message)
			};
			statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(null, text, parameters)));
			return true;
		}
	}
}
