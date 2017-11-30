using BloxEngine;
using System;
using System.CodeDom;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Array_Add_Block))]
	public class Array_Add_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			if (!Array_Add_ScriptGenerator.IsValidContext(bdi.contextBlock))
			{
				return false;
			}
			CodeExpression codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, typeof(Array));
			if (codeExpression == null)
			{
				return false;
			}
			CodeExpression codeExpression2 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], null) ?? new CodePrimitiveExpression(null);
			CodeExpression valueExpr = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(BloxUtil)), "ArrayAdd", codeExpression, codeExpression2);
			if (bdi.contextBlock.b.GetType() == typeof(Variable_Block))
			{
				statements.Add(Variable_ScriptGenerator.CreateVarValueSetStatement(bdi.contextBlock, valueExpr));
				return true;
			}
			statements.Add(BloxScriptGenerator.CreateMemberSetExpression(bdi.contextBlock, valueExpr, typeof(Array)));
			return true;
		}

		public static bool IsValidContext(BloxBlockEd context)
		{
			if (context != null)
			{
				if (context.b.GetType() == typeof(Variable_Block))
				{
					return true;
				}
				if (context.b.mi != null && context.b.mi.CanSetValue)
				{
					return true;
				}
			}
			Debug.LogError("The Context is invalid since it's value can't be changed/ updated.");
			return false;
		}
	}
}
