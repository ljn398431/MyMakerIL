using BloxEngine;
using BloxEngine.Databinding;
using plyLibEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Internal;

namespace BloxEditor.Databinding
{
	[plyCustomEd(typeof(MemberBindProvider), Name = "Member Bind", opt = 1)]
	public class MemberBindProviderEd : DataProviderEd
	{
		private class BindableData
		{
			public string visibleName;

			public BloxMemberInfo mi;

			public Type type;
		}

		private static plyEdCoroutine loader = null;

		private static Dictionary<string, BindableData> getterBindables = null;

		private static Dictionary<string, BindableData> setterBindables = null;

		private static List<Type> scanTypes = null;

		private Type currSubBindType;

		private int memberBindIdx = -1;

		private int subBindIdx = -1;

		private BindableData[] currBindables;

		private GUIContent[] currBindableLabels;

		private List<BindableData> subBindables = new List<BindableData>();

		private GUIContent[] subBindableLabels = new GUIContent[0];

		private static readonly GUIContent GC_Loading = new GUIContent("loading ...");

		private static readonly GUIContent GC_InvokeMember = new GUIContent("Invoke");

		private static readonly GUIContent GC_SetMember = new GUIContent("Set");

		private static readonly GUIContent GC_GetMember = new GUIContent("Get");

		private static readonly GUIContent GC_TargetObject = new GUIContent("in object");

		private static readonly GUIContent GC_ToVal = new GUIContent("to");

		private static readonly GUIContent GC_With = new GUIContent("with");

		private static readonly GUIContent GC_Param = new GUIContent();

		private static readonly GUIContent GC_ToValBindWin = new GUIContent("Value Getter");

		private static void OnBlockSelectionChanged()
		{
			if (MemberBindProviderEd.loader != null)
			{
				MemberBindProviderEd.loader.Stop();
				MemberBindProviderEd.loader = null;
			}
			MemberBindProviderEd.scanTypes = null;
			MemberBindProviderEd.getterBindables = null;
			MemberBindProviderEd.setterBindables = null;
			MemberBindProviderEd.loader = plyEdCoroutine.Start(MemberBindProviderEd.Loader(), true);
		}

		private void OnBlockSelectionChanged2()
		{
			this.currBindables = null;
			this.currBindableLabels = null;
			this.subBindables = new List<BindableData>();
			this.subBindableLabels = new GUIContent[0];
		}

		public override void OnCreated()
		{
			BloxSettingsWindow.onSavedBlocksSettings = (Action)Delegate.Remove(BloxSettingsWindow.onSavedBlocksSettings, new Action(MemberBindProviderEd.OnBlockSelectionChanged));
			BloxSettingsWindow.onSavedBlocksSettings = (Action)Delegate.Combine(BloxSettingsWindow.onSavedBlocksSettings, new Action(MemberBindProviderEd.OnBlockSelectionChanged));
			BloxSettingsWindow.onSavedBlocksSettings = (Action)Delegate.Remove(BloxSettingsWindow.onSavedBlocksSettings, new Action(this.OnBlockSelectionChanged2));
			BloxSettingsWindow.onSavedBlocksSettings = (Action)Delegate.Combine(BloxSettingsWindow.onSavedBlocksSettings, new Action(this.OnBlockSelectionChanged2));
			if (MemberBindProviderEd.loader == null)
			{
				MemberBindProviderEd.loader = plyEdCoroutine.Start(MemberBindProviderEd.Loader(), true);
			}
		}

