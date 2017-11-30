using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BloxEngine
{
	[ExcludeFromBlox]
	public static class BloxStringSerializer
	{
		private static readonly Dictionary<Type, Func<object, string>> writers;

		private static readonly Dictionary<Type, Func<string, object>> readers;

		public static string Serialize(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			Type type = obj.GetType();
			if (type.IsEnum)
			{
				return BloxStringSerializer.GetData((int)obj);
			}
			if (type.IsArray)
			{
				return BloxStringSerializer.SerializeArray(obj);
			}
			if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(List<>))
				{
					return BloxStringSerializer.SerializeList(obj);
				}
				return null;
			}
			if (BloxStringSerializer.writers.ContainsKey(type))
			{
				return BloxStringSerializer.writers[type](obj);
			}
			return JsonUtility.ToJson(obj);
		}

		public static object Deserialize(string data, Type t)
		{
			if (t == null)
			{
				return null;
			}
			if (((data != null) ? data.Length : 0) != 0)
			{
				if (t.IsEnum)
				{
					return Enum.ToObject(t, BloxStringSerializer.ToInt(data));
				}
				if (t.IsArray)
				{
					return BloxStringSerializer.DeserializeArray(data, t);
				}
				if (t.IsGenericType)
				{
					if (t.GetGenericTypeDefinition() == typeof(List<>))
					{
						return BloxStringSerializer.DeserializeList(data, t);
					}
					return null;
				}
				if (BloxStringSerializer.readers.ContainsKey(t))
				{
					return BloxStringSerializer.readers[t](data);
				}
				return JsonUtility.FromJson(data, t);
			}
			return BloxMemberInfo.GetDefaultValue(t);
		}

		public static string SerializeArray(object obj)
		{
			Array array = obj as Array;
			if (array == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter writer = new StringWriter(stringBuilder))
			{
				BloxStringSerializer.WriteIntToStringWriter(writer, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					string text = BloxStringSerializer.Serialize(array.GetValue(i));
					BloxStringSerializer.WriteIntToStringWriter(writer, (text != null) ? text.Length : 0);
				}
			}
			return stringBuilder.ToString();
		}

		public static string SerializeList(object obj)
		{
			IList list = obj as IList;
			if (list == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				BloxStringSerializer.WriteIntToStringWriter(stringWriter, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					string text = BloxStringSerializer.Serialize(list[i]);
					BloxStringSerializer.WriteIntToStringWriter(stringWriter, (text != null) ? text.Length : 0);
					stringWriter.Write(text);
				}
			}
			return stringBuilder.ToString();
		}

		public static object DeserializeArray(string data, Type t)
		{
			t = t.GetElementType();
			if (((data != null) ? data.Length : 0) != 0)
			{
				Array array = null;
				using (StringReader stringReader = new StringReader(data))
				{
					int num = BloxStringSerializer.ReadIntFromStringReader(stringReader);
					array = Array.CreateInstance(t, num);
					if (num > 0)
					{
						for (int i = 0; i < num; i++)
						{
							int num2 = BloxStringSerializer.ReadIntFromStringReader(stringReader);
							if (num2 == 0)
							{
								array.SetValue(BloxStringSerializer.Deserialize(null, t), i);
							}
							else
							{
								char[] array2 = new char[num2];
								stringReader.ReadBlock(array2, 0, num2);
								object value = BloxStringSerializer.Deserialize(array2.ToString(), t);
								array.SetValue(value, i);
							}
						}
						return array;
					}
					return array;
				}
			}
			return Array.CreateInstance(t, 0);
		}

		public static object DeserializeList(string data, Type t)
		{
			IList list = (IList)Activator.CreateInstance(t);
			if (((data != null) ? data.Length : 0) != 0)
			{
				t = t.GetGenericArguments()[0];
				using (StringReader stringReader = new StringReader(data))
				{
					int num = BloxStringSerializer.ReadIntFromStringReader(stringReader);
					if (num > 0)
					{
						for (int i = 0; i < num; i++)
						{
							int num2 = BloxStringSerializer.ReadIntFromStringReader(stringReader);
							if (num2 == 0)
							{
								list.Add(BloxStringSerializer.Deserialize(null, t));
							}
							else
							{
								char[] array = new char[num2];
								stringReader.ReadBlock(array, 0, num2);
								object value = BloxStringSerializer.Deserialize(array.ToString(), t);
								list.Add(value);
							}
						}
						return list;
					}
					return list;
				}
			}
			return list;
		}

		private static void WriteIntToStringWriter(StringWriter writer, int val)
		{
			writer.Write(val);
			writer.Write(";");
		}

		private static int ReadIntFromStringReader(StringReader reader)
		{
			string text = "";
			while (true)
			{
				int num = reader.Read();
				if (num == -1)
					break;
				char c = Convert.ToChar(num);
				if (c == ';')
					break;
				text += c.ToString();
			}
			return int.Parse(text);
		}

		static BloxStringSerializer()
		{
			BloxStringSerializer.writers = new Dictionary<Type, Func<object, string>>();
			BloxStringSerializer.readers = new Dictionary<Type, Func<string, object>>();
			BloxStringSerializer.writers[typeof(bool)] = ((object value) => BloxStringSerializer.GetData((bool)value));
			BloxStringSerializer.writers[typeof(byte)] = ((object value) => BloxStringSerializer.GetData((byte)value));
			BloxStringSerializer.writers[typeof(sbyte)] = ((object value) => BloxStringSerializer.GetData((sbyte)value));
			BloxStringSerializer.writers[typeof(char)] = ((object value) => BloxStringSerializer.GetData((char)value));
			BloxStringSerializer.writers[typeof(int)] = ((object value) => BloxStringSerializer.GetData((int)value));
			BloxStringSerializer.writers[typeof(uint)] = ((object value) => BloxStringSerializer.GetData((uint)value));
			BloxStringSerializer.writers[typeof(short)] = ((object value) => BloxStringSerializer.GetData((short)value));
			BloxStringSerializer.writers[typeof(ushort)] = ((object value) => BloxStringSerializer.GetData((ushort)value));
			BloxStringSerializer.writers[typeof(long)] = ((object value) => BloxStringSerializer.GetData((long)value));
			BloxStringSerializer.writers[typeof(ulong)] = ((object value) => BloxStringSerializer.GetData((ulong)value));
			BloxStringSerializer.writers[typeof(float)] = ((object value) => BloxStringSerializer.GetData((float)value));
			BloxStringSerializer.writers[typeof(double)] = ((object value) => BloxStringSerializer.GetData((double)value));
			BloxStringSerializer.writers[typeof(decimal)] = ((object value) => BloxStringSerializer.GetData((decimal)value));
			BloxStringSerializer.writers[typeof(string)] = ((object value) => BloxStringSerializer.GetData((string)value));
			BloxStringSerializer.writers[typeof(Vector2)] = ((object value) => BloxStringSerializer.GetData((Vector2)value));
			BloxStringSerializer.writers[typeof(Vector3)] = ((object value) => BloxStringSerializer.GetData((Vector3)value));
			BloxStringSerializer.writers[typeof(Vector4)] = ((object value) => BloxStringSerializer.GetData((Vector4)value));
			BloxStringSerializer.writers[typeof(Quaternion)] = ((object value) => BloxStringSerializer.GetData((Quaternion)value));
			BloxStringSerializer.writers[typeof(Rect)] = ((object value) => BloxStringSerializer.GetData((Rect)value));
			BloxStringSerializer.writers[typeof(Color)] = ((object value) => BloxStringSerializer.GetData((Color)value));
			BloxStringSerializer.writers[typeof(Color32)] = ((object value) => BloxStringSerializer.GetData((Color32)value));
			BloxStringSerializer.readers[typeof(bool)] = ((string data) => BloxStringSerializer.ToBool(data));
			BloxStringSerializer.readers[typeof(byte)] = ((string data) => BloxStringSerializer.ToByte(data));
			BloxStringSerializer.readers[typeof(sbyte)] = ((string data) => BloxStringSerializer.ToSByte(data));
			BloxStringSerializer.readers[typeof(char)] = ((string data) => BloxStringSerializer.ToChar(data));
			BloxStringSerializer.readers[typeof(int)] = ((string data) => BloxStringSerializer.ToInt(data));
			BloxStringSerializer.readers[typeof(uint)] = ((string data) => BloxStringSerializer.ToUInt(data));
			BloxStringSerializer.readers[typeof(short)] = ((string data) => BloxStringSerializer.ToShort(data));
			BloxStringSerializer.readers[typeof(ushort)] = ((string data) => BloxStringSerializer.ToUShort(data));
			BloxStringSerializer.readers[typeof(long)] = ((string data) => BloxStringSerializer.ToLong(data));
			BloxStringSerializer.readers[typeof(ulong)] = ((string data) => BloxStringSerializer.ToULong(data));
			BloxStringSerializer.readers[typeof(float)] = ((string data) => BloxStringSerializer.ToFloat(data));
			BloxStringSerializer.readers[typeof(double)] = ((string data) => BloxStringSerializer.ToDouble(data));
			BloxStringSerializer.readers[typeof(decimal)] = ((string data) => BloxStringSerializer.ToDecimal(data));
			BloxStringSerializer.readers[typeof(string)] = ((string data) => BloxStringSerializer.ToString(data));
			BloxStringSerializer.readers[typeof(Vector2)] = ((string data) => BloxStringSerializer.ToVector2(data));
			BloxStringSerializer.readers[typeof(Vector3)] = ((string data) => BloxStringSerializer.ToVector3(data));
			BloxStringSerializer.readers[typeof(Vector4)] = ((string data) => BloxStringSerializer.ToVector4(data));
			BloxStringSerializer.readers[typeof(Quaternion)] = ((string data) => BloxStringSerializer.ToQuaternion(data));
			BloxStringSerializer.readers[typeof(Rect)] = ((string data) => BloxStringSerializer.ToRect(data));
			BloxStringSerializer.readers[typeof(Color)] = ((string data) => BloxStringSerializer.ToColor(data));
			BloxStringSerializer.readers[typeof(Color32)] = ((string data) => BloxStringSerializer.ToColor32(data));
		}

		public static string GetData(bool value)
		{
			return value.ToString();
		}

		public static string GetData(byte value)
		{
			return value.ToString();
		}

		public static string GetData(sbyte value)
		{
			return value.ToString();
		}

		public static string GetData(char value)
		{
			return value.ToString();
		}

		public static string GetData(int value)
		{
			return value.ToString();
		}

		public static string GetData(uint value)
		{
			return value.ToString();
		}

		public static string GetData(short value)
		{
			return value.ToString();
		}

		public static string GetData(ushort value)
		{
			return value.ToString();
		}

		public static string GetData(long value)
		{
			return value.ToString();
		}

		public static string GetData(ulong value)
		{
			return value.ToString();
		}

		public static string GetData(float value)
		{
			return value.ToString();
		}

		public static string GetData(double value)
		{
			return value.ToString();
		}

		public static string GetData(decimal value)
		{
			return value.ToString();
		}

		public static string GetData(string value)
		{
			return value;
		}

		public static string GetData(Vector2 value)
		{
			return value.x + "," + value.y;
		}

		public static string GetData(Vector3 value)
		{
			return value.x + "," + value.y + "," + value.z;
		}

		public static string GetData(Vector4 value)
		{
			return value.x + "," + value.y + "," + value.z + "," + value.w;
		}

		public static string GetData(Quaternion value)
		{
			return value.x + "," + value.y + "," + value.z + "," + value.w;
		}

		public static string GetData(Color value)
		{
			return value.r + "," + value.g + "," + value.b + "," + value.a;
		}

		public static string GetData(Color32 value)
		{
			return value.r + "," + value.g + "," + value.b + "," + value.a;
		}

		public static string GetData(Rect value)
		{
			return value.x + "," + value.y + "," + value.width + "," + value.height;
		}

		public static byte ToByte(string data)
		{
			try
			{
				return byte.Parse(data);
			}
			catch
			{
				return 0;
			}
		}

		public static sbyte ToSByte(string data)
		{
			try
			{
				return sbyte.Parse(data);
			}
			catch
			{
				return 0;
			}
		}

		public static char ToChar(string data)
		{
			try
			{
				return char.Parse(data);
			}
			catch
			{
				return '\0';
			}
		}

		public static bool ToBool(string data)
		{
			try
			{
				return bool.Parse(data);
			}
			catch
			{
				return false;
			}
		}

		public static short ToShort(string data)
		{
			try
			{
				return short.Parse(data);
			}
			catch
			{
				return 0;
			}
		}

		public static ushort ToUShort(string data)
		{
			try
			{
				return ushort.Parse(data);
			}
			catch
			{
				return 0;
			}
		}

		public static uint ToUInt(string data)
		{
			try
			{
				return uint.Parse(data);
			}
			catch
			{
				return 0u;
			}
		}

		public static int ToInt(string data)
		{
			try
			{
				return int.Parse(data);
			}
			catch
			{
				return 0;
			}
		}

		public static long ToLong(string data)
		{
			try
			{
				return long.Parse(data);
			}
			catch
			{
				return 0L;
			}
		}

		public static ulong ToULong(string data)
		{
			try
			{
				return ulong.Parse(data);
			}
			catch
			{
				return 0uL;
			}
		}

		public static float ToFloat(string data)
		{
			try
			{
				return float.Parse(data);
			}
			catch
			{
				return 0f;
			}
		}

		public static double ToDouble(string data)
		{
			try
			{
				return double.Parse(data);
			}
			catch
			{
				return 0.0;
			}
		}

		public static decimal ToDecimal(string data)
		{
			try
			{
				return decimal.Parse(data);
			}
			catch
			{
				return decimal.Zero;
			}
		}

		public static string ToString(string data)
		{
			return data;
		}

		public static Rect ToRect(string data)
		{
			try
			{
				string[] array = data.Split(',');
				if (array.Length == 4)
				{
					return new Rect(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				}
			}
			catch
			{
			}
			return default(Rect);
		}

		public static Vector2 ToVector2(string data)
		{
			try
			{
				string[] array = data.Split(',');
				if (array.Length == 2)
				{
					return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
				}
			}
			catch
			{
			}
			return default(Vector2);
		}

		public static Vector3 ToVector3(string data)
		{
			try
			{
				string[] array = data.Split(',');
				if (array.Length == 3)
				{
					return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
				}
			}
			catch
			{
			}
			return default(Vector3);
		}

		public static Vector4 ToVector4(string data)
		{
			try
			{
				string[] array = data.Split(',');
				if (array.Length == 4)
				{
					return new Vector4(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				}
			}
			catch
			{
			}
			return default(Vector4);
		}

		public static Quaternion ToQuaternion(string data)
		{
			try
			{
				string[] array = data.Split(',');
				if (array.Length == 4)
				{
					return new Quaternion(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				}
			}
			catch
			{
			}
			return default(Quaternion);
		}

		public static Color ToColor(string data)
		{
			try
			{
				string[] array = data.Split(',');
				if (array.Length == 4)
				{
					return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				}
			}
			catch
			{
			}
			return default(Color);
		}

		public static Color32 ToColor32(string data)
		{
			try
			{
				string[] array = data.Split(',');
				if (array.Length == 4)
				{
					return new Color32(byte.Parse(array[0]), byte.Parse(array[1]), byte.Parse(array[2]), byte.Parse(array[3]));
				}
			}
			catch
			{
			}
			return default(Color32);
		}
	}
}
