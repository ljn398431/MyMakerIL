using BloxEngine;
using System;
using System.CodeDom;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(SelfGameObject_Block))]
	public class SelfGameObject_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			Type type = (bdi.fieldIdx == -1) ? bdi.owningBlock.def.contextType : bdi.owningBlock.ParameterTypes()[bdi.fieldIdx];
			if (type != null && typeof(Component).IsAssignableFrom(type))
			{
				bdi.sgReturnType = type;
				return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetComponent", new CodeTypeReference(type)));
			}
			return new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "gameObject");
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			return false;
		}
	}
}
