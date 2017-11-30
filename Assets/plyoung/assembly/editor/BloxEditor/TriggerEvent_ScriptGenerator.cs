using BloxEngine;
using System.CodeDom;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(TriggerEvent_Block))]
	public class TriggerEvent_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression codeExpression = null;
			if (bdi.paramBlocks[1] == null)
			{
				codeExpression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "bloxContainer");
			}
			else
			{
				CodeExpression codeExpression2 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], null);
				if (codeExpression2 == null)
				{
					return false;
				}
				codeExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "GetComponent", new CodeTypeReference(typeof(BloxContainer))), codeExpression2);
			}
			CodeExpression[] array = new CodeExpression[(bdi.paramBlocks.Length > 3) ? 3 : 2];
			array[0] = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], typeof(string));
			if (array[0] == null)
			{
				Debug.LogError("Event name missing.");
				return false;
			}
			array[1] = (BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[2], typeof(float)) ?? new CodePrimitiveExpression(0f));
			if (bdi.paramBlocks.Length > 3)
			{
				CodeExpression[] array2 = new CodeExpression[bdi.paramBlocks.Length - 3];
				for (int i = 3; i < bdi.paramBlocks.Length; i++)
				{
					CodeExpression codeExpression3 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[i], null) ?? new CodePrimitiveExpression(null);
					array2[i - 3] = new CodeObjectCreateExpression(typeof(BloxEventArg), new CodePrimitiveExpression("param" + (i - 3)), codeExpression3);
				}
				array[2] = new CodeArrayCreateExpression(typeof(BloxEventArg), array2);
			}
			statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(codeExpression, "TriggerEvent", array)));
			return true;
		}
	}
}
