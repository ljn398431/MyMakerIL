using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BloxEngine
{
	[ExcludeFromBlox]
	public static class BloxUtil
	{
		private static List<Assembly> _priorityAsms;

		private static List<Assembly> _otherAsms;

		private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

		public static T GetComponent<T>(object obj) where T : Component
		{
			if (obj == null)
			{
				return null;
			}
			T val = (T)(obj as T);
			if ((UnityEngine.Object)(object)val != (UnityEngine.Object)null)
			{
				return val;
			}
			GameObject gameObject = obj as GameObject;
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				return gameObject.GetComponent<T>();
			}
			Component component = obj as Component;
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				return component.GetComponent<T>();
			}
			return null;
		}

		public static bool TryConvert(object value, Type targetType, out object result)
		{
			if (value != null && targetType != null)
			{
				try
				{
					result = Convert.ChangeType(value, targetType);
					return true;
				}
				catch (Exception)
				{
					result = null;
					return false;
				}
			}
			result = null;
			return false;
		}

		public static bool TryConvert<T>(object value, out T result)
		{
			if (value == null)
			{
				result = default(T);
				return false;
			}
			try
			{
				result = (T)Convert.ChangeType(value, typeof(T));
				return true;
			}
			catch (Exception)
			{
				result = default(T);
				return false;
			}
		}

		public static bool IsEqual(object a, object b, bool tryConverted)
		{
			if (a == null && b == null)
			{
				return true;
			}
			if (a != null && b != null)
			{
				if (object.Equals(a, b))
				{
					return true;
				}
				UnityEngine.Object x = a as UnityEngine.Object;
				UnityEngine.Object @object = b as UnityEngine.Object;
				if ((x != (UnityEngine.Object)null || @object != (UnityEngine.Object)null) && x == @object)
				{
					return true;
				}
				if (tryConverted)
				{
					object objB = null;
					BloxUtil.TryConvert(b, a.GetType(), out objB);
					return object.Equals(a, objB);
				}
				return false;
			}
			return false;
		}

		public static int Compare(object a, object b)
		{
			if (a != null && b != null)
			{
				object y = Convert.ChangeType(b, a.GetType());
				return Comparer<object>.Default.Compare(a, y);
			}
			throw new Exception("One or both values are null and can't be compared in this manner.");
		}

		public static Type GetType(string typeName, string assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName).GetType(typeName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static float AsFloat(object o)
		{
			float result = 0f;
			try
			{
				result = (float)o;
				return result;
			}
			catch (Exception ex)
			{
				if (o != null && o.GetType() == typeof(int))
				{
					result = (float)(int)o;
					return result;
				}
				if (!BloxUtil.TryConvert<float>(o, out result))
				{
					Debug.LogError(ex.Message);
					return result;
				}
				return result;
			}
		}

		public static Array ArrayAdd(Array arr, object value)
		{
			if (arr == null)
			{
				return null;
			}
			Type elementType = arr.GetType().GetElementType();
			Array array = Array.CreateInstance(elementType, arr.Length + 1);
			arr.CopyTo(array, 0);
			try
			{
				array.SetValue(value, array.Length - 1);
				return array;
			}
			catch (Exception)
			{
				if (BloxUtil.TryConvert(value, elementType, out value))
				{
					array.SetValue(value, array.Length - 1);
					return array;
				}
				throw new Exception("Expected value type was [" + elementType + "] but the value was of type [" + ((value != null) ? value.GetType() : null) + "]");
			}
		}

		public static Array ArrayInsert(Array arr, object value, int index)
		{
			if (arr == null)
			{
				return null;
			}
			Type elementType = arr.GetType().GetElementType();
			Array array = Array.CreateInstance(elementType, arr.Length + 1);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (i == index)
				{
					try
					{
						array.SetValue(value, i);
					}
					catch (Exception)
					{
						if (BloxUtil.TryConvert(value, elementType, out value))
						{
							array.SetValue(value, array.Length - 1);
							return array;
						}
						throw new Exception("Expected value type was [" + elementType + "] but the value was of type [" + ((value != null) ? value.GetType() : null) + "]");
					}
				}
				else
				{
					array.SetValue(arr.GetValue(num), i);
					num++;
				}
			}
			return array;
		}

		public static Array ArraySet(Array arr, object value, int index)
		{
			if (arr == null)
			{
				return null;
			}
			try
			{
				arr.SetValue(value, index);
				return arr;
			}
			catch (Exception)
			{
				Type elementType = arr.GetType().GetElementType();
				if (BloxUtil.TryConvert(value, elementType, out value))
				{
					arr.SetValue(value, arr.Length - 1);
					return arr;
				}
				throw new Exception("Expected value type was [" + elementType + "] but the value was of type [" + ((value != null) ? value.GetType() : null) + "]");
			}
		}

		public static Array ArrayClear(Array arr)
		{
			if (arr == null)
			{
				return null;
			}
			return Array.CreateInstance(arr.GetType().GetElementType(), 0);
		}

		public static Array ArrayClone(Array arr)
		{
			if (arr == null)
			{
				return null;
			}
			Array array = Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
			arr.CopyTo(array, 0);
			return array;
		}

		public static bool ArrayContains(Array arr, object value)
		{
			if (arr == null)
			{
				return false;
			}
			Type elementType = arr.GetType().GetElementType();
			if (value != null && value.GetType() != elementType)
			{
				BloxUtil.TryConvert(value, elementType, out value);
			}
			for (int i = 0; i < arr.Length; i++)
			{
				if (BloxUtil.IsEqual(arr.GetValue(i), value, false))
				{
					return true;
				}
			}
			return false;
		}

		public static int ArrayIndexOf(Array arr, object value)
		{
			if (arr == null)
			{
				return -1;
			}
			Type elementType = arr.GetType().GetElementType();
			if (value != null && value.GetType() != elementType)
			{
				BloxUtil.TryConvert(value, elementType, out value);
			}
			for (int i = 0; i < arr.Length; i++)
			{
				if (BloxUtil.IsEqual(arr.GetValue(i), value, false))
				{
					return i;
				}
			}
			return -1;
		}

		public static Array ArrayRemove(Array arr, object value)
		{
			if (arr == null)
			{
				return null;
			}
			if (arr.Length == 0)
			{
				return Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
			}
			int num = BloxUtil.ArrayIndexOf(arr, value);
			if (num >= 0)
			{
				return BloxUtil.ArrayRemoveAt(arr, num);
			}
			return arr;
		}

		public static Array ArrayRemoveAt(Array arr, int index)
		{
			if (arr == null)
			{
				return null;
			}
			if (arr.Length == 0)
			{
				return Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
			}
			Array array = Array.CreateInstance(arr.GetType().GetElementType(), arr.Length - 1);
			if (array.Length == 0)
			{
				return array;
			}
			int num = 0;
			for (int i = 0; i < arr.Length; i++)
			{
				if (i != index)
				{
					array.SetValue(arr.GetValue(i), num);
					num++;
				}
			}
			return array;
		}

		public static IList ArrayToList(Array arr)
		{
			if (arr == null)
			{
				return null;
			}
			IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(arr.GetType().GetElementType()));
			for (int i = 0; i < arr.Length; i++)
			{
				list.Add(arr.GetValue(i));
			}
			return list;
		}

		public static void ArraySort(Array arr)
		{
			if (((arr != null) ? arr.Length : 0) != 0)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < arr.Length; i++)
				{
					list.Add(arr.GetValue(i));
				}
				list.Sort();
				for (int j = 0; j < arr.Length; j++)
				{
					arr.SetValue(list[j], j);
				}
			}
		}

		public static Array ArraySortRes(Array arr)
		{
			if (arr == null)
			{
				return null;
			}
			if (arr.Length == 0)
			{
				return Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
			}
			List<object> list = new List<object>();
			for (int i = 0; i < arr.Length; i++)
			{
				list.Add(arr.GetValue(i));
			}
			list.Sort();
			Array array = Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
			for (int j = 0; j < array.Length; j++)
			{
				array.SetValue(list[j], j);
			}
			return array;
		}

		public static void ArrayReverse(Array arr)
		{
			if (((arr != null) ? arr.Length : 0) != 0)
			{
				BloxUtil.ArrayReverseRes(arr).CopyTo(arr, 0);
			}
		}

		public static Array ArrayReverseRes(Array arr)
		{
			if (arr == null)
			{
				return null;
			}
			if (arr.Length == 0)
			{
				return Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
			}
			Array array = Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
			int num = arr.Length - 1;
			for (int i = 0; i < arr.Length; i++)
			{
				array.SetValue(arr.GetValue(i), num);
				num--;
			}
			return array;
		}

		public static void ListAdd(IList list, object value)
		{
			if (list != null)
			{
				try
				{
					list.Add(value);
				}
				catch (Exception)
				{
					Type type = list.GetType().GetGenericArguments()[0];
					if (BloxUtil.TryConvert(value, type, out value))
					{
						list.Add(value);
						goto end_IL_000f;
					}
					throw new Exception("Expected value type was [" + type + "] but the value was of type [" + ((value != null) ? value.GetType() : null) + "]");
					end_IL_000f:;
				}
			}
		}

		public static void ListInsert(IList list, object value, int index)
		{
			if (list != null)
			{
				try
				{
					list.Insert(index, value);
				}
				catch (Exception)
				{
					Type type = list.GetType().GetGenericArguments()[0];
					if (BloxUtil.TryConvert(value, type, out value))
					{
						list.Insert(index, value);
						goto end_IL_000f;
					}
					throw new Exception("Expected value type was [" + type + "] but the value was of type [" + ((value != null) ? value.GetType() : null) + "]");
					end_IL_000f:;
				}
			}
		}

		public static void ListSet(IList list, object value, int index)
		{
			if (list != null)
			{
				try
				{
					list[index] = value;
				}
				catch (Exception)
				{
					Type type = list.GetType().GetGenericArguments()[0];
					if (BloxUtil.TryConvert(value, type, out value))
					{
						list[index] = value;
						goto end_IL_000f;
					}
					throw new Exception("Expected value type was [" + type + "] but the value was of type [" + ((value != null) ? value.GetType() : null) + "]");
					end_IL_000f:;
				}
			}
		}

		public static void ListClear(IList list)
		{
			if (list != null)
			{
				list.Clear();
			}
		}

		public static IList ListClone(IList list)
		{
			if (list == null)
			{
				return null;
			}
			IList list2 = (IList)Activator.CreateInstance(list.GetType());
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(list[i]);
			}
			return list2;
		}

		public static bool ListContains(IList list, object value)
		{
			if (list == null)
			{
				return false;
			}
			Type type = list.GetType().GetGenericArguments()[0];
			if (value != null && value.GetType() != type)
			{
				BloxUtil.TryConvert(value, type, out value);
			}
			return list.Contains(value);
		}

		public static int ListIndexOf(IList list, object value)
		{
			if (list == null)
			{
				return -1;
			}
			Type type = list.GetType().GetGenericArguments()[0];
			if (value != null && value.GetType() != type)
			{
				BloxUtil.TryConvert(value, type, out value);
			}
			return list.IndexOf(value);
		}

		public static void ListRemove(IList list, object value)
		{
			if (list != null)
			{
				Type type = list.GetType().GetGenericArguments()[0];
				if (value != null && value.GetType() != type)
				{
					BloxUtil.TryConvert(value, type, out value);
				}
				list.Remove(value);
			}
		}

		public static void ListRemoveAt(IList list, int index)
		{
			if (list != null)
			{
				list.RemoveAt(index);
			}
		}

		public static Array ListToArray(IList list)
		{
			if (list == null)
			{
				return null;
			}
			Array array = Array.CreateInstance(list.GetType().GetGenericArguments()[0], list.Count);
			list.CopyTo(array, 0);
			return array;
		}

		public static void ListSort(IList list)
		{
			if (((list != null) ? list.Count : 0) != 0)
			{
				List<object> list2 = new List<object>();
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(list[i]);
				}
				list2.Sort();
				for (int j = 0; j < list2.Count; j++)
				{
					list[j] = list2[j];
				}
			}
		}

		public static IList ListSortRes(IList list)
		{
			if (list == null)
			{
				return null;
			}
			IList list2 = (IList)Activator.CreateInstance(list.GetType());
			if (list.Count == 0)
			{
				List<object> list3 = new List<object>();
				for (int i = 0; i < list.Count; i++)
				{
					list3.Add(list[i]);
				}
				list3.Sort();
				for (int j = 0; j < list.Count; j++)
				{
					list2.Add(list3[j]);
				}
			}
			return list2;
		}

		public static void ListReverse(IList list)
		{
			if (((list != null) ? list.Count : 0) != 0)
			{
				List<object> list2 = new List<object>();
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(list[i]);
				}
				list2.Reverse();
				for (int j = 0; j < list2.Count; j++)
				{
					list[j] = list2[j];
				}
			}
		}

		public static IList ListReverseRes(IList list)
		{
			if (list == null)
			{
				return null;
			}
			IList list2 = (IList)Activator.CreateInstance(list.GetType());
			if (list.Count == 0)
			{
				List<object> list3 = new List<object>();
				for (int i = 0; i < list.Count; i++)
				{
					list3.Add(list[i]);
				}
				list3.Reverse();
				for (int j = 0; j < list.Count; j++)
				{
					list2.Add(list3[j]);
				}
			}
			return list2;
		}

		public static bool NOT(bool val)
		{
			return !val;
		}

		public static Type FindType(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			try
			{
				Type type = null;
				if (BloxUtil._cachedTypes.TryGetValue(name, out type))
				{
					return type;
				}
				if ((type = Type.GetType(name)) != null)
				{
					BloxUtil._cachedTypes.Add(name, type);
					return type;
				}
				if ((type = Type.GetType("UnityEngine." + name + ", UnityEngine")) != null)
				{
					BloxUtil._cachedTypes.Add(name, type);
					return type;
				}
				if ((type = Type.GetType(name + ", Assembly-CSharp")) != null)
				{
					BloxUtil._cachedTypes.Add(name, type);
					return type;
				}
				if (BloxUtil._priorityAsms == null)
				{
					BloxUtil._priorityAsms = new List<Assembly>();
					BloxUtil._otherAsms = new List<Assembly>();
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					for (int i = 0; i < assemblies.Length; i++)
					{
						if (assemblies[i].FullName.Contains("UnityEngine"))
						{
							BloxUtil._priorityAsms.Add(assemblies[i]);
						}
						else if (assemblies[i].FullName.Contains("Assembly-CSharp"))
						{
							BloxUtil._priorityAsms.Add(assemblies[i]);
						}
						else
						{
							BloxUtil._otherAsms.Add(assemblies[i]);
						}
					}
				}
				for (int j = 0; j < BloxUtil._priorityAsms.Count; j++)
				{
					Type[] exportedTypes = BloxUtil._priorityAsms[j].GetExportedTypes();
					for (int k = 0; k < exportedTypes.Length; k++)
					{
						if (exportedTypes[k].Name == name)
						{
							BloxUtil._cachedTypes.Add(name, exportedTypes[k]);
							return exportedTypes[k];
						}
					}
				}
				for (int l = 0; l < BloxUtil._otherAsms.Count; l++)
				{
					Type[] exportedTypes2 = BloxUtil._otherAsms[l].GetExportedTypes();
					for (int m = 0; m < exportedTypes2.Length; m++)
					{
						if (exportedTypes2[m].Name == name)
						{
							BloxUtil._cachedTypes.Add(name, exportedTypes2[m]);
							return exportedTypes2[m];
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}
