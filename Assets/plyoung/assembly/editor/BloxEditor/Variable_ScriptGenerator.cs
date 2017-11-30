using BloxEngine;
using BloxEngine.Variables;
using System;
using System.CodeDom;
using UnityEngine;

namespace BloxEditor
{
	[BloxBlockScriptGenerator(typeof(Variable_Block))]
	public class Variable_ScriptGenerator : BloxBlockScriptGenerator
	{
		public override CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi)
		{
			Variable_Block variable_Block = (Variable_Block)bdi.b;
			plyVar plyVar = null;
			if (!this.UpdateBlockReturnType(bdi, ref plyVar))
			{
				Debug.LogErrorFormat("The [{0}] variable [{1}] is not defined.", variable_Block.varType, variable_Block.varName);
				return null;
			}
			if (variable_Block.varType == plyVariablesType.Event)
			{
				if (BloxScriptGenerator.processingEvent.def.pars.Length != 0)
				{
					BloxEventDef.Param[] pars = BloxScriptGenerator.processingEvent.def.pars;
					for (int i = 0; i < pars.Length; i++)
					{
						BloxEventDef.Param param = pars[i];
						if (param.name == variable_Block.varName)
						{
							bdi.sgReturnType = param.type;
							return new CodeVariableReferenceExpression(variable_Block.varName);
						}
					}
				}
				string cleanEventVariableName = BloxScriptGenerator.GetCleanEventVariableName(variable_Block.varName, false);
				if (BloxScriptGenerator.processingEvent.ev.ident == "Custom" && cleanEventVariableName.StartsWith("param") && cleanEventVariableName.Length > 5)
				{
					int argIdx = -1;
					if (int.TryParse(cleanEventVariableName.Substring(5), out argIdx))
					{
						Type type = (bdi.fieldIdx == -1) ? bdi.owningBlock.def.contextType : bdi.owningBlock.ParameterTypes()[bdi.fieldIdx];
						if (type == null)
						{
							type = typeof(object);
						}
						BloxScriptGenerator.AddEventVariable(cleanEventVariableName, type, null, argIdx, false);
					}
				}
				else
				{
					Type type2 = (bdi.fieldIdx == -1) ? bdi.owningBlock.def.contextType : bdi.owningBlock.ParameterTypes()[bdi.fieldIdx];
					if (type2 == null)
					{
						type2 = typeof(object);
					}
					BloxScriptGenerator.AddEventVariable(cleanEventVariableName, type2, null, -1, false);
				}
				Type eventVariableType = BloxScriptGenerator.GetEventVariableType(cleanEventVariableName);
				if (eventVariableType != null)
				{
					bdi.sgReturnType = eventVariableType;
				}
				return new CodeVariableReferenceExpression(cleanEventVariableName);
			}
			if (variable_Block.varType == plyVariablesType.Blox)
			{
				CodeExpression targetObject = new CodeVariableReferenceExpression(BloxScriptGenerator.GetCleanBloxVariableName(variable_Block.varName));
				Type type3 = (bdi.fieldIdx == -1) ? bdi.owningBlock.def.contextType : bdi.owningBlock.ParameterTypes()[bdi.fieldIdx];
				if (type3 != null)
				{
					plyVarValueHandler valueHandler = plyVar.ValueHandler;
					Type type4 = (valueHandler != null) ? valueHandler.GetType() : null;
					if (type4 == typeof(plyVar_GameObject) || type4 == typeof(plyVar_Component))
					{
						if (typeof(GameObject).IsAssignableFrom(type3))
						{
							bdi.sgReturnType = type3;
							return new CodeMethodInvokeExpression(targetObject, "GetGameObject");
						}
						if (typeof(Component).IsAssignableFrom(type3))
						{
							bdi.sgReturnType = type3;
							return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject, "GetComponent", new CodeTypeReference(type3)));
						}
					}
					else if (plyVar.ValueHandler.GetType() == typeof(plyVar_UnityObject))
					{
						bdi.sgReturnType = type3;
						return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject, "GetObject", new CodeTypeReference(type3)));
					}
				}
				return new CodeMethodInvokeExpression(targetObject, "GetValue");
			}
			if (variable_Block.varType == plyVariablesType.Object)
			{
				CodeExpression targetObject2 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], null) ?? new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "gameObject");
				targetObject2 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject2, "GetComponent", new CodeTypeReference(typeof(ObjectVariables))));
				Type type5 = (bdi.fieldIdx == -1) ? bdi.owningBlock.def.contextType : bdi.owningBlock.ParameterTypes()[bdi.fieldIdx];
				if (type5 != null)
				{
					if (typeof(GameObject).IsAssignableFrom(type5))
					{
						bdi.sgReturnType = type5;
						return new CodeMethodInvokeExpression(targetObject2, "GetGameObjectVarValue", new CodePrimitiveExpression(variable_Block.varName));
					}
					if (typeof(Component).IsAssignableFrom(type5))
					{
						bdi.sgReturnType = type5;
						return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject2, "GetComponentVarValue", new CodeTypeReference(type5)), new CodePrimitiveExpression(variable_Block.varName));
					}
					if (typeof(UnityEngine.Object).IsAssignableFrom(type5))
					{
						bdi.sgReturnType = type5;
						return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject2, "GetUnityObjectVarValue", new CodeTypeReference(type5)), new CodePrimitiveExpression(variable_Block.varName));
					}
				}
				return new CodeMethodInvokeExpression(targetObject2, "GetVarValue", new CodePrimitiveExpression(variable_Block.varName));
			}
			if (variable_Block.varType == plyVariablesType.Global)
			{
				CodeExpression targetObject3 = new CodeVariableReferenceExpression(BloxScriptGenerator.GetCleanGlobalVariableName(variable_Block.varName));
				Type type6 = (bdi.fieldIdx == -1) ? bdi.owningBlock.def.contextType : bdi.owningBlock.ParameterTypes()[bdi.fieldIdx];
				if (type6 != null)
				{
					plyVarValueHandler valueHandler2 = plyVar.ValueHandler;
					Type type7 = (valueHandler2 != null) ? valueHandler2.GetType() : null;
					if (type7 == typeof(plyVar_GameObject) || type7 == typeof(plyVar_Component))
					{
						if (typeof(GameObject).IsAssignableFrom(type6))
						{
							bdi.sgReturnType = type6;
							return new CodeMethodInvokeExpression(targetObject3, "GetGameObject");
						}
						if (typeof(Component).IsAssignableFrom(type6))
						{
							bdi.sgReturnType = type6;
							return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject3, "GetComponent", new CodeTypeReference(type6)));
						}
					}
					else if (plyVar.ValueHandler.GetType() == typeof(plyVar_UnityObject))
					{
						bdi.sgReturnType = type6;
						return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject3, "GetObject", new CodeTypeReference(type6)));
					}
				}
				return new CodeMethodInvokeExpression(targetObject3, "GetValue");
			}
			return null;
		}

		public override bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			CodeExpression valueExpr = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], null) ?? new CodePrimitiveExpression(null);
			statements.Add(Variable_ScriptGenerator.CreateVarValueSetStatement(bdi, valueExpr));
			return true;
		}

		public static CodeStatement CreateVarValueSetStatement(BloxBlockEd bdi, CodeExpression valueExpr)
		{
			Variable_Block variable_Block = (Variable_Block)bdi.b;
			Type type = (bdi.paramBlocks[0] == null) ? typeof(object) : bdi.paramBlocks[0].sgReturnType;
			if (variable_Block.varType == plyVariablesType.Event)
			{
				string cleanEventVariableName = BloxScriptGenerator.GetCleanEventVariableName(variable_Block.varName, false);
				if (BloxScriptGenerator.AddEventVariable(cleanEventVariableName, type, null, -1, true))
				{
					return new CodeAssignStatement(new CodeVariableReferenceExpression(cleanEventVariableName), valueExpr);
				}
				return new CodeVariableDeclarationStatement(type, cleanEventVariableName, valueExpr);
			}
			if (variable_Block.varType == plyVariablesType.Blox)
			{
				return new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(BloxScriptGenerator.GetCleanBloxVariableName(variable_Block.varName)), "SetValue", valueExpr));
			}
			if (variable_Block.varType == plyVariablesType.Object)
			{
				return new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[1], null) ?? new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "gameObject"), "GetComponent", new CodeTypeReference(typeof(ObjectVariables)))), "SetVarValue", new CodePrimitiveExpression(variable_Block.varName), valueExpr));
			}
			if (variable_Block.varType == plyVariablesType.Global)
			{
				return new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(GlobalVariables)), "Instance"), "SetVarValue", new CodePrimitiveExpression(variable_Block.varName), valueExpr));
			}
			return null;
		}

		private bool UpdateBlockReturnType(BloxBlockEd bdi, ref plyVar theVar)
		{
			Variable_Block variable_Block = (Variable_Block)bdi.b;
			if (variable_Block.varType == plyVariablesType.Blox)
			{
				plyVar plyVar = theVar = this.FindVariable(BloxScriptGenerator.processingBlox.variables, variable_Block.varName);
				if (plyVar == null)
				{
					return false;
				}
				if (plyVar.variableType != null)
				{
					bdi.sgReturnType = plyVar.variableType;
				}
			}
			else if (variable_Block.varType == plyVariablesType.Global)
			{
				plyVar plyVar2 = theVar = this.FindVariable(BloxEd.GlobalVarsObj.variables, variable_Block.varName);
				if (plyVar2 == null)
				{
					return false;
				}
				if (plyVar2.variableType != null)
				{
					bdi.sgReturnType = plyVar2.variableType;
				}
			}
			return true;
		}

		private plyVar FindVariable(plyVariables vars, string name)
		{
			for (int i = 0; i < vars.varDefs.Count; i++)
			{
				if (vars.varDefs[i].name == name)
				{
					return vars.varDefs[i];
				}
			}
			return null;
		}
	}
}
