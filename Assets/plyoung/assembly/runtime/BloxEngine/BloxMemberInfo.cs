using BloxEngine.Databinding;
using System;
using System.Reflection;
using UnityEngine;

namespace BloxEngine
{
	[ExcludeFromBlox]
	public class BloxMemberInfo
	{
		private MemberInfo member;

		private ConstructorInfo ci;

		private MethodInfo mi;

		private PropertyInfo pi;

		private FieldInfo fi;

		private object prevContext;

		private Property pT;

		public static readonly char MEMBER_ENCODE_SEPERATOR = '\u001f';

		public string Name
		{
			get;
			private set;
		}

		public string ScriptedName
		{
			get;
			private set;
		}

		public MemberTypes MemberType
		{
			get;
			private set;
		}

		public Type ReflectedType
		{
			get;
			private set;
		}

		public Type DeclaringType
		{
			get;
			private set;
		}

		public Type ReturnType
		{
			get;
			private set;
		}

		public bool IsStatic
		{
			get;
			private set;
		}

		public bool CanSetValue
		{
			get;
			private set;
		}

		public bool CanGetValue
		{
			get;
			private set;
		}

		public bool IsFieldOrProperty
		{
			get;
			private set;
		}

		public bool isPropertyT
		{
			get;
			private set;
		}

		public MemberInfo MI
		{
			get
			{
				return this.member;
			}
		}

		public bool IsValid
		{
			get
			{
				if (this.member != null && this.MemberType != 0 && this.ReflectedType != null)
				{
					return this.DeclaringType != null;
				}
				return false;
			}
		}

		public bool IsGenericMethod
		{
			get
			{
				if (this.ci != null)
				{
					return this.ci.IsGenericMethod;
				}
				if (this.mi != null)
				{
					return this.mi.IsGenericMethod;
				}
				return false;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				if (this.ci != null)
				{
					return this.ci.IsSpecialName;
				}
				if (this.mi != null)
				{
					return this.mi.IsSpecialName;
				}
				if (this.pi != null)
				{
					return this.pi.IsSpecialName;
				}
				if (this.fi != null)
				{
					return this.fi.IsSpecialName;
				}
				return false;
			}
		}

		public bool HasIndexParameters
		{
			get
			{
				if (this.pi != null)
				{
					return this.pi.GetIndexParameters().Length != 0;
				}
				return false;
			}
		}

		public BloxMemberInfo(MemberInfo member, Type reflectedType = null)
		{
			if (member == null)
			{
				this.member = null;
			}
			else
			{
				this.member = member;
				this.MemberType = member.MemberType;
				this.ReflectedType = (reflectedType ?? member.ReflectedType);
				this.DeclaringType = member.DeclaringType;
				this.Name = member.Name;
				this.ScriptedName = member.Name;
				this.CanSetValue = false;
				this.CanGetValue = false;
				this.IsFieldOrProperty = false;
				this.isPropertyT = false;
				if (member.MemberType == MemberTypes.Property)
				{
					this.pi = (PropertyInfo)member;
					this.ReturnType = this.pi.PropertyType;
					MethodInfo getMethod = this.pi.GetGetMethod();
					MethodInfo setMethod = this.pi.GetSetMethod();
					this.IsStatic = ((getMethod != null && getMethod.IsStatic) || (setMethod != null && setMethod.IsStatic));
					this.CanSetValue = (setMethod != null);
					this.CanGetValue = (getMethod != null);
					this.IsFieldOrProperty = true;
					this.isPropertyT = typeof(Property).IsAssignableFrom(this.ReturnType);
					if (this.isPropertyT)
					{
						this.ReturnType = this.ReturnType.GetGenericArguments()[0];
						this.ScriptedName += ".Value";
					}
				}
				else if (member.MemberType == MemberTypes.Field)
				{
					this.fi = (FieldInfo)member;
					this.ReturnType = this.fi.FieldType;
					this.IsStatic = this.fi.IsStatic;
					this.CanSetValue = (!this.fi.IsInitOnly && !this.fi.IsLiteral);
					this.CanGetValue = true;
					this.IsFieldOrProperty = true;
					this.isPropertyT = typeof(Property).IsAssignableFrom(this.ReturnType);
					if (this.isPropertyT)
					{
						this.ReturnType = this.ReturnType.GetGenericArguments()[0];
						this.ScriptedName += ".Value";
					}
				}
				else if (member.MemberType == MemberTypes.Constructor)
				{
					this.ci = (ConstructorInfo)member;
					this.ReturnType = this.DeclaringType;
					this.IsStatic = true;
					this.CanGetValue = true;
				}
				else if (member.MemberType == MemberTypes.Method)
				{
					this.mi = (MethodInfo)member;
					this.ReturnType = this.mi.ReturnType;
					this.IsStatic = this.mi.IsStatic;
					this.CanGetValue = (this.ReturnType != null && this.ReturnType != typeof(void));
				}
			}
		}