		public override string Label(DataProvider target)
		{
			MemberBindProvider memberBindProvider = (MemberBindProvider)target;
			if (memberBindProvider.member == null)
			{
				return base.nfo.Name;
			}
			return BloxMemberInfo.SimpleMemberPath(memberBindProvider.member);
		}

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			MemberBindProvider memberBindProvider = (MemberBindProvider)target;
			float num = (float)(memberBindProvider.valSetterSources.Length + 1);
			if (memberBindProvider.member != null)
			{
				if (!memberBindProvider.member.IsStatic)
				{
					num = (float)(num + 1.0);
				}
				if (((memberBindProvider.member.MemberType == MemberTypes.Method) ? memberBindProvider.valSetterSources.Length : 0) != 0)
				{
					num = (float)(num + 1.0);
				}
			}
			return (float)((EditorGUIUtility.singleLineHeight + 2.0) * num);
		}

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			MemberBindProvider memberBindProvider = (MemberBindProvider)target;
			rect.height = EditorGUIUtility.singleLineHeight;
			if (MemberBindProviderEd.loader != null)
			{
				plyEdGUI.DrawSpinner(rect, MemberBindProviderEd.GC_Loading);
				if (Event.current.type == EventType.Repaint)
				{
					MemberBindProviderEd.loader.DoUpdate();
				}
			}
			else
			{
				this.InitCurrBindables(memberBindProvider, isSetter);
				EditorGUIUtility.labelWidth = 65f;
				Rect rect2 = rect;
				GUIContent label = isSetter ? MemberBindProviderEd.GC_SetMember : MemberBindProviderEd.GC_GetMember;
				if (memberBindProvider.member != null && memberBindProvider.member.MemberType == MemberTypes.Method)
				{
					label = MemberBindProviderEd.GC_InvokeMember;
				}
				EditorGUI.BeginChangeCheck();
				this.memberBindIdx = EditorGUI.Popup(rect2, label, this.memberBindIdx, this.currBindableLabels);
				if (EditorGUI.EndChangeCheck())
				{
					memberBindProvider.member = this.currBindables[this.memberBindIdx].mi;
					memberBindProvider.instanceMember = null;
					if (memberBindProvider.valSetterSources.Length != 0)
					{
						memberBindProvider.valSetterSources = new DataBindingValueSource[0];
					}
					if (memberBindProvider.sourceObjType == MemberBindProvider.DataSourceOject.Instance)
					{
						MemberBindProvider target2 = memberBindProvider;
						BloxMemberInfo member = memberBindProvider.member;
						this.InitSubBindables(target2, (member != null) ? member.ReflectedType : null);
					}
				}
				if (memberBindProvider.member == null)
				{
					if (memberBindProvider.instanceMember != null)
					{
						memberBindProvider.instanceMember = null;
						GUI.changed = true;
					}
					if (memberBindProvider.valSetterSources.Length != 0)
					{
						memberBindProvider.valSetterSources = new DataBindingValueSource[0];
						GUI.changed = true;
					}
				}
				else
				{
					if (!memberBindProvider.member.IsStatic)
					{
						rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
						if (memberBindProvider.sourceObjType != 0 && memberBindProvider.sourceObjType != MemberBindProvider.DataSourceOject.Owner)
						{
							rect2.width = (float)(EditorGUIUtility.labelWidth + 100.0);
						}
						EditorGUI.BeginChangeCheck();
						memberBindProvider.sourceObjType = (MemberBindProvider.DataSourceOject)EditorGUI.EnumPopup(rect2, MemberBindProviderEd.GC_TargetObject, (Enum)(object)memberBindProvider.sourceObjType);
						if (EditorGUI.EndChangeCheck())
						{
							memberBindProvider.sourceObjData = "";
							if (memberBindProvider.sourceObjType == MemberBindProvider.DataSourceOject.Instance)
							{
								MemberBindProvider target3 = memberBindProvider;
								BloxMemberInfo member2 = memberBindProvider.member;
								this.InitSubBindables(target3, (member2 != null) ? member2.ReflectedType : null);
							}
						}
						rect2.x += (float)(EditorGUIUtility.labelWidth + 102.0);
						rect2.width = (float)(rect.width - (EditorGUIUtility.labelWidth + 102.0));
						if (memberBindProvider.sourceObjType == MemberBindProvider.DataSourceOject.WithName || memberBindProvider.sourceObjType == MemberBindProvider.DataSourceOject.WithTag)
						{
							memberBindProvider.sourceObjData = EditorGUI.TextField(rect2, memberBindProvider.sourceObjData);
						}
						else if (memberBindProvider.sourceObjType == MemberBindProvider.DataSourceOject.OfType)
						{
							EditorGUI.LabelField(rect2, BloxEd.PrettyTypeName(memberBindProvider.member.ReflectedType, true));
						}
						else if (memberBindProvider.sourceObjType == MemberBindProvider.DataSourceOject.Instance)
						{
							EditorGUI.BeginChangeCheck();
							this.subBindIdx = EditorGUI.Popup(rect2, this.subBindIdx, this.subBindableLabels);
							if (EditorGUI.EndChangeCheck())
							{
								memberBindProvider.instanceMember = this.subBindables[this.subBindIdx].mi;
							}
						}
					}
					rect2.x = rect.x;
					rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
					rect2.width = rect.width;
					if (memberBindProvider.member.MemberType == MemberTypes.Method)
					{
						ParameterInfo[] parameters = memberBindProvider.member.GetParameters();
						if (memberBindProvider.valSetterSources.Length != parameters.Length)
						{
							GUI.changed = true;
							memberBindProvider.valSetterSources = new DataBindingValueSource[parameters.Length];
							for (int i = 0; i < parameters.Length; i++)
							{
								memberBindProvider.valSetterSources[i] = new DataBindingValueSource();
							}
						}
						if (memberBindProvider.valSetterSources.Length != 0)
						{
							GUI.Label(rect2, MemberBindProviderEd.GC_With);
							for (int j = 0; j < memberBindProvider.valSetterSources.Length; j++)
							{
								MemberBindProviderEd.GC_Param.text = parameters[j].Name + "=";
								rect2.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
								memberBindProvider.valSetterSources[j] = DataProviderEd.DataBindingValueSourceField(rect2, MemberBindProviderEd.GC_Param, memberBindProvider.valSetterSources[j], MemberBindProviderEd.GC_ToValBindWin, memberBindProvider);
							}
						}
					}
					else if (isSetter)
					{
						if (memberBindProvider.valSetterSources.Length != 1)
						{
							GUI.changed = true;
							memberBindProvider.valSetterSources = new DataBindingValueSource[1]
							{
								new DataBindingValueSource()
							};
						}
						memberBindProvider.valSetterSources[0] = DataProviderEd.DataBindingValueSourceField(rect2, MemberBindProviderEd.GC_ToVal, memberBindProvider.valSetterSources[0], MemberBindProviderEd.GC_ToValBindWin, memberBindProvider);
					}
				}
			}
		}

		private void InitCurrBindables(MemberBindProvider target, bool isForSetter)
		{
			if (this.currBindables != null && this.currBindableLabels != null)
				return;
			Dictionary<string, BindableData> dictionary = isForSetter ? MemberBindProviderEd.setterBindables : MemberBindProviderEd.getterBindables;
			this.currBindables = new BindableData[dictionary.Count];
			dictionary.Values.CopyTo(this.currBindables, 0);
			this.currBindableLabels = new GUIContent[this.currBindables.Length];
			for (int i = 0; i < this.currBindables.Length; i++)
			{
				this.currBindableLabels[i] = new GUIContent(this.currBindables[i].visibleName);
			}
			this.memberBindIdx = -1;
			BloxMemberInfo member = target.member;
			string memberName = MemberBindProviderEd.GetMemberName((member != null) ? member.MI : null);
			if (!string.IsNullOrEmpty(memberName))
			{
				int num = 0;
				while (num < this.currBindables.Length)
				{
					if (!(memberName == this.currBindables[num].visibleName))
					{
						num++;
						continue;
					}
					this.memberBindIdx = num;
					break;
				}
			}
			if (target.sourceObjType == MemberBindProvider.DataSourceOject.Instance && target.member != null)
			{
				this.InitSubBindables(target, target.member.ReflectedType);
			}
		}

		private void InitSubBindables(MemberBindProvider target, Type rt)
		{
			if (this.currSubBindType != rt)
			{
				this.currSubBindType = rt;
				this.subBindIdx = -1;
				this.subBindables.Clear();
				this.subBindableLabels = new GUIContent[0];
				if (rt != null)
				{
					foreach (BindableData value in MemberBindProviderEd.getterBindables.Values)
					{
						if (value.mi.IsStatic && rt.IsAssignableFrom(value.mi.ReturnType))
						{
							this.subBindables.Add(new BindableData
							{
								visibleName = value.mi.ReflectedType.FullName.Replace(".", "/") + "/" + value.mi.Name,
								mi = value.mi,
								type = null
							});
						}
					}
					string a = BloxMemberInfo.FullMemberPath(target.instanceMember).Replace(".", "/");
					this.subBindableLabels = new GUIContent[this.subBindables.Count];
					for (int i = 0; i < this.subBindables.Count; i++)
					{
						this.subBindableLabels[i] = new GUIContent(this.subBindables[i].visibleName);
						if (a == this.subBindables[i].visibleName)
						{
							this.subBindIdx = i;
						}
					}
				}
			}
		}

		private static IEnumerator Loader()
		{
			int count = 0;
			int countBeforeYield = 20;
			if (MemberBindProviderEd.getterBindables == null || MemberBindProviderEd.setterBindables == null)
			{
				MemberBindProviderEd.getterBindables = new Dictionary<string, BindableData>();
				MemberBindProviderEd.setterBindables = new Dictionary<string, BindableData>();
				if (MemberBindProviderEd.scanTypes == null)
				{
					MemberBindProviderEd.scanTypes = new List<Type>();
					List<Type> usedValueTypes = new List<Type>(20)
					{
						typeof(bool),
						typeof(int),
						typeof(float),
						typeof(string),
						typeof(Vector2),
						typeof(Vector3),
						typeof(Rect),
						typeof(Color)
					};
					new List<Type>();
					List<string> lines = plyEdUtil.ReadCompressedLines(plyEdUtil.ProjectFullPath + BloxEdGlobal.MiscPath + "blocks.bin");
					if (lines.Count > 0)
					{
						yield return (object)null;
						for (int i = 0; i < lines.Count; i++)
						{
							string text = lines[i];
							if (!string.IsNullOrEmpty(text))
							{
								BloxMemberInfo bloxMemberInfo = BloxMemberInfo.DecodeMember(text);
								if (bloxMemberInfo != null && !MemberBindProviderEd.scanTypes.Contains(bloxMemberInfo.ReflectedType))
								{
									MemberBindProviderEd.scanTypes.Add(bloxMemberInfo.ReflectedType);
								}
								count++;
								if (count >= countBeforeYield)
								{
									count = 0;
									yield return (object)null;
								}
							}
						}
					}
					foreach (Type item in usedValueTypes)
					{
						if (!MemberBindProviderEd.scanTypes.Contains(item))
						{
							MemberBindProviderEd.scanTypes.Add(item);
						}
					}
					yield return (object)null;
					MemberBindProviderEd.scanTypes.Sort((Type a, Type b) => a.FullName.CompareTo(b.FullName));
				}
				List<Type>.Enumerator enumerator2 = MemberBindProviderEd.scanTypes.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						Type current2 = enumerator2.Current;
						if (current2.IsClass && !current2.IsEnum && (!current2.IsAbstract || current2.IsSealed) && !current2.IsSpecialName && !current2.IsSubclassOf(typeof(Attribute)) && current2 != typeof(Attribute) && !current2.IsSubclassOf(typeof(Delegate)) && current2 != typeof(Delegate) && !current2.IsSubclassOf(typeof(Exception)) && current2 != typeof(Exception) && !current2.IsSubclassOf(typeof(BloxEventHandler)) && current2 != typeof(BloxEventHandler))
						{
							bool inclOnlySpecifiedMembers = false;
							object[] customAttributes = current2.GetCustomAttributes(typeof(ExcludeFromBloxAttribute), true);
							if (customAttributes.Length != 0)
							{
								if (!((ExcludeFromBloxAttribute)customAttributes[0]).ExceptForSpecifiedMembers)
								{
									continue;
								}
								inclOnlySpecifiedMembers = true;
							}
							bool flag = true;
							BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
							if (BloxEd.Instance.includeDeclaredOnly | flag)
							{
								bindingFlags = (BindingFlags)((int)bindingFlags | 2);
							}
							PropertyInfo[] properties = current2.GetProperties(bindingFlags);
							for (int j = 0; j < properties.Length; j++)
							{
								PropertyInfo propertyInfo = properties[j];
								MemberBindProviderEd.ProcessBindableMember(propertyInfo, propertyInfo.PropertyType, inclOnlySpecifiedMembers);
							}
							FieldInfo[] fields = current2.GetFields(bindingFlags);
							for (int j = 0; j < fields.Length; j++)
							{
								FieldInfo fieldInfo = fields[j];
								MemberBindProviderEd.ProcessBindableMember(fieldInfo, fieldInfo.FieldType, inclOnlySpecifiedMembers);
							}
							MethodInfo[] methods = current2.GetMethods(bindingFlags);
							for (int j = 0; j < methods.Length; j++)
							{
								MethodInfo methodInfo = methods[j];
								if (!methodInfo.IsSpecialName)
								{
									MemberBindProviderEd.ProcessBindableMember(methodInfo, methodInfo.ReturnType, inclOnlySpecifiedMembers);
								}
							}
							count++;
							if (count >= countBeforeYield)
							{
								count = 0;
								yield return (object)null;
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2).Dispose();
				}
				enumerator2 = default(List<Type>.Enumerator);
			}
			yield return (object)null;
			MemberBindProviderEd.loader.Stop();
			MemberBindProviderEd.loader = null;
		}

		private static void ProcessBindableMember(MemberInfo mi, Type valType, bool inclOnlySpecifiedMembers)
		{
			bool flag = inclOnlySpecifiedMembers;
			object[] customAttributes = mi.GetCustomAttributes(true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Type type = customAttributes[i].GetType();
				if (type == typeof(IncludeInBloxAttribute))
				{
					flag = false;
				}
				if (type == typeof(ExcludeFromBloxAttribute))
				{
					flag = true;
					break;
				}
				if (type == typeof(BloxEventAttribute))
				{
					flag = true;
					break;
				}
				if (type == typeof(ObsoleteAttribute))
				{
					flag = true;
					break;
				}
				if (type == typeof(ExcludeFromDocsAttribute))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Type _ = mi.ReflectedType;
				string memberName = MemberBindProviderEd.GetMemberName(mi);
				BloxMemberInfo bloxMemberInfo = new BloxMemberInfo(mi, null);
				if (bloxMemberInfo.CanGetValue && valType != null && valType != typeof(void) && !MemberBindProviderEd.getterBindables.ContainsKey(memberName))
				{
					MemberBindProviderEd.getterBindables.Add(memberName, new BindableData
					{
						visibleName = memberName,
						mi = bloxMemberInfo,
						type = valType
					});
				}
				if (!bloxMemberInfo.CanSetValue && bloxMemberInfo.MemberType != MemberTypes.Method)
					return;
				if (!MemberBindProviderEd.setterBindables.ContainsKey(memberName))
				{
					MemberBindProviderEd.setterBindables.Add(memberName, new BindableData
					{
						visibleName = memberName,
						mi = bloxMemberInfo,
						type = ((bloxMemberInfo.MemberType == MemberTypes.Method) ? typeof(object) : valType)
					});
				}
			}
		}

		private static string GetMemberName(MemberInfo mi)
		{
			if (mi == null)
			{
				return "";
			}
			ParameterInfo[] array = null;
			string text = mi.Name;
			if (mi.MemberType == MemberTypes.Method)
			{
				MethodInfo methodInfo = (MethodInfo)mi;
				array = methodInfo.GetParameters();
				text += BloxEd.ParametersNameSection(array);
				if (methodInfo.ReturnType != null && methodInfo.ReturnType != typeof(void))
				{
					text = text + ": " + BloxEd.PrettyTypeName(methodInfo.ReturnType, true);
				}
			}
			else if (mi.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)mi;
				text = text + ": " + BloxEd.PrettyTypeName(fieldInfo.FieldType, true);
			}
			else if (mi.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)mi;
				text = text + ": " + BloxEd.PrettyTypeName(propertyInfo.PropertyType, true);
			}
			text = mi.ReflectedType.FullName.Replace('.', '/') + "/" + text;
			text = text.Replace('+', '.');
			if (string.IsNullOrEmpty(mi.ReflectedType.Namespace))
			{
				text = "Scripts/" + text;
			}
			return text;
		}
	}
}
