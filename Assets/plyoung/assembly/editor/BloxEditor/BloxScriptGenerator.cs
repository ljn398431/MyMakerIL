using BloxEngine;
using BloxEngine.Variables;
using Microsoft.CSharp;
using plyLibEditor;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[InitializeOnLoad]
	public class BloxScriptGenerator
	{
		private class EventInfo
		{
			public BloxEventEd evi = new BloxEventEd();

			public string methodName;

			public string customName;

			public override string ToString()
			{
				return this.methodName;
			}
		}

		private class EventVariableInfo
		{
			public string name;

			public Type type;

			public CodeExpression initExpr;

			public int argIdx;

			public bool isCreated;

			public EventVariableInfo(string name, Type type, CodeExpression initExpr, int argIdx, bool isCreated)
			{
				this.name = name;
				this.type = type;
				this.initExpr = initExpr;
				this.argIdx = argIdx;
				this.isCreated = isCreated;
			}
		}

		private static readonly GUIContent GC_Compile1;

		private static readonly GUIContent GC_Compile2;

		private static Regex nameRegex;

		private static Dictionary<Type, BloxBlockScriptGenerator> generators;

		private static List<string> scriptFiles;

		public static Blox processingBlox;

		public static BloxEventEd processingEvent;

		private static CodeMemberMethod processingMethod;

		private static CodeMemberMethod awakeMethod;

		private static bool addMethodTopLabel;

		private static Dictionary<string, EventVariableInfo> eventVars;

		private static Dictionary<string, string> eventVarNames;

		private static Dictionary<string, string> bloxVarNames;

		private static Dictionary<string, string> globalVarNames;

		private static CodeGeneratorOptions options;

		private static List<string> usedAssemblies;

		private static bool enviroChecked;

		static BloxScriptGenerator()
		{
			BloxScriptGenerator.GC_Compile1 = new GUIContent(Ico._sync + " Compile", "Compile this Blox into a Script. This is only needed if you want scripts, else the Blox runtime will execute this Blox.");
			BloxScriptGenerator.GC_Compile2 = new GUIContent(Ico._sync_problem + " Compile", "The Blox changed. It must be Compiled else the Blox runtime will be used rather than the existing Script (if any present). This is only needed if you want scripts.");
			BloxScriptGenerator.nameRegex = new Regex("[\\W_]+");
			BloxScriptGenerator.scriptFiles = new List<string>();
			BloxScriptGenerator.processingBlox = null;
			BloxScriptGenerator.processingEvent = null;
			BloxScriptGenerator.processingMethod = null;
			BloxScriptGenerator.awakeMethod = null;
			BloxScriptGenerator.addMethodTopLabel = false;
			BloxScriptGenerator.eventVars = null;
			BloxScriptGenerator.eventVarNames = null;
			BloxScriptGenerator.bloxVarNames = null;
			BloxScriptGenerator.globalVarNames = null;
			BloxScriptGenerator.options = new CodeGeneratorOptions
			{
				BracingStyle = "C",
				BlankLinesBetweenMembers = false
			};
			BloxScriptGenerator.usedAssemblies = new List<string>();
			BloxScriptGenerator.enviroChecked = false;
			BloxEditorWindow.ToolbarDrawCallbacks = (Action<Blox>)Delegate.Combine(BloxEditorWindow.ToolbarDrawCallbacks, new Action<Blox>(BloxScriptGenerator.OnToolbarButton));
			BloxScriptGenerator.generators = plyCustomEd.CreateCustomEditorsDict<BloxBlockScriptGenerator>(typeof(BloxBlockScriptGeneratorAttribute));
		}

		public static void GenerateAllScripts()
		{
			BloxScriptGenerator.CheckEnviro();
			plyEdUtil.ClearUnityConsole();
			BloxScriptGenerator.RemoveAllScripts();
			if (BloxEd.BloxGlobalObj.bloxDefs.Count == 0)
			{
				EditorUtility.DisplayDialog("Script-gen", "There are no Blox Definitions.", "OK");
			}
			else
			{
                Debug.Log("GenerateAllScripts ", "BloxEditor.BloxScriptGenerator", Color.green);
                BloxEd.Instance.LoadBlockDefs(false);
				BloxEd.Instance.LoadEventDefs();
				float num = 0f;
				float num2 = 0.05f;
				EditorUtility.DisplayProgressBar("Script-gen", "Loading definitions.", num);
				while (true)
				{
					if (!BloxEd.Instance.BlockDefsLoading && !BloxEd.Instance.EventDefsLoading)
						break;
					BloxEd.Instance.DoUpdate();
					num += num2;
					if (num > 1.0)
					{
						num = 0f;
					}
					EditorUtility.DisplayProgressBar("Script-gen", "Loading definitions.", num);
				}
				EditorUtility.ClearProgressBar();
				bool flag = false;
				num = 0f;
				num2 = (float)(1.0 / (float)BloxEd.BloxGlobalObj.bloxDefs.Count);
				EditorUtility.DisplayProgressBar("Script-gen", "", num);
				foreach (Blox bloxDef in BloxEd.BloxGlobalObj.bloxDefs)
				{
					try
					{
						num += num2;
						EditorUtility.DisplayProgressBar("Script-gen", "Processing: " + bloxDef.screenName, num);
						if (!BloxScriptGenerator.CreateScript(bloxDef))
						{
							flag = true;
						}
					}
					catch (Exception exception)
					{
						flag = true;
						Debug.LogException(exception);
					}
				}
				EditorUtility.ClearProgressBar();
				if (flag)
				{
					BloxScriptGenerator.ShowCompileError();
				}
				BloxScriptGenerator.CompileScripts();
				BloxEditorWindow.Reload();
			}
		}

		public static void RemoveAllScripts()
		{
			plyEdUtil.ClearUnityConsole();
			if (plyEdUtil.RelativePathExist(BloxEdGlobal.ScriptPath))
			{
				plyEdUtil.CheckPath(BloxEdGlobal.ScriptPath);
				string[] files = Directory.GetFiles(plyEdUtil.ProjectFullPath + BloxEdGlobal.ScriptPath);
				for (int i = 0; i < files.Length; i++)
				{
					AssetDatabase.DeleteAsset(plyEdUtil.ProjectRelativePath(files[i]));
				}
				AssetDatabase.Refresh();
			}
		}

		private static void OnToolbarButton(Blox blox)
		{
			if (!BloxEd.Instance.BlockDefsLoading)
			{
				BloxScriptGenerator.CheckEnviro();
				if (GUILayout.Button(blox.scriptDirty ? BloxScriptGenerator.GC_Compile2 : BloxScriptGenerator.GC_Compile1, plyEdGUI.Styles.ToolbarButton))
				{
					plyEdUtil.ClearUnityConsole();
					bool flag = false;
					EditorUtility.DisplayProgressBar("Script-gen", "Processing: " + blox.screenName, 0.1f);
					try
					{
						if (!BloxScriptGenerator.CreateScript(blox))
						{
							flag = true;
						}
					}
					catch (Exception exception)
					{
						flag = true;
						Debug.LogException(exception);
					}
					EditorUtility.ClearProgressBar();
					if (flag)
					{
						BloxScriptGenerator.ShowCompileError();
					}
					BloxScriptGenerator.CompileScripts();
					BloxEditorWindow.Reload();
				}
			}
		}

		private static void ShowCompileError()
		{
			EditorUtility.DisplayDialog("Script-gen", "There were errors while generating the Blox script(s). Please see the messages in the Unity console for more information.\n\nMake sure the Blox works fine at runtime when uncompiled and fix any errors that the Blox runtime might show in the console. If that is not the problem then it is possible that you are dealing with data which Blox is able to automatically convert between various data types when assigning values or doing other operations. This is not possible in the generated scripts for script runtime performance.\n\nIf you can't change the way the Block(s) are used you can simply leave the Blox in an uncompiled state and let the Blox runtime execute it. Blox is able to work with a mix of Blox runtime and generated scripts. ", "OK");
		}

		private static void CheckEnviro()
		{
			if (!BloxScriptGenerator.enviroChecked)
			{
				BloxScriptGenerator.enviroChecked = true;
				string text = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
				string text2 = Path.Combine(EditorApplication.applicationContentsPath, "Mono/bin").Replace("\\", "/");
				string text3 = Path.Combine(EditorApplication.applicationContentsPath, "Frameworks/Mono/bin").Replace("\\", "/");
				if (!text.Contains(text2))
				{
					text = text + ":" + text2;
				}
				Environment.SetEnvironmentVariable("PATH", text, EnvironmentVariableTarget.Process);
				if (!text.Contains(text3))
				{
					text = text + ":" + text3;
				}
				Environment.SetEnvironmentVariable("PATH", text, EnvironmentVariableTarget.Process);
			}
		}

		private static void CompileScripts()
		{
			foreach (string scriptFile in BloxScriptGenerator.scriptFiles)
			{
				AssetDatabase.ImportAsset(scriptFile, ImportAssetOptions.ForceUpdate);
			}
			BloxScriptGenerator.scriptFiles.Clear();
			AssetDatabase.Refresh();
		}

		private static bool CreateScript(Blox blox)
		{
			string text = BloxEdGlobal.GUIDScriptNames ? blox.ident : (BloxScriptGenerator.CleanMemberName(blox.screenName) + blox.ident.Substring(0, 5));
			Debug.Log("Script-gen: [" + blox.screenName + " => " + text + "]");
			BloxEditorWindow instance = BloxEditorWindow.Instance;
			if ((object)instance != null)
			{
				instance.UnselectAll();
			}
			plyEdUtil.CheckPath(BloxEdGlobal.ScriptPath);
			string str = BloxEdGlobal.ScriptPath + text + ".cs";
			blox.Serialize();
			blox.Deserialize();
			BloxScriptGenerator.processingBlox = blox;
			BloxScriptGenerator.bloxVarNames = new Dictionary<string, string>();
			BloxScriptGenerator.globalVarNames = new Dictionary<string, string>();
			BloxScriptGenerator.awakeMethod = null;
			BloxScriptGenerator.usedAssemblies = new List<string>();
			BloxScriptGenerator.usedAssemblies.Add(typeof(object).Assembly.Location);
			BloxScriptGenerator.usedAssemblies.Add(typeof(UnityEngine.Object).Assembly.Location);
			BloxScriptGenerator.usedAssemblies.Add(typeof(Blox).Assembly.Location);
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			CodeNamespace codeNamespace = new CodeNamespace("BloxGenerated");
			codeNamespace.Comments.Add(new CodeCommentStatement("Blox Name: " + blox.screenName));
			codeNamespace.Comments.Add(new CodeCommentStatement("Blox Ident: " + blox.ident));
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(text);
			codeTypeDeclaration.IsClass = true;
			codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("BloxEngine.ExcludeFromBlox"));
			codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("UnityEngine.AddComponentMenu", new CodeAttributeArgument(new CodePrimitiveExpression(""))));
			codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(typeof(MonoBehaviour)));
			codeNamespace.Types.Add(codeTypeDeclaration);
			codeCompileUnit.Namespaces.Add(codeNamespace);
			codeTypeDeclaration.Members.Add(new CodeMemberField
			{
				Name = "bloxContainer",
				Attributes = MemberAttributes.Public,
				Type = new CodeTypeReference(typeof(BloxContainer))
			});
			if (!BloxScriptGenerator.ProcessEvents(blox, codeTypeDeclaration))
			{
				if (File.Exists(plyEdUtil.ProjectFullPath + str))
				{
					File.WriteAllText(plyEdUtil.ProjectFullPath + str, " ");
				}
				blox.scriptDirty = true;
				plyEdUtil.SetDirty(blox);
				BloxScriptGenerator.processingBlox = null;
				BloxScriptGenerator.processingEvent = null;
				return false;
			}
			BloxScriptGenerator.InsertBloxVariables(codeTypeDeclaration);
			BloxScriptGenerator.InsertGlobalVariables(codeTypeDeclaration);
			BloxScriptGenerator.AddInitCodeToAwakeMethod();
			if (BloxScriptGenerator.CodeHasErrors(blox, codeCompileUnit) && !BloxEdGlobal.SaveBrokenScripts)
			{
				if (File.Exists(plyEdUtil.ProjectFullPath + str))
				{
					File.WriteAllText(plyEdUtil.ProjectFullPath + str, " ");
				}
				blox.scriptDirty = true;
				plyEdUtil.SetDirty(blox);
				BloxScriptGenerator.processingBlox = null;
				BloxScriptGenerator.processingEvent = null;
				return false;
			}
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(plyEdUtil.ProjectFullPath + str, false))
				{
					streamWriter.WriteLine("#pragma warning disable 0414, 0219, 0162, 0429, 0649");
					IndentedTextWriter indentedTextWriter = new IndentedTextWriter(streamWriter, "    ");
					CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
					cSharpCodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, indentedTextWriter, BloxScriptGenerator.options);
					indentedTextWriter.Close();
					cSharpCodeProvider.Dispose();
				}
				Debug.Log(" ... script successfully generated: " + text);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				if (!BloxEdGlobal.SaveBrokenScripts && File.Exists(plyEdUtil.ProjectFullPath + str))
				{
					File.WriteAllText(plyEdUtil.ProjectFullPath + str, " ");
				}
				blox.scriptDirty = true;
				plyEdUtil.SetDirty(blox);
				BloxScriptGenerator.processingBlox = null;
				BloxScriptGenerator.processingEvent = null;
				return false;
			}
			blox.scriptDirty = false;
			plyEdUtil.SetDirty(blox);
			BloxScriptGenerator.processingBlox = null;
			BloxScriptGenerator.processingEvent = null;
			return true;
		}

		private static bool CodeHasErrors(Blox blox, CodeCompileUnit codeUnit)
		{
			CompilerParameters compilerParameters = new CompilerParameters();
			foreach (string usedAssembly in BloxScriptGenerator.usedAssemblies)
			{
				compilerParameters.ReferencedAssemblies.Add(usedAssembly);
			}
			try
			{
				CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
				CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromDom(compilerParameters, codeUnit);
				cSharpCodeProvider.Dispose();
				if (compilerResults.Errors.Count > 0)
				{
					bool flag = true;
					foreach (CompilerError error in compilerResults.Errors)
					{
						if (!error.IsWarning)
						{
							flag = false;
							Debug.LogError("Compile error: " + error.ErrorText);
							break;
						}
					}
					if (!flag)
					{
						return true;
					}
				}
			}
			catch (Exception exception)
			{
				EditorUtility.DisplayDialog("Script-gen", "An error occurred while trying to check the generated script. See Unity console for more info and ask on the support forum if you need further help with this problem.", "OK");
				Debug.LogException(exception);
				return true;
			}
			return false;
		}

		private static bool ProcessEvents(Blox blox, CodeTypeDeclaration c)
		{
			List<string> list = new List<string>();
			Dictionary<string, List<EventInfo>> dictionary = new Dictionary<string, List<EventInfo>>();
			BloxEvent[] events = blox.events;
			for (int i = 0; i < events.Length; i++)
			{
				BloxEvent bloxEvent = events[i];
				if (bloxEvent.active)
				{
					string key = bloxEvent.ident;
					EventInfo eventInfo = new EventInfo();
					eventInfo.evi.Set(bloxEvent, true);
					do
					{
					}
					while (!eventInfo.evi.CheckEventBlockDefs());
					if (eventInfo.evi.hasUndefinedblocks)
					{
						Debug.LogErrorFormat("There are undefined Blocks in the Event [{0}] of the Blox [{1} => {2}]", eventInfo.evi.ev.screenName, blox.screenName, blox.ident);
						return false;
					}
					string text = BloxScriptGenerator.CleanMemberName(bloxEvent.screenName);
					EventInfo obj = eventInfo;
					string str = text;
					Guid guid = Guid.NewGuid();
					obj.methodName = "_" + str + "_" + guid.ToString("N").Substring(0, 4);
					while (!plyEdUtil.StringIsUnique(list, eventInfo.methodName))
					{
						EventInfo obj2 = eventInfo;
						string str2 = text;
						guid = Guid.NewGuid();
						obj2.methodName = "_" + str2 + "_" + guid.ToString("N").Substring(0, 4);
					}
					list.Add(eventInfo.methodName);
					if (bloxEvent.ident == "Custom")
					{
						eventInfo.customName = bloxEvent.screenName;
						key = "@Custom/" + eventInfo.customName;
					}
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, new List<EventInfo>());
					}
					dictionary[key].Add(eventInfo);
				}
			}
			foreach (KeyValuePair<string, List<EventInfo>> item in dictionary)
			{
				BloxEventDef def = item.Value[0].evi.def;
				if (item.Value.Count > 1 || (def.ident != "Custom" && BloxScriptGenerator.CleanMemberName(item.Value[0].evi.ev.screenName) != def.methodNfo.Name))
				{
					EventInfo eventInfo2 = item.Value[0];
					CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
					c.Members.Add(codeMemberMethod);
					codeMemberMethod.Name = ((eventInfo2.customName == null) ? def.methodNfo.Name : eventInfo2.customName);
					codeMemberMethod.Attributes = (MemberAttributes)24578;
					codeMemberMethod.Comments.Add(new CodeCommentStatement("Event name: " + eventInfo2.evi.ev.screenName));
					CodeExpression[] array = null;
					if (eventInfo2.evi.ev.ident == "Custom")
					{
						array = new CodeExpression[1]
						{
							new CodeVariableReferenceExpression("args")
						};
						codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(BloxEventArg[]), "args"));
					}
					else
					{
						array = new CodeExpression[def.pars.Length];
						for (int j = 0; j < def.pars.Length; j++)
						{
							codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(def.pars[j].type, def.pars[j].name));
							array[j] = new CodeVariableReferenceExpression(def.pars[j].name);
						}
					}
					if (codeMemberMethod.Name == "Awake")
					{
						if (BloxScriptGenerator.awakeMethod == null)
						{
							BloxScriptGenerator.awakeMethod = codeMemberMethod;
						}
						else
						{
							Debug.LogWarning("Awake() method already created!");
						}
					}
					List<EventInfo>.Enumerator enumerator2 = item.Value.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							EventInfo current2 = enumerator2.Current;
							CodeMethodInvokeExpression codeMethodInvokeExpression = new CodeMethodInvokeExpression(null, current2.methodName, array);
							if (current2.evi.HasYieldInstruction())
							{
								codeMethodInvokeExpression = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "StartCoroutine", codeMethodInvokeExpression);
							}
							codeMemberMethod.Statements.Add(codeMethodInvokeExpression);
						}
					}
					finally
					{
						((IDisposable)enumerator2).Dispose();
					}
					enumerator2 = item.Value.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							EventInfo current3 = enumerator2.Current;
							codeMemberMethod = new CodeMemberMethod();
							c.Members.Add(codeMemberMethod);
							codeMemberMethod.Name = current3.methodName;
							codeMemberMethod.Attributes = (MemberAttributes)20482;
							codeMemberMethod.Comments.Add(new CodeCommentStatement("Event name: " + current3.evi.ev.screenName));
							if (current3.evi.HasYieldInstruction())
							{
								codeMemberMethod.ReturnType = new CodeTypeReference(typeof(IEnumerator));
							}
							if (current3.evi.ev.ident == "Custom")
							{
								codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(BloxEventArg[]), "args"));
							}
							else
							{
								for (int k = 0; k < def.pars.Length; k++)
								{
									codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(def.pars[k].type, def.pars[k].name));
								}
							}
							if (!BloxScriptGenerator.ProcessEventBody(blox, current3.evi, codeMemberMethod))
							{
								return false;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2).Dispose();
					}
				}
				else
				{
					EventInfo eventInfo3 = item.Value[0];
					CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
					c.Members.Add(codeMemberMethod2);
					codeMemberMethod2.Name = ((eventInfo3.customName == null) ? def.methodNfo.Name : eventInfo3.customName);
					codeMemberMethod2.Attributes = (MemberAttributes)24578;
					codeMemberMethod2.Comments.Add(new CodeCommentStatement("Event name: " + eventInfo3.evi.ev.screenName));
					if (eventInfo3.evi.HasYieldInstruction())
					{
						codeMemberMethod2.ReturnType = new CodeTypeReference(typeof(IEnumerator));
					}
					if (eventInfo3.evi.ev.ident == "Custom")
					{
						codeMemberMethod2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(BloxEventArg[]), "args"));
					}
					else
					{
						for (int l = 0; l < def.pars.Length; l++)
						{
							codeMemberMethod2.Parameters.Add(new CodeParameterDeclarationExpression(def.pars[l].type, def.pars[l].name));
						}
					}
					if (codeMemberMethod2.Name == "Awake")
					{
						if (BloxScriptGenerator.awakeMethod == null)
						{
							BloxScriptGenerator.awakeMethod = codeMemberMethod2;
						}
						else
						{
							Debug.LogWarning("Awake() method already created!");
						}
					}
					if (!BloxScriptGenerator.ProcessEventBody(blox, eventInfo3.evi, codeMemberMethod2))
					{
						return false;
					}
				}
			}
			if (BloxScriptGenerator.awakeMethod == null)
			{
				CodeMemberMethod codeMemberMethod3 = new CodeMemberMethod
				{
					Name = "Awake",
					Attributes = (MemberAttributes)24578
				};
				codeMemberMethod3.Comments.Add(new CodeCommentStatement("Init"));
				c.Members.Insert(0, codeMemberMethod3);
				BloxScriptGenerator.awakeMethod = codeMemberMethod3;
			}
			return true;
		}

		private static void AddInitCodeToAwakeMethod()
		{
			CodeExpression codeExpression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "bloxContainer");
			Dictionary<string, string>.Enumerator enumerator = BloxScriptGenerator.bloxVarNames.GetEnumerator();
			List<plyVar>.Enumerator enumerator2;
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.Current;
					enumerator2 = BloxScriptGenerator.processingBlox.variables.varDefs.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							plyVar current2 = enumerator2.Current;
							if (current2.name == current.Key)
							{
								CodeExpression targetObject = new CodeMethodInvokeExpression(codeExpression, "FindVariable", new CodePrimitiveExpression(BloxScriptGenerator.processingBlox.ident), new CodePrimitiveExpression(current2.name));
								CodeExpression left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), current.Value);
								CodeExpression right = new CodeCastExpression(current2.ValueHandler.GetType(), new CodePropertyReferenceExpression(targetObject, "ValueHandler"));
								BloxScriptGenerator.awakeMethod.Statements.Insert(0, new CodeAssignStatement(left, right));
								break;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			CodeExpression targetObject2 = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(GlobalVariables)), "Instance");
			enumerator = BloxScriptGenerator.globalVarNames.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current3 = enumerator.Current;
					enumerator2 = BloxEd.GlobalVarsObj.variables.varDefs.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							plyVar current4 = enumerator2.Current;
							if (current4.name == current3.Key)
							{
								CodeExpression targetObject3 = new CodeMethodInvokeExpression(targetObject2, "FindVariable", new CodePrimitiveExpression(current4.name));
								CodeExpression left2 = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), current3.Value);
								CodeExpression right2 = new CodeCastExpression(current4.ValueHandler.GetType(), new CodePropertyReferenceExpression(targetObject3, "ValueHandler"));
								BloxScriptGenerator.awakeMethod.Statements.Insert(0, new CodeAssignStatement(left2, right2));
								break;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			BloxScriptGenerator.awakeMethod.Statements.Insert(0, new CodeAssignStatement(codeExpression, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetComponent", new CodeTypeReference(typeof(BloxContainer))))));
		}

		private static void InsertGlobalVariables(CodeTypeDeclaration c)
		{
			foreach (KeyValuePair<string, string> globalVarName in BloxScriptGenerator.globalVarNames)
			{
				foreach (plyVar varDef in BloxEd.GlobalVarsObj.variables.varDefs)
				{
					if (varDef.name == globalVarName.Key)
					{
						c.Members.Insert(2, new CodeMemberField
						{
							Name = globalVarName.Value,
							Attributes = MemberAttributes.Private,
							Type = new CodeTypeReference(varDef.ValueHandler.GetType())
						});
						break;
					}
				}
			}
		}

		private static void InsertBloxVariables(CodeTypeDeclaration c)
		{
			foreach (KeyValuePair<string, string> bloxVarName in BloxScriptGenerator.bloxVarNames)
			{
				foreach (plyVar varDef in BloxScriptGenerator.processingBlox.variables.varDefs)
				{
					if (varDef.name == bloxVarName.Key)
					{
						c.Members.Insert(2, new CodeMemberField
						{
							Name = bloxVarName.Value,
							Attributes = MemberAttributes.Private,
							Type = new CodeTypeReference(varDef.ValueHandler.GetType())
						});
						break;
					}
				}
			}
		}

		private static bool ProcessEventBody(Blox blox, BloxEventEd evi, CodeMemberMethod m)
		{
			BloxScriptGenerator.addMethodTopLabel = false;
			BloxScriptGenerator.processingMethod = m;
			BloxScriptGenerator.processingEvent = evi;
			BloxScriptGenerator.eventVars = new Dictionary<string, EventVariableInfo>();
			BloxScriptGenerator.eventVarNames = new Dictionary<string, string>();
			for (BloxBlockEd bloxBlockEd = evi.firstBlock; bloxBlockEd != null; bloxBlockEd = bloxBlockEd.next)
			{
				if (bloxBlockEd.b.active)
				{
					try
					{
						if (!BloxScriptGenerator.CreateBlockCodeStatements(bloxBlockEd, m.Statements))
						{
							Debug.LogErrorFormat("Stopped. Error while processing Block [{0}:{1}:{2}]", evi.ev.blox.screenName, evi.ev.screenName, bloxBlockEd.b.ident);
							BloxScriptGenerator.processingEvent = null;
							return false;
						}
					}
					catch (Exception exception)
					{
						Debug.LogErrorFormat("Stopped. Error while processing Block [{0}:{1}:{2}]", evi.ev.blox.screenName, evi.ev.screenName, bloxBlockEd.b.ident);
						Debug.LogException(exception);
						BloxScriptGenerator.processingEvent = null;
						return false;
					}
				}
			}
			if (BloxScriptGenerator.addMethodTopLabel)
			{
				m.Statements.Insert(0, new CodeLabeledStatement(m.Name + "_top"));
			}
			foreach (EventVariableInfo value in BloxScriptGenerator.eventVars.Values)
			{
				if (!value.isCreated)
				{
					CodeExpression codeExpression = value.initExpr ?? BloxScriptGenerator.DefaultValueCodeExpression(value.type);
					if (evi.ev.ident == "Custom" && value.argIdx >= 0)
					{
						codeExpression = new CodeSnippetExpression(string.Format("args.Length > {0} ? {1}args[{2}].val : {3}", value.argIdx, (value.type == typeof(object)) ? "" : ("(" + BloxScriptGenerator.CodeSnippetFromCodeExpression(new CodeTypeReferenceExpression(value.type)) + ")"), value.argIdx, BloxScriptGenerator.CodeSnippetFromCodeExpression(codeExpression)));
					}
					m.Statements.Insert(0, new CodeVariableDeclarationStatement(value.type, value.name, codeExpression));
				}
			}
			BloxScriptGenerator.addMethodTopLabel = false;
			BloxScriptGenerator.processingEvent = null;
			BloxScriptGenerator.processingMethod = null;
			return true;
		}

		public static bool CreateBlockCodeStatements(BloxBlockEd bdi, CodeStatementCollection statements)
		{
			if (bdi == null)
			{
				return false;
			}
			if (bdi.owningBlock != null)
			{
				Debug.LogError("Block with owner can't be used as Statement.");
				return false;
			}
			BloxScriptGenerator.RecordUsedAssembliesFrom(bdi);
			BloxBlockScriptGenerator bloxBlockScriptGenerator = default(BloxBlockScriptGenerator);
			if (BloxScriptGenerator.generators.TryGetValue(bdi.b.GetType(), out bloxBlockScriptGenerator))
			{
				return bloxBlockScriptGenerator.CreateBlockCodeStatements(bdi, statements);
			}
			if (bdi.b.blockType != BloxBlockType.Value && bdi.b.mi != null)
			{
				CodeExpression codeExpression = null;
				if (bdi.b.mi.IsStatic)
				{
					codeExpression = new CodeTypeReferenceExpression(bdi.b.mi.ReflectedType);
				}
				else
				{
					codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, bdi.b.mi.ReflectedType);
					if (codeExpression == null)
					{
						Debug.LogErrorFormat("The Context is invalid in Block [{0}:{1}:{2}]", BloxScriptGenerator.processingEvent.ev.blox.screenName, BloxScriptGenerator.processingEvent.ev.screenName, bdi.b.ident);
						return false;
					}
				}
				if (bdi.b.mi.MemberType == MemberTypes.Property)
				{
					CodeExpression right = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], bdi.b.mi.ReturnType) ?? new CodePrimitiveExpression(null);
					statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(codeExpression, bdi.b.mi.ScriptedName), right));
					return true;
				}
				if (bdi.b.mi.MemberType == MemberTypes.Field)
				{
					CodeExpression right2 = BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[0], bdi.b.mi.ReturnType) ?? new CodePrimitiveExpression(null);
					statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(codeExpression, bdi.b.mi.ScriptedName), right2));
					return true;
				}
				if (bdi.b.mi.MemberType == MemberTypes.Method)
				{
					Type[] parameterTypes = bdi.b.mi.GetParameterTypes();
					CodeExpression[] array = new CodeExpression[bdi.paramBlocks.Length];
					for (int i = 0; i < bdi.paramBlocks.Length; i++)
					{
						array[i] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[i], parameterTypes[i]), parameterTypes[i], bdi.paramBlocks[i]);
						if (array[i] == null)
						{
							if (bdi.DefaultParamVals[i] == null)
							{
								array[i] = new CodePrimitiveExpression(null);
							}
							else
							{
								Type type = bdi.DefaultParamVals[i].GetType();
								array[i] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.ProcessValue(type, bdi.DefaultParamVals[i]), parameterTypes[i], type);
							}
						}
					}
					statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(codeExpression, bdi.b.mi.ScriptedName, array)));
					return true;
				}
				if (bdi.b.mi.MemberType == MemberTypes.Constructor)
				{
					Type[] parameterTypes2 = bdi.b.mi.GetParameterTypes();
					CodeExpression[] array2 = new CodeExpression[bdi.paramBlocks.Length];
					for (int j = 0; j < bdi.paramBlocks.Length; j++)
					{
						array2[j] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[j], parameterTypes2[j]), parameterTypes2[j], bdi.paramBlocks[j]);
						if (array2[j] == null)
						{
							if (bdi.DefaultParamVals[j] == null)
							{
								array2[j] = new CodePrimitiveExpression(null);
							}
							else
							{
								Type type2 = bdi.DefaultParamVals[j].GetType();
								array2[j] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.ProcessValue(bdi.DefaultParamVals[j].GetType(), bdi.DefaultParamVals[j]), parameterTypes2[j], type2);
							}
						}
					}
					statements.Add(new CodeExpressionStatement(new CodeObjectCreateExpression(bdi.b.mi.ReflectedType, array2)));
					return true;
				}
				Debug.LogError(bdi.b.mi.MemberType + " can't be used as Statement");
				return false;
			}
			Debug.LogError("Value Block or Block without MemberInfo can't be used as Statement. Is this a custom Block with a missing Script Generator?");
			return false;
		}

		public static CodeStatementCollection CreateChildBlockCodeStatementCollection(BloxBlockEd bdi)
		{
			if (bdi == null)
			{
				return null;
			}
			CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
			for (bdi = bdi.firstChild; bdi != null; bdi = bdi.next)
			{
				if (bdi.b.active && !BloxScriptGenerator.CreateBlockCodeStatements(bdi, codeStatementCollection))
				{
					return null;
				}
			}
			return codeStatementCollection;
		}

		public static CodeStatement[] CreateChildBlockCodeStatements(BloxBlockEd bdi)
		{
			CodeStatementCollection codeStatementCollection = BloxScriptGenerator.CreateChildBlockCodeStatementCollection(bdi);
			if (codeStatementCollection == null)
			{
				return null;
			}
			CodeStatement[] array = new CodeStatement[codeStatementCollection.Count];
			codeStatementCollection.CopyTo(array, 0);
			return array;
		}

		public static CodeExpression CreateBlockCodeExpression(BloxBlockEd bdi, Type expectedType = null)
		{
			if (bdi == null)
			{
				if (expectedType != null)
				{
					if (typeof(GameObject).IsAssignableFrom(expectedType))
					{
						return new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "gameObject");
					}
					if (typeof(Component).IsAssignableFrom(expectedType))
					{
						return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetComponent", new CodeTypeReference(expectedType)));
					}
				}
				return null;
			}
			if (bdi.owningBlock == null)
			{
				Debug.LogError("Block without owner can't be used as Expression.");
				return null;
			}
			BloxScriptGenerator.RecordUsedAssembliesFrom(bdi);
			BloxBlockScriptGenerator bloxBlockScriptGenerator = default(BloxBlockScriptGenerator);
			if (BloxScriptGenerator.generators.TryGetValue(bdi.b.GetType(), out bloxBlockScriptGenerator))
			{
				return BloxScriptGenerator.CheckCast(bloxBlockScriptGenerator.CreateBlockCodeExpression(bdi), expectedType, bdi);
			}
			if (bdi.b.blockType == BloxBlockType.Value && bdi.b.mi == null)
			{
				if (bdi.b.returnValue == null)
				{
					if (bdi.sgReturnType != null && bdi.sgReturnType != typeof(void))
					{
						return BloxScriptGenerator.CheckCast(new CodeDefaultValueExpression(new CodeTypeReference(bdi.sgReturnType)), expectedType, bdi);
					}
					return new CodePrimitiveExpression(null);
				}
				Type type = bdi.b.returnValue.GetType();
				return BloxScriptGenerator.CheckCast(BloxScriptGenerator.ProcessValue(type, bdi.b.returnValue), expectedType, type);
			}
			if (bdi.b.mi == null)
			{
				Debug.LogError("Block without MemberInfo can't be used as Expression.");
				return null;
			}
			CodeExpression codeExpression = null;
			if (bdi.b.mi.IsStatic)
			{
				codeExpression = new CodeTypeReferenceExpression(bdi.b.mi.ReflectedType);
			}
			else
			{
				codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, bdi.b.mi.ReflectedType);
				if (codeExpression == null)
				{
					Debug.LogErrorFormat("The Context is invalid in Block [{0}:{1}:{2}]", BloxScriptGenerator.processingEvent.ev.blox.screenName, BloxScriptGenerator.processingEvent.ev.screenName, bdi.b.ident);
					return null;
				}
			}
			if (bdi.b.mi.MemberType == MemberTypes.Property)
			{
				codeExpression = new CodePropertyReferenceExpression(codeExpression, bdi.b.mi.ScriptedName);
				if (bdi.b.mi.ReturnType != expectedType)
				{
					if (typeof(Component).IsAssignableFrom(expectedType))
					{
						bdi.sgReturnType = expectedType;
						codeExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(codeExpression, "GetComponent", new CodeTypeReference(expectedType)));
					}
					if (typeof(GameObject).IsAssignableFrom(expectedType) && typeof(Component).IsAssignableFrom(bdi.b.mi.ReturnType))
					{
						bdi.sgReturnType = typeof(GameObject);
						codeExpression = new CodePropertyReferenceExpression(codeExpression, bdi.b.mi.ScriptedName);
						return new CodePropertyReferenceExpression(codeExpression, "gameObject");
					}
				}
				return BloxScriptGenerator.CheckCast(codeExpression, expectedType, bdi);
			}
			if (bdi.b.mi.MemberType == MemberTypes.Field)
			{
				codeExpression = new CodeFieldReferenceExpression(codeExpression, bdi.b.mi.ScriptedName);
				if (bdi.b.mi.ReturnType != expectedType)
				{
					if (typeof(Component).IsAssignableFrom(expectedType))
					{
						bdi.sgReturnType = expectedType;
						codeExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(codeExpression, "GetComponent", new CodeTypeReference(expectedType)));
					}
					if (typeof(GameObject).IsAssignableFrom(expectedType) && typeof(Component).IsAssignableFrom(bdi.b.mi.ReturnType))
					{
						bdi.sgReturnType = typeof(GameObject);
						codeExpression = new CodePropertyReferenceExpression(codeExpression, bdi.b.mi.ScriptedName);
						return new CodePropertyReferenceExpression(codeExpression, "gameObject");
					}
				}
				return BloxScriptGenerator.CheckCast(codeExpression, expectedType, bdi);
			}
			if (bdi.b.mi.MemberType == MemberTypes.Method)
			{
				Type[] parameterTypes = bdi.b.mi.GetParameterTypes();
				CodeExpression[] array = new CodeExpression[bdi.paramBlocks.Length];
				for (int i = 0; i < bdi.paramBlocks.Length; i++)
				{
					array[i] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[i], parameterTypes[i]), parameterTypes[i], bdi.paramBlocks[i]);
					if (array[i] == null)
					{
						if (bdi.DefaultParamVals[i] == null)
						{
							array[i] = new CodePrimitiveExpression(null);
						}
						else
						{
							Type type2 = bdi.DefaultParamVals[i].GetType();
							array[i] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.ProcessValue(type2, bdi.DefaultParamVals[i]), parameterTypes[i], type2);
						}
					}
				}
				codeExpression = new CodeMethodInvokeExpression(codeExpression, bdi.b.mi.ScriptedName, array);
				if (bdi.b.mi.ReturnType != expectedType)
				{
					if (typeof(Component).IsAssignableFrom(expectedType))
					{
						bdi.sgReturnType = expectedType;
						codeExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(codeExpression, "GetComponent", new CodeTypeReference(expectedType)));
					}
					if (typeof(GameObject).IsAssignableFrom(expectedType) && typeof(Component).IsAssignableFrom(bdi.b.mi.ReturnType))
					{
						bdi.sgReturnType = typeof(GameObject);
						codeExpression = new CodePropertyReferenceExpression(codeExpression, bdi.b.mi.ScriptedName);
						return new CodePropertyReferenceExpression(codeExpression, "gameObject");
					}
				}
				return BloxScriptGenerator.CheckCast(codeExpression, expectedType, bdi);
			}
			if (bdi.b.mi.MemberType == MemberTypes.Constructor)
			{
				Type[] parameterTypes2 = bdi.b.mi.GetParameterTypes();
				CodeExpression[] array2 = new CodeExpression[bdi.paramBlocks.Length];
				for (int j = 0; j < bdi.paramBlocks.Length; j++)
				{
					array2[j] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.CreateBlockCodeExpression(bdi.paramBlocks[j], parameterTypes2[j]), parameterTypes2[j], bdi.paramBlocks[j]);
					if (array2[j] == null)
					{
						if (bdi.DefaultParamVals[j] == null)
						{
							array2[j] = new CodePrimitiveExpression(null);
						}
						else
						{
							Type type3 = bdi.DefaultParamVals[j].GetType();
							array2[j] = BloxScriptGenerator.CheckCast(BloxScriptGenerator.ProcessValue(bdi.DefaultParamVals[j].GetType(), bdi.DefaultParamVals[j]), parameterTypes2[j], type3);
						}
					}
				}
				codeExpression = new CodeObjectCreateExpression(bdi.b.mi.ReflectedType, array2);
				if (bdi.b.mi.ReturnType != expectedType)
				{
					if (typeof(Component).IsAssignableFrom(expectedType))
					{
						bdi.sgReturnType = expectedType;
						codeExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(codeExpression, "GetComponent", new CodeTypeReference(expectedType)));
					}
					if (typeof(GameObject).IsAssignableFrom(expectedType) && typeof(Component).IsAssignableFrom(bdi.b.mi.ReturnType))
					{
						bdi.sgReturnType = typeof(GameObject);
						codeExpression = new CodePropertyReferenceExpression(codeExpression, bdi.b.mi.ScriptedName);
						return new CodePropertyReferenceExpression(codeExpression, "gameObject");
					}
				}
				return BloxScriptGenerator.CheckCast(codeExpression, expectedType, bdi);
			}
			Debug.LogError(bdi.b.mi.MemberType + " can't be used as Expression");
			return null;
		}

		public static CodeExpression CheckCast(CodeExpression expr, Type expectedType, Type returnType)
		{
			if (expectedType != null && returnType != null && expr != null)
			{
				if (!expectedType.IsAssignableFrom(returnType))
				{
					expr = new CodeCastExpression(expectedType, expr);
				}
				return expr;
			}
			return expr;
		}

		public static CodeExpression CheckCast(CodeExpression expr, Type expectedType, BloxBlockEd bdi)
		{
			if (expectedType != null && bdi != null && bdi.sgReturnType != null && expr != null)
			{
				if (!expectedType.IsAssignableFrom(bdi.sgReturnType))
				{
					bdi.sgReturnType = expectedType;
					expr = new CodeCastExpression(expectedType, expr);
				}
				return expr;
			}
			return expr;
		}

		public static string CodeSnippetFromCodeExpression(CodeExpression expr)
		{
			try
			{
				string result = null;
				using (StringWriter stringWriter = new StringWriter())
				{
					CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
					cSharpCodeProvider.GenerateCodeFromExpression(expr, stringWriter, BloxScriptGenerator.options);
					result = stringWriter.ToString();
					cSharpCodeProvider.Dispose();
				}
				return result;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return null;
			}
		}

		public static CodeStatement CreateMemberSetExpression(BloxBlockEd bdi, CodeExpression valueExpr, Type valueType)
		{
			if (bdi != null && bdi.b.mi != null)
			{
				CodeExpression codeExpression = null;
				if (bdi.b.mi.IsStatic)
				{
					codeExpression = new CodeTypeReferenceExpression(bdi.b.mi.ReflectedType);
				}
				else
				{
					codeExpression = BloxScriptGenerator.CreateBlockCodeExpression(bdi.contextBlock, bdi.b.mi.ReflectedType);
					if (codeExpression == null)
					{
						Debug.LogErrorFormat("The Context is invalid in Block [{0}:{1}:{2}]", BloxScriptGenerator.processingEvent.ev.blox.screenName, BloxScriptGenerator.processingEvent.ev.screenName, bdi.b.ident);
						return null;
					}
				}
				if (bdi.b.mi.MemberType == MemberTypes.Field)
				{
					return new CodeAssignStatement(new CodeFieldReferenceExpression(codeExpression, bdi.b.mi.ScriptedName), BloxScriptGenerator.CheckCast(valueExpr, bdi.b.mi.ReturnType, valueType));
				}
				if (bdi.b.mi.MemberType == MemberTypes.Property)
				{
					return new CodeAssignStatement(new CodePropertyReferenceExpression(codeExpression, bdi.b.mi.ScriptedName), BloxScriptGenerator.CheckCast(valueExpr, bdi.b.mi.ReturnType, valueType));
				}
				return null;
			}
			return null;
		}

		private static CodeExpression DefaultValueCodeExpression(Type type)
		{
			if (type == null)
			{
				return new CodePrimitiveExpression(null);
			}
			if (type == typeof(string))
			{
				return new CodePrimitiveExpression("");
			}
			if (!type.IsValueType && !type.IsEnum)
			{
				return new CodePrimitiveExpression(null);
			}
			return BloxScriptGenerator.ProcessValue(type, BloxMemberInfo.GetDefaultValue(type));
		}

		private static CodeExpression ProcessValue(Type type, object value)
		{
			if (value != null && !type.IsPrimitive && type != typeof(string))
			{
				if (type.IsEnum)
				{
					Enum @enum = (Enum)value;
					return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(type), @enum.ToString());
				}
				if (type == typeof(Rect))
				{
					Rect rect = (Rect)value;
					return new CodeObjectCreateExpression(type, new CodePrimitiveExpression(rect.x), new CodePrimitiveExpression(rect.y), new CodePrimitiveExpression(rect.width), new CodePrimitiveExpression(rect.height));
				}
				if (type == typeof(Vector2))
				{
					Vector2 vector = (Vector2)value;
					return new CodeObjectCreateExpression(type, new CodePrimitiveExpression(vector.x), new CodePrimitiveExpression(vector.y));
				}
				if (type == typeof(Vector3))
				{
					Vector3 vector2 = (Vector3)value;
					return new CodeObjectCreateExpression(type, new CodePrimitiveExpression(vector2.x), new CodePrimitiveExpression(vector2.y), new CodePrimitiveExpression(vector2.z));
				}
				if (type == typeof(Vector4))
				{
					Vector4 vector3 = (Vector4)value;
					return new CodeObjectCreateExpression(type, new CodePrimitiveExpression(vector3.x), new CodePrimitiveExpression(vector3.y), new CodePrimitiveExpression(vector3.z), new CodePrimitiveExpression(vector3.w));
				}
				if (type == typeof(Quaternion))
				{
					Quaternion quaternion = (Quaternion)value;
					return new CodeObjectCreateExpression(type, new CodePrimitiveExpression(quaternion.x), new CodePrimitiveExpression(quaternion.y), new CodePrimitiveExpression(quaternion.z), new CodePrimitiveExpression(quaternion.w));
				}
				if (type == typeof(Color))
				{
					Color color = (Color)value;
					return new CodeObjectCreateExpression(type, new CodePrimitiveExpression(color.r), new CodePrimitiveExpression(color.g), new CodePrimitiveExpression(color.b), new CodePrimitiveExpression(color.a));
				}
				if (type == typeof(Color32))
				{
					Color32 color2 = (Color32)value;
					return new CodeObjectCreateExpression(type, new CodePrimitiveExpression(color2.r), new CodePrimitiveExpression(color2.g), new CodePrimitiveExpression(color2.b), new CodePrimitiveExpression(color2.a));
				}
				if (type.IsArray)
				{
					Type elementType = type.GetElementType();
					Array array = (Array)value;
					CodeExpression[] array2 = new CodeExpression[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = BloxScriptGenerator.ProcessValue(elementType, array.GetValue(i));
					}
					return new CodeArrayCreateExpression(elementType, array2);
				}
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
				{
					Type type2 = type.GetGenericArguments()[0];
					string text = BloxScriptGenerator.CodeSnippetFromCodeExpression(new CodeObjectCreateExpression(new CodeTypeReference(type.FullName)));
					if (text == null)
					{
						return null;
					}
					IList list = value as IList;
					if (list.Count > 0)
					{
						text += "{";
						for (int j = 0; j < list.Count; j++)
						{
							string str = null;
							try
							{
								CodeExpression expression = BloxScriptGenerator.ProcessValue(type2, list[j]);
								using (StringWriter stringWriter = new StringWriter())
								{
									CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
									cSharpCodeProvider.GenerateCodeFromExpression(expression, stringWriter, BloxScriptGenerator.options);
									str = stringWriter.ToString();
									cSharpCodeProvider.Dispose();
								}
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
								return null;
							}
							if (j != 0)
							{
								text += ", ";
							}
							text += str;
						}
						text += "}";
					}
					return new CodeSnippetExpression(text);
				}
				Debug.LogWarningFormat("Not supported value type [{0}]. Using default value expression.", type);
				return new CodeDefaultValueExpression(new CodeTypeReference(type));
			}
			return new CodePrimitiveExpression(value);
		}

		private static string CleanMemberName(string name)
		{
			return BloxScriptGenerator.nameRegex.Replace(name, "");
		}

		public static string GetCleanEventVariableName(string dirtyName, bool forceNewUnique = false)
		{
			string text = null;
			if (!forceNewUnique && BloxScriptGenerator.eventVarNames.TryGetValue(dirtyName, out text))
			{
				return text;
			}
			if (BloxScriptGenerator.processingEvent.ev.ident == "Custom" && dirtyName.StartsWith("param") && dirtyName.Length > 5 && plyEdUtil.IsValidVariableName(dirtyName))
			{
				int num = -1;
				if (int.TryParse(dirtyName.Substring(5), out num))
				{
					BloxScriptGenerator.eventVarNames.Add(dirtyName, dirtyName);
					return dirtyName;
				}
			}
			string text2 = BloxScriptGenerator.CleanMemberName(dirtyName);
			string str = text2;
			Guid guid = Guid.NewGuid();
			text = "e_" + str + "_" + guid.ToString("N").Substring(0, 4);
			bool flag = true;
			while (flag)
			{
				flag = false;
				foreach (string value in BloxScriptGenerator.eventVarNames.Values)
				{
					if (!(value == text))
					{
						continue;
					}
					flag = true;
					break;
				}
				if (flag)
				{
					string str2 = text2;
					guid = Guid.NewGuid();
					text = "e_" + str2 + "_" + guid.ToString("N").Substring(0, 4);
				}
			}
			BloxScriptGenerator.eventVarNames.Add(dirtyName, text);
			return text;
		}

		public static string GetCleanBloxVariableName(string dirtyName)
		{
			string text = null;
			if (BloxScriptGenerator.bloxVarNames.TryGetValue(dirtyName, out text))
			{
				return text;
			}
			string text2 = BloxScriptGenerator.CleanMemberName(dirtyName);
			string str = text2;
			Guid guid = Guid.NewGuid();
			text = "b_" + str + "_" + guid.ToString("N").Substring(0, 4);
			bool flag = true;
			while (flag)
			{
				flag = false;
				foreach (string value in BloxScriptGenerator.bloxVarNames.Values)
				{
					if (!(value == text))
					{
						continue;
					}
					flag = true;
					break;
				}
				if (flag)
				{
					string str2 = text2;
					guid = Guid.NewGuid();
					text = "b_" + str2 + "_" + guid.ToString("N").Substring(0, 4);
				}
			}
			BloxScriptGenerator.bloxVarNames.Add(dirtyName, text);
			return text;
		}

		public static string GetCleanGlobalVariableName(string dirtyName)
		{
			string text = null;
			if (BloxScriptGenerator.globalVarNames.TryGetValue(dirtyName, out text))
			{
				return text;
			}
			string text2 = BloxScriptGenerator.CleanMemberName(dirtyName);
			string str = text2;
			Guid guid = Guid.NewGuid();
			text = "g_" + str + "_" + guid.ToString("N").Substring(0, 4);
			bool flag = true;
			while (flag)
			{
				flag = false;
				foreach (string value in BloxScriptGenerator.globalVarNames.Values)
				{
					if (!(value == text))
					{
						continue;
					}
					flag = true;
					break;
				}
				if (flag)
				{
					string str2 = text2;
					guid = Guid.NewGuid();
					text = "g_" + str2 + "_" + guid.ToString("N").Substring(0, 4);
				}
			}
			BloxScriptGenerator.globalVarNames.Add(dirtyName, text);
			return text;
		}

		public static bool AddEventVariable(string name, Type type, CodeExpression initExpr, int argIdx, bool isCreated)
		{
			if (BloxScriptGenerator.eventVars.ContainsKey(name))
			{
				return true;
			}
			BloxScriptGenerator.eventVars.Add(name, new EventVariableInfo(name, type, initExpr, argIdx, isCreated));
			return false;
		}

		public static Type GetEventVariableType(string name)
		{
			if (BloxScriptGenerator.eventVars.ContainsKey(name))
			{
				return BloxScriptGenerator.eventVars[name].type;
			}
			return null;
		}

		public static CodeGotoStatement CreateGotoTopStatement()
		{
			BloxScriptGenerator.addMethodTopLabel = true;
			return new CodeGotoStatement(BloxScriptGenerator.processingMethod.Name + "_top");
		}

		public static void RecordUsedAssembliesFrom(BloxBlockEd bdi)
		{
			if (bdi != null)
			{
				if (bdi.b.mi != null && bdi.b.mi.ReflectedType != null)
				{
					string location = bdi.b.mi.ReflectedType.Assembly.Location;
					if (!BloxScriptGenerator.usedAssemblies.Contains(location))
					{
						BloxScriptGenerator.usedAssemblies.Add(location);
					}
				}
				if (bdi.sgReturnType != null)
				{
					string location2 = bdi.sgReturnType.Assembly.Location;
					if (!BloxScriptGenerator.usedAssemblies.Contains(location2))
					{
						BloxScriptGenerator.usedAssemblies.Add(location2);
					}
				}
			}
		}
	}
}