		public ParameterInfo[] GetParameters()
		{
			if (this.mi != null)
			{
				return this.mi.GetParameters();
			}
			if (this.ci != null)
			{
				return this.ci.GetParameters();
			}
			return null;
		}

		public Type[] GetParameterTypes()
		{
			ParameterInfo[] parameters = this.GetParameters();
			if (((parameters != null) ? parameters.Length : 0) != 0)
			{
				Type[] array = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i] = parameters[i].ParameterType;
				}
				return array;
			}
			return null;
		}

		public object[] GetCustomAttributes(bool inherit)
		{
			if (this.member == null)
			{
				return new object[0];
			}
			return this.member.GetCustomAttributes(inherit);
		}

		public object Invoke(object context, object[] args)
		{
			try
			{
				if (this.mi != null)
				{
					return this.mi.Invoke(context, args);
				}
				if (this.ci != null)
				{
					return this.ci.Invoke(args);
				}
			}
			catch (ArgumentException)
			{
				bool flag = true;
				ParameterInfo[] parameters = this.GetParameters();
				object[] array = new object[parameters.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ((i >= args.Length) ? null : args[i]);
					if (array[i] == null)
					{
						if (!parameters[i].ParameterType.IsAssignableFrom(typeof(Nullable)))
						{
							flag = false;
							break;
						}
					}
					else if (!BloxUtil.TryConvert(array[i], parameters[i].ParameterType, out array[i]))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					if (this.mi != null)
					{
						return this.mi.Invoke(context, array);
					}
					if (this.ci == null)
						goto end_IL_003a;
					return this.ci.Invoke(array);
				}
				throw new Exception("One or more parameters/field values could not be converted to the correct type(s) expected.");
				end_IL_003a:;
			}
			return null;
		}

		public object GetValue(object context)
		{
			try
			{
				if (!this.IsStatic && context == null)
				{
					return null;
				}
				if (this.isPropertyT)
				{
					if (this.prevContext == context && this.pT != null)
					{
						return this.pT.Value;
					}
					this.prevContext = context;
					if (this.fi != null)
					{
						this.pT = (this.fi.GetValue(context) as Property);
					}
					else if (this.pi != null)
					{
						this.pT = (this.pi.GetValue(context, null) as Property);
					}
					Property obj = this.pT;
					return (obj != null) ? obj.Value : null;
				}
				if (this.fi != null)
				{
					return this.fi.GetValue(context);
				}
				if (this.pi != null)
				{
					return this.pi.GetValue(context, null);
				}
				if (this.mi != null)
				{
					return this.Invoke(context, null);
				}
			}
			catch (TargetException)
			{
				string[] obj2 = new string[5]
				{
					"The Context is invalid. Expected [",
					null,
					null,
					null,
					null
				};
				MemberInfo obj3 = this.member;
				obj2[1] = ((obj3 != null) ? obj3.ReflectedType.Name : null);
				obj2[2] = "] but context was set to [";
				obj2[3] = ((context != null) ? context.GetType().Name : null);
				obj2[4] = "]";
				throw new Exception(string.Concat(obj2));
			}
			return null;
		}

		public void SetValue(object context, object value)
		{
			try
			{
				if (this.IsStatic || context != null)
				{
					if (this.isPropertyT)
					{
						if (this.prevContext != context || this.pT == null)
						{
							this.prevContext = context;
							if (this.fi != null)
							{
								this.pT = (this.fi.GetValue(context) as Property);
							}
							else if (this.pi != null)
							{
								this.pT = (this.pi.GetValue(context, null) as Property);
							}
							if (this.pT != null)
							{
								this.pT.Value = value;
							}
						}
						else
						{
							this.pT.Value = value;
						}
					}
					else if (this.fi != null)
					{
						this.fi.SetValue(context, value);
					}
					else if (this.pi != null)
					{
						this.pi.SetValue(context, value, null);
					}
				}
			}
			catch (ArgumentException)
			{
				throw new Exception("One or more parameters/field values could not be converted to the correct type(s) expected by this Block.");
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public Property GetPropertyT(object context)
		{
			if (!this.isPropertyT)
			{
				return null;
			}
			if (!this.IsStatic && context == null)
			{
				return null;
			}
			if (this.prevContext != context || this.pT == null)
			{
				this.prevContext = context;
				if (this.fi != null)
				{
					this.pT = (this.fi.GetValue(context) as Property);
				}
				else if (this.pi != null)
				{
					this.pT = (this.pi.GetValue(context, null) as Property);
				}
			}
			return this.pT;
		}

		public static BloxMemberInfo DecodeMember(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				return null;
			}
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
			BloxMemberInfo result = null;
			string[] array = data.Split(BloxMemberInfo.MEMBER_ENCODE_SEPERATOR);
			if (array.Length >= 3)
			{
				Type type = Type.GetType(array[1]);
				if (type == null)
				{
					return null;
				}
				if (array[0] == "F")
				{
					MemberInfo[] array2 = type.FindMembers(MemberTypes.Field, bindingAttr, BloxMemberInfo.FilterMembersByName, array[2]);
					if (array2.Length == 0)
					{
						Debug.LogError("Could not find field [" + array[2] + "] in " + type);
						return null;
					}
					result = new BloxMemberInfo(array2[0], type);
				}
				else if (array[0] == "P")
				{
					MemberInfo[] array3 = type.FindMembers(MemberTypes.Property, bindingAttr, BloxMemberInfo.FilterMembersByName, array[2]);
					if (array3.Length == 0)
					{
						Debug.LogError("Could not find property [" + array[2] + "] in " + type);
						return null;
					}
					result = new BloxMemberInfo(array3[0], type);
				}
				else if (array[0] == "C")
				{
					if (string.IsNullOrEmpty(array[2]))
					{
						return new BloxMemberInfo(type, type);
					}
					ConstructorInfo[] array4 = (ConstructorInfo[])type.FindMembers(MemberTypes.Constructor, bindingAttr, BloxMemberInfo.FilterMembersByName, array[2]);
					if (array4.Length == 0)
					{
						Debug.LogError("Could not find constructor [" + array[2] + "] in " + type);
						return null;
					}
					if (array4.Length == 1)
					{
						result = new BloxMemberInfo(array4[0], type);
					}
					else
					{
						Type[] array5 = new Type[0];
						if (array.Length > 3)
						{
							array5 = new Type[array.Length - 3];
							for (int i = 0; i < array.Length - 3; i++)
							{
								array5[i] = Type.GetType(array[i + 3]);
								if (array5[i] == null)
								{
									Debug.LogError("Could not get parameter type [" + array[i + 3] + "] for " + array[1] + "." + array[2]);
									return null;
								}
							}
						}
						for (int j = 0; j < array4.Length; j++)
						{
							ParameterInfo[] parameters = array4[j].GetParameters();
							if (array5.Length == parameters.Length)
							{
								if (parameters.Length == 0)
								{
									result = new BloxMemberInfo(array4[j], type);
									break;
								}
								bool flag = true;
								int num = 0;
								while (num < parameters.Length)
								{
									if (array5[num] == parameters[num].ParameterType)
									{
										num++;
										continue;
									}
									flag = false;
									break;
								}
								if (flag)
								{
									result = new BloxMemberInfo(array4[j], type);
									break;
								}
							}
						}
					}
				}
				else if (array[0] == "M")
				{
					MethodInfo[] array6 = (MethodInfo[])type.FindMembers(MemberTypes.Method, bindingAttr, BloxMemberInfo.FilterMembersByName, array[2]);
					if (array6.Length == 0)
					{
						Debug.LogError("Could not find method [" + array[2] + "] in " + type);
						return null;
					}
					if (array6.Length == 1)
					{
						result = new BloxMemberInfo(array6[0], type);
					}
					else
					{
						Type[] array7 = new Type[0];
						if (array.Length > 3)
						{
							array7 = new Type[array.Length - 3];
							for (int k = 0; k < array.Length - 3; k++)
							{
								array7[k] = Type.GetType(array[k + 3]);
								if (array7[k] == null)
								{
									Debug.Log(data);
									Debug.LogError("Could not get parameter type [" + array[k + 3] + "] for " + array[1] + " ." + array[2]);
									return null;
								}
							}
						}
						for (int l = 0; l < array6.Length; l++)
						{
							ParameterInfo[] parameters2 = array6[l].GetParameters();
							if (array7.Length == parameters2.Length)
							{
								if (parameters2.Length == 0)
								{
									result = new BloxMemberInfo(array6[l], type);
									break;
								}
								bool flag2 = true;
								int num2 = 0;
								while (num2 < parameters2.Length)
								{
									if (array7[num2] == parameters2[num2].ParameterType)
									{
										num2++;
										continue;
									}
									flag2 = false;
									break;
								}
								if (flag2)
								{
									result = new BloxMemberInfo(array6[l], type);
									break;
								}
							}
						}
					}
				}
				else
				{
					Debug.LogError("[BloxUtil.Create] Unexpected flag: " + array[0]);
				}
			}
			else
			{
				Debug.LogError("[BloxUtil.Create] Invalid data provided");
			}
			return result;
		}

		public static string EncodeMember(BloxMemberInfo member)
		{
			if (member == null)
			{
				return "";
			}
			string text = "";
			if (member.MemberType == MemberTypes.Field)
			{
				text = "F";
			}
			else if (member.MemberType == MemberTypes.Property)
			{
				text = "P";
			}
			else if (member.MemberType == MemberTypes.Constructor)
			{
				text = "C";
			}
			else if (member.MemberType == MemberTypes.Method)
			{
				text = "M";
			}
			string[] obj = new string[5]
			{
				text,
				null,
				null,
				null,
				null
			};
			char mEMBER_ENCODE_SEPERATOR = BloxMemberInfo.MEMBER_ENCODE_SEPERATOR;
			obj[1] = mEMBER_ENCODE_SEPERATOR.ToString();
			obj[2] = member.ReflectedType.AssemblyQualifiedName;
			mEMBER_ENCODE_SEPERATOR = BloxMemberInfo.MEMBER_ENCODE_SEPERATOR;
			obj[3] = mEMBER_ENCODE_SEPERATOR.ToString();
			obj[4] = member.Name;
			text = string.Concat(obj);
			ParameterInfo[] parameters = member.GetParameters();
			if (((parameters != null) ? parameters.Length : 0) != 0)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					string str = text;
					mEMBER_ENCODE_SEPERATOR = BloxMemberInfo.MEMBER_ENCODE_SEPERATOR;
					text = str + mEMBER_ENCODE_SEPERATOR.ToString() + parameters[i].ParameterType.AssemblyQualifiedName;
				}
			}
			return text;
		}

		/// <summary> returns "TypeName.MemberName" </summary>
		public static string SimpleMemberPath(BloxMemberInfo member)
		{
			if (member == null)
			{
				return "";
			}
			return member.ReflectedType.Name + "." + member.Name;
		}

		/// <summary> returns "TypeFullName.MemberName" </summary>
		public static string FullMemberPath(BloxMemberInfo member)
		{
			if (member == null)
			{
				return "";
			}
			return member.ReflectedType.FullName + "." + member.Name;
		}

		private static bool FilterMembersByName(MemberInfo mi, object obj)
		{
			return mi.Name == obj.ToString();
		}

		public static object GetDefaultValue(Type t)
		{
			if (t != null && t != typeof(void))
			{
				if (!(t.IsValueType | t.IsEnum))
				{
					if (t != typeof(string))
					{
						return null;
					}
					return string.Empty;
				}
				return Activator.CreateInstance(t);
			}
			return null;
		}
	}
}
