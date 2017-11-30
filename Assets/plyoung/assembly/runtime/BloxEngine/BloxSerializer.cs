using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BloxEngine
{
	[ExcludeFromBlox]
	public static class BloxSerializer
	{
		private static readonly Dictionary<Type, Func<object, byte[]>> writers;

		private static readonly Dictionary<Type, Func<byte[], object>> readers;

		public static byte[] Serialize(object obj)
		{
			if (obj == null)
			{
				return new byte[0];
			}
			Type type = obj.GetType();
			if (type.IsEnum)
			{
				return BloxSerializer.GetBytes((int)obj);
			}
			if (type.IsArray)
			{
				return BloxSerializer.SerializeArray(obj);
			}
			if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(List<>))
				{
					return BloxSerializer.SerializeList(obj);
				}
				return new byte[0];
			}
			if (BloxSerializer.writers.ContainsKey(type))
			{
				return BloxSerializer.writers[type](obj);
			}
			return new byte[0];
		}

		public static object Deserialize(byte[] data, Type t)
		{
			if (t == null)
			{
				return null;
			}
			if (((data != null) ? data.Length : 0) != 0)
			{
				if (t.IsEnum)
				{
					return Enum.ToObject(t, BloxSerializer.ToInt(data));
				}
				if (t.IsArray)
				{
					return BloxSerializer.DeserializeArray(data, t);
				}
				if (t.IsGenericType)
				{
					if (t.GetGenericTypeDefinition() == typeof(List<>))
					{
						return BloxSerializer.DeserializeList(data, t);
					}
					return new byte[0];
				}
				if (BloxSerializer.readers.ContainsKey(t))
				{
					return BloxSerializer.readers[t](data);
				}
				return null;
			}
			return BloxMemberInfo.GetDefaultValue(t);
		}

		public static byte[] SerializeArray(object obj)
		{
			Array array = obj as Array;
			if (array == null)
			{
				return new byte[0];
			}
			MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					byte[] array2 = BloxSerializer.Serialize(array.GetValue(i));
					binaryWriter.Write(array2.Length);
					binaryWriter.Write(array2);
				}
			}
			memoryStream.Flush();
			byte[] buffer = memoryStream.GetBuffer();
			memoryStream.Close();
			return buffer;
		}

		public static byte[] SerializeList(object obj)
		{
			IList list = obj as IList;
			if (list == null)
			{
				return new byte[0];
			}
			MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					byte[] array = BloxSerializer.Serialize(list[i]);
					binaryWriter.Write(array.Length);
					binaryWriter.Write(array);
				}
			}
			memoryStream.Flush();
			byte[] buffer = memoryStream.GetBuffer();
			memoryStream.Close();
			return buffer;
		}

		public static object DeserializeArray(byte[] data, Type t)
		{
			t = t.GetElementType();
			if (((data != null) ? data.Length : 0) != 0)
			{
				Array array = null;
				using (MemoryStream input = new MemoryStream(data, false))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						int num = binaryReader.ReadInt32();
						array = Array.CreateInstance(t, num);
						for (int i = 0; i < num; i++)
						{
							int num2 = binaryReader.ReadInt32();
							byte[] array2 = new byte[num2];
							binaryReader.Read(array2, 0, num2);
							object value = BloxSerializer.Deserialize(array2, t);
							array.SetValue(value, i);
						}
						return array;
					}
				}
			}
			return Array.CreateInstance(t, 0);
		}

		public static object DeserializeList(byte[] data, Type t)
		{
			IList list = (IList)Activator.CreateInstance(t);
			if (((data != null) ? data.Length : 0) != 0)
			{
				t = t.GetGenericArguments()[0];
				using (MemoryStream input = new MemoryStream(data, false))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							int num2 = binaryReader.ReadInt32();
							byte[] array = new byte[num2];
							binaryReader.Read(array, 0, num2);
							object value = BloxSerializer.Deserialize(array, t);
							list.Add(value);
						}
						return list;
					}
				}
			}
			return list;
		}

		static BloxSerializer()
		{
			BloxSerializer.writers = new Dictionary<Type, Func<object, byte[]>>();
			BloxSerializer.readers = new Dictionary<Type, Func<byte[], object>>();
			BloxSerializer.writers[typeof(bool)] = ((object value) => BloxSerializer.GetBytes((bool)value));
			BloxSerializer.writers[typeof(byte)] = ((object value) => BloxSerializer.GetBytes((byte)value));
			BloxSerializer.writers[typeof(sbyte)] = ((object value) => BloxSerializer.GetBytes((sbyte)value));
			BloxSerializer.writers[typeof(char)] = ((object value) => BloxSerializer.GetBytes((char)value));
			BloxSerializer.writers[typeof(int)] = ((object value) => BloxSerializer.GetBytes((int)value));
			BloxSerializer.writers[typeof(uint)] = ((object value) => BloxSerializer.GetBytes((uint)value));
			BloxSerializer.writers[typeof(short)] = ((object value) => BloxSerializer.GetBytes((short)value));
			BloxSerializer.writers[typeof(ushort)] = ((object value) => BloxSerializer.GetBytes((ushort)value));
			BloxSerializer.writers[typeof(long)] = ((object value) => BloxSerializer.GetBytes((long)value));
			BloxSerializer.writers[typeof(ulong)] = ((object value) => BloxSerializer.GetBytes((ulong)value));
			BloxSerializer.writers[typeof(float)] = ((object value) => BloxSerializer.GetBytes((float)value));
			BloxSerializer.writers[typeof(double)] = ((object value) => BloxSerializer.GetBytes((double)value));
			BloxSerializer.writers[typeof(decimal)] = ((object value) => BloxSerializer.GetBytes((decimal)value));
			BloxSerializer.writers[typeof(string)] = ((object value) => BloxSerializer.GetBytes((string)value));
			BloxSerializer.writers[typeof(Vector2)] = ((object value) => BloxSerializer.GetBytes((Vector2)value));
			BloxSerializer.writers[typeof(Vector3)] = ((object value) => BloxSerializer.GetBytes((Vector3)value));
			BloxSerializer.writers[typeof(Vector4)] = ((object value) => BloxSerializer.GetBytes((Vector4)value));
			BloxSerializer.writers[typeof(Quaternion)] = ((object value) => BloxSerializer.GetBytes((Quaternion)value));
			BloxSerializer.writers[typeof(Rect)] = ((object value) => BloxSerializer.GetBytes((Rect)value));
			BloxSerializer.writers[typeof(Color)] = ((object value) => BloxSerializer.GetBytes((Color)value));
			BloxSerializer.writers[typeof(Color32)] = ((object value) => BloxSerializer.GetBytes((Color32)value));
			BloxSerializer.readers[typeof(bool)] = ((byte[] data) => BloxSerializer.ToBool(data));
			BloxSerializer.readers[typeof(byte)] = ((byte[] data) => BloxSerializer.ToByte(data));
			BloxSerializer.readers[typeof(sbyte)] = ((byte[] data) => BloxSerializer.ToSByte(data));
			BloxSerializer.readers[typeof(char)] = ((byte[] data) => BloxSerializer.ToChar(data));
			BloxSerializer.readers[typeof(int)] = ((byte[] data) => BloxSerializer.ToInt(data));
			BloxSerializer.readers[typeof(uint)] = ((byte[] data) => BloxSerializer.ToUInt(data));
			BloxSerializer.readers[typeof(short)] = ((byte[] data) => BloxSerializer.ToShort(data));
			BloxSerializer.readers[typeof(ushort)] = ((byte[] data) => BloxSerializer.ToUShort(data));
			BloxSerializer.readers[typeof(long)] = ((byte[] data) => BloxSerializer.ToLong(data));
			BloxSerializer.readers[typeof(ulong)] = ((byte[] data) => BloxSerializer.ToULong(data));
			BloxSerializer.readers[typeof(float)] = ((byte[] data) => BloxSerializer.ToFloat(data));
			BloxSerializer.readers[typeof(double)] = ((byte[] data) => BloxSerializer.ToDouble(data));
			BloxSerializer.readers[typeof(decimal)] = ((byte[] data) => BloxSerializer.ToDecimal(data));
			BloxSerializer.readers[typeof(string)] = ((byte[] data) => BloxSerializer.ToString(data));
			BloxSerializer.readers[typeof(Vector2)] = ((byte[] data) => BloxSerializer.ToVector2(data));
			BloxSerializer.readers[typeof(Vector3)] = ((byte[] data) => BloxSerializer.ToVector3(data));
			BloxSerializer.readers[typeof(Vector4)] = ((byte[] data) => BloxSerializer.ToVector4(data));
			BloxSerializer.readers[typeof(Quaternion)] = ((byte[] data) => BloxSerializer.ToQuaternion(data));
			BloxSerializer.readers[typeof(Rect)] = ((byte[] data) => BloxSerializer.ToRect(data));
			BloxSerializer.readers[typeof(Color)] = ((byte[] data) => BloxSerializer.ToColor(data));
			BloxSerializer.readers[typeof(Color32)] = ((byte[] data) => BloxSerializer.ToColor32(data));
		}

		public static byte[] GetBytes(byte value)
		{
			return new byte[1]
			{
				value
			};
		}

		public static byte[] GetBytes(sbyte value)
		{
			return new byte[1]
			{
				(byte)value
			};
		}

		public static byte[] GetBytes(char value)
		{
			return new byte[1]
			{
				(byte)value
			};
		}

		public static byte[] GetBytes(bool value)
		{
			return new byte[1]
			{
				(byte)(value ? 1 : 0)
			};
		}

		public static byte[] GetBytes(short value)
		{
			return new byte[2]
			{
				(byte)value,
				(byte)(value >> 8)
			};
		}

		public static byte[] GetBytes(ushort value)
		{
			return new byte[2]
			{
				(byte)value,
				(byte)(value >> 8)
			};
		}

		public static byte[] GetBytes(uint value)
		{
			return new byte[4]
			{
				(byte)value,
				(byte)(value >> 8),
				(byte)(value >> 16),
				(byte)(value >> 24)
			};
		}

		public static byte[] GetBytes(int value)
		{
			return new byte[4]
			{
				(byte)value,
				(byte)(value >> 8),
				(byte)(value >> 16),
				(byte)(value >> 24)
			};
		}

		public static byte[] GetBytes(long value)
		{
			return new byte[8]
			{
				(byte)value,
				(byte)(value >> 8),
				(byte)(value >> 16),
				(byte)(value >> 24),
				(byte)(value >> 32),
				(byte)(value >> 40),
				(byte)(value >> 48),
				(byte)(value >> 56)
			};
		}

		public static byte[] GetBytes(ulong value)
		{
			return new byte[8]
			{
				(byte)value,
				(byte)(value >> 8),
				(byte)(value >> 16),
				(byte)(value >> 24),
				(byte)(value >> 32),
				(byte)(value >> 40),
				(byte)(value >> 48),
				(byte)(value >> 56)
			};
		}

		public static byte[] GetBytes(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		public static byte[] GetBytes(double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		public static byte[] GetBytes(decimal value)
		{
			byte[] obj = new byte[16];
			int[] bits = decimal.GetBits(value);
			int num = bits[0];
			int num2 = bits[1];
			int num3 = bits[2];
			int num4 = bits[3];
			obj[0] = (byte)num;
			obj[1] = (byte)(num >> 8);
			obj[2] = (byte)(num >> 16);
			obj[3] = (byte)(num >> 24);
			obj[4] = (byte)num2;
			obj[5] = (byte)(num2 >> 8);
			obj[6] = (byte)(num2 >> 16);
			obj[7] = (byte)(num2 >> 24);
			obj[8] = (byte)num3;
			obj[9] = (byte)(num3 >> 8);
			obj[10] = (byte)(num3 >> 16);
			obj[11] = (byte)(num3 >> 24);
			obj[12] = (byte)num4;
			obj[13] = (byte)(num4 >> 8);
			obj[14] = (byte)(num4 >> 16);
			obj[15] = (byte)(num4 >> 24);
			return obj;
		}

		public static byte[] GetBytes(string value)
		{
			return Encoding.Unicode.GetBytes(value);
		}

		public static byte[] GetBytes(Vector2 value)
		{
			byte[] array = new byte[8];
			byte[] bytes = BitConverter.GetBytes(value.x);
			byte[] bytes2 = BitConverter.GetBytes(value.y);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
				Array.Reverse(bytes2);
			}
			Buffer.BlockCopy(bytes, 0, array, 0, 4);
			Buffer.BlockCopy(bytes2, 0, array, 4, 4);
			return array;
		}

		public static byte[] GetBytes(Vector3 value)
		{
			byte[] array = new byte[12];
			byte[] bytes = BitConverter.GetBytes(value.x);
			byte[] bytes2 = BitConverter.GetBytes(value.y);
			byte[] bytes3 = BitConverter.GetBytes(value.z);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
				Array.Reverse(bytes2);
				Array.Reverse(bytes3);
			}
			Buffer.BlockCopy(bytes, 0, array, 0, 4);
			Buffer.BlockCopy(bytes2, 0, array, 4, 4);
			Buffer.BlockCopy(bytes3, 0, array, 8, 4);
			return array;
		}

		public static byte[] GetBytes(Vector4 value)
		{
			byte[] array = new byte[16];
			byte[] bytes = BitConverter.GetBytes(value.x);
			byte[] bytes2 = BitConverter.GetBytes(value.y);
			byte[] bytes3 = BitConverter.GetBytes(value.z);
			byte[] bytes4 = BitConverter.GetBytes(value.w);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
				Array.Reverse(bytes2);
				Array.Reverse(bytes3);
				Array.Reverse(bytes4);
			}
			Buffer.BlockCopy(bytes, 0, array, 0, 4);
			Buffer.BlockCopy(bytes2, 0, array, 4, 4);
			Buffer.BlockCopy(bytes3, 0, array, 8, 4);
			Buffer.BlockCopy(bytes4, 0, array, 12, 4);
			return array;
		}

		public static byte[] GetBytes(Quaternion value)
		{
			byte[] array = new byte[16];
			byte[] bytes = BitConverter.GetBytes(value.x);
			byte[] bytes2 = BitConverter.GetBytes(value.y);
			byte[] bytes3 = BitConverter.GetBytes(value.z);
			byte[] bytes4 = BitConverter.GetBytes(value.w);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
				Array.Reverse(bytes2);
				Array.Reverse(bytes3);
				Array.Reverse(bytes4);
			}
			Buffer.BlockCopy(bytes, 0, array, 0, 4);
			Buffer.BlockCopy(bytes2, 0, array, 4, 4);
			Buffer.BlockCopy(bytes3, 0, array, 8, 4);
			Buffer.BlockCopy(bytes4, 0, array, 12, 4);
			return array;
		}

		public static byte[] GetBytes(Color value)
		{
			byte[] array = new byte[16];
			byte[] bytes = BitConverter.GetBytes(value.r);
			byte[] bytes2 = BitConverter.GetBytes(value.g);
			byte[] bytes3 = BitConverter.GetBytes(value.b);
			byte[] bytes4 = BitConverter.GetBytes(value.a);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
				Array.Reverse(bytes2);
				Array.Reverse(bytes3);
				Array.Reverse(bytes4);
			}
			Buffer.BlockCopy(bytes, 0, array, 0, 4);
			Buffer.BlockCopy(bytes2, 0, array, 4, 4);
			Buffer.BlockCopy(bytes3, 0, array, 8, 4);
			Buffer.BlockCopy(bytes4, 0, array, 12, 4);
			return array;
		}

		public static byte[] GetBytes(Color32 value)
		{
			return new byte[4]
			{
				value.r,
				value.g,
				value.b,
				value.a
			};
		}

		public static byte[] GetBytes(Rect value)
		{
			byte[] array = new byte[16];
			byte[] bytes = BitConverter.GetBytes(value.x);
			byte[] bytes2 = BitConverter.GetBytes(value.y);
			byte[] bytes3 = BitConverter.GetBytes(value.width);
			byte[] bytes4 = BitConverter.GetBytes(value.height);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
				Array.Reverse(bytes2);
				Array.Reverse(bytes3);
				Array.Reverse(bytes4);
			}
			Buffer.BlockCopy(bytes, 0, array, 0, 4);
			Buffer.BlockCopy(bytes2, 0, array, 4, 4);
			Buffer.BlockCopy(bytes3, 0, array, 8, 4);
			Buffer.BlockCopy(bytes4, 0, array, 12, 4);
			return array;
		}

		public static byte ToByte(byte[] data)
		{
			return data[0];
		}

		public static sbyte ToSByte(byte[] data)
		{
			return (sbyte)data[0];
		}

		public static char ToChar(byte[] data)
		{
			return (char)data[0];
		}

		public static bool ToBool(byte[] data)
		{
			return data[0] != 0;
		}

		public static short ToShort(byte[] data)
		{
			return (short)(data[0] | data[1] << 8);
		}

		public static ushort ToUShort(byte[] data)
		{
			return (ushort)(data[0] | data[1] << 8);
		}

		public static uint ToUInt(byte[] data)
		{
			return (uint)(data[0] | data[1] << 8 | data[2] << 16 | data[3] << 24);
		}

		public static int ToInt(byte[] data)
		{
			return data[0] | data[1] << 8 | data[2] << 16 | data[3] << 24;
		}

		public static long ToLong(byte[] data)
		{
			uint num = (uint)(data[0] | data[1] << 8 | data[2] << 16 | data[3] << 24);
			return (long)((ulong)(uint)(data[4] | data[5] << 8 | data[6] << 16 | data[7] << 24) << 32 | num);
		}

		public static ulong ToULong(byte[] data)
		{
			uint num = (uint)(data[0] | data[1] << 8 | data[2] << 16 | data[3] << 24);
			return (ulong)(uint)(data[4] | data[5] << 8 | data[6] << 16 | data[7] << 24) << 32 | num;
		}

		public static float ToFloat(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 4);
			}
			return BitConverter.ToSingle(data, 0);
		}

		public static double ToDouble(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 8);
			}
			return BitConverter.ToDouble(data, 0);
		}

		public static decimal ToDecimal(byte[] data)
		{
			return new decimal(new int[4]
			{
				data[0] | data[1] << 8 | data[2] << 16 | data[3] << 24,
				data[4] | data[5] << 8 | data[6] << 16 | data[7] << 24,
				data[8] | data[9] << 8 | data[10] << 16 | data[11] << 24,
				data[12] | data[13] << 8 | data[14] << 16 | data[15] << 24
			});
		}

		public static string ToString(byte[] data)
		{
			return Encoding.Unicode.GetString(data);
		}

		public static Rect ToRect(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 4);
				Array.Reverse(data, 4, 4);
				Array.Reverse(data, 8, 4);
				Array.Reverse(data, 12, 4);
			}
			float x = BitConverter.ToSingle(data, 0);
			float y = BitConverter.ToSingle(data, 4);
			float width = BitConverter.ToSingle(data, 8);
			float height = BitConverter.ToSingle(data, 12);
			return new Rect(x, y, width, height);
		}

		public static Vector2 ToVector2(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 4);
				Array.Reverse(data, 4, 4);
			}
			float x = BitConverter.ToSingle(data, 0);
			float y = BitConverter.ToSingle(data, 4);
			return new Vector2(x, y);
		}

		public static Vector3 ToVector3(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 4);
				Array.Reverse(data, 4, 4);
				Array.Reverse(data, 8, 4);
			}
			float x = BitConverter.ToSingle(data, 0);
			float y = BitConverter.ToSingle(data, 4);
			float z = BitConverter.ToSingle(data, 8);
			return new Vector3(x, y, z);
		}

		public static Vector4 ToVector4(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 4);
				Array.Reverse(data, 4, 4);
				Array.Reverse(data, 8, 4);
				Array.Reverse(data, 12, 4);
			}
			float x = BitConverter.ToSingle(data, 0);
			float y = BitConverter.ToSingle(data, 4);
			float z = BitConverter.ToSingle(data, 8);
			float w = BitConverter.ToSingle(data, 12);
			return new Vector4(x, y, z, w);
		}

		public static Quaternion ToQuaternion(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 4);
				Array.Reverse(data, 4, 4);
				Array.Reverse(data, 8, 4);
				Array.Reverse(data, 12, 4);
			}
			float x = BitConverter.ToSingle(data, 0);
			float y = BitConverter.ToSingle(data, 4);
			float z = BitConverter.ToSingle(data, 8);
			float w = BitConverter.ToSingle(data, 12);
			return new Quaternion(x, y, z, w);
		}

		public static Color ToColor(byte[] data)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, 0, 4);
				Array.Reverse(data, 4, 4);
				Array.Reverse(data, 8, 4);
				Array.Reverse(data, 12, 4);
			}
			float r = BitConverter.ToSingle(data, 0);
			float g = BitConverter.ToSingle(data, 4);
			float b = BitConverter.ToSingle(data, 8);
			float a = BitConverter.ToSingle(data, 12);
			return new Color(r, g, b, a);
		}

		public static Color32 ToColor32(byte[] data)
		{
			return new Color32(data[0], data[1], data[2], data[3]);
		}
	}
}
