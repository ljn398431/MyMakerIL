using System;
using System.Reflection;
using UnityEngine;

namespace BloxEngine
{
	[Serializable]
	[ExcludeFromBlox]
	public class BloxBlockData
	{
		[Serializable]
		[ExcludeFromBlox]
		public struct BlockField
		{
			public string fieldName;

			public string typeName;

			public byte[] data;
		}

		public static readonly BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		public static readonly BindingFlags CtorBindFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		public string blockSysType;

		public BloxBlockType blockType;

		public string ident;

		public bool active;

		public Vector2 _ed_viewOffs;

		public string returnType;

		public byte[] returnValue = new byte[0];

		public string memberReflectedType;

		public MemberTypes memberType;

		public string memberName;

		public string[] paramTypes = new string[0];

		public BlockField[] fields = new BlockField[0];

		public int next = -1;

		public int firstChild = -1;

		public int contextBlock = -1;

		public int[] paramBlocks = new int[0];

		public BloxBlockData Copy()
		{
			BloxBlockData bloxBlockData = new BloxBlockData(null);
			bloxBlockData.blockSysType = this.blockSysType;
			bloxBlockData.blockType = this.blockType;
			bloxBlockData.ident = this.ident;
			bloxBlockData.active = this.active;
			bloxBlockData._ed_viewOffs = this._ed_viewOffs;
			bloxBlockData.returnType = this.returnType;
			bloxBlockData.returnValue = new byte[this.returnValue.Length];
			this.returnValue.CopyTo(bloxBlockData.returnValue, 0);
			bloxBlockData.memberReflectedType = this.memberReflectedType;
			bloxBlockData.memberType = this.memberType;
			bloxBlockData.memberName = this.memberName;
			bloxBlockData.paramTypes = new string[this.paramTypes.Length];
			this.paramTypes.CopyTo(bloxBlockData.paramTypes, 0);
			bloxBlockData.fields = new BlockField[this.fields.Length];
			this.fields.CopyTo(bloxBlockData.fields, 0);
			bloxBlockData.next = this.next;
			bloxBlockData.firstChild = this.firstChild;
			bloxBlockData.contextBlock = this.contextBlock;
			bloxBlockData.paramBlocks = new int[this.paramBlocks.Length];
			this.paramBlocks.CopyTo(bloxBlockData.paramBlocks, 0);
			return bloxBlockData;
		}

		public BloxBlockData(BloxBlock b)
		{
			if (b != null)
			{
				Type type = b.GetType();
				this.blockSysType = type.AssemblyQualifiedName;
				this.blockType = b.blockType;
				this.ident = b.ident;
				this.active = b.active;
				this._ed_viewOffs = b._ed_viewOffs;
				if (b.returnType != null)
				{
					this.returnType = b.returnType.AssemblyQualifiedName;
					this.returnValue = BloxSerializer.Serialize(b.returnValue);
				}
				if (b.mi != null && b.mi.IsValid)
				{
					this.memberReflectedType = b.mi.ReflectedType.AssemblyQualifiedName;
					this.memberType = b.mi.MemberType;
					this.memberName = b.mi.Name;
					ParameterInfo[] parameters = b.mi.GetParameters();
					if (((parameters != null) ? parameters.Length : 0) != 0)
					{
						this.paramTypes = new string[parameters.Length];
						for (int i = 0; i < parameters.Length; i++)
						{
							this.paramTypes[i] = parameters[i].ParameterType.AssemblyQualifiedName;
						}
					}
				}
				if (type != typeof(BloxBlock))
				{
					FieldInfo[] array = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
					this.fields = new BlockField[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						object value = array[j].GetValue(b);
						if (value != null)
						{
							this.fields[j].fieldName = array[j].Name;
							this.fields[j].typeName = array[j].FieldType.AssemblyQualifiedName;
							this.fields[j].data = BloxSerializer.Serialize(value);
						}
					}
				}
			}
		}

		public BloxBlock CreateBlock()
		{
			if (string.IsNullOrEmpty(this.blockSysType))
			{
				return null;
			}
			Type type = Type.GetType(this.blockSysType);
			if (type == null)
			{
				Debug.LogError("Could not find Block type: " + this.blockSysType);
				return null;
			}
			BloxBlock bloxBlock = (BloxBlock)Activator.CreateInstance(type);
			bloxBlock.blockType = this.blockType;
			bloxBlock.ident = this.ident;
			bloxBlock.active = this.active;
			bloxBlock._ed_viewOffs = this._ed_viewOffs;
			bloxBlock.returnType = (string.IsNullOrEmpty(this.returnType) ? null : Type.GetType(this.returnType));
			if (bloxBlock.returnType != null)
			{
				bloxBlock.returnValue = BloxSerializer.Deserialize(this.returnValue, bloxBlock.returnType);
			}
			if (!string.IsNullOrEmpty(this.memberName))
			{
				Type type2 = string.IsNullOrEmpty(this.memberReflectedType) ? null : Type.GetType(this.memberReflectedType);
				if (type2 != null)
				{
					if (this.memberType == MemberTypes.Field)
					{
						bloxBlock.mi = new BloxMemberInfo(type2.GetField(this.memberName, BloxBlockData.BindFlags), type2);
					}
					else if (this.memberType == MemberTypes.Property)
					{
						bloxBlock.mi = new BloxMemberInfo(type2.GetProperty(this.memberName, BloxBlockData.BindFlags), type2);
					}
					else if (this.memberType == MemberTypes.Constructor)
					{
						if (((this.paramTypes != null) ? this.paramTypes.Length : 0) != 0)
						{
							Type[] array = null;
							array = new Type[this.paramTypes.Length];
							for (int i = 0; i < this.paramTypes.Length; i++)
							{
								array[i] = Type.GetType(this.paramTypes[i]);
							}
							bloxBlock.mi = new BloxMemberInfo(type2.GetConstructor(BloxBlockData.CtorBindFlags, null, array, null), type2);
						}
						else
						{
							bloxBlock.mi = new BloxMemberInfo(type2.GetConstructor(Type.EmptyTypes), type2);
						}
					}
					else if (this.memberType == MemberTypes.Method)
					{
						if (((this.paramTypes != null) ? this.paramTypes.Length : 0) != 0)
						{
							Type[] array2 = null;
							array2 = new Type[this.paramTypes.Length];
							for (int j = 0; j < this.paramTypes.Length; j++)
							{
								array2[j] = Type.GetType(this.paramTypes[j]);
							}
							bloxBlock.mi = new BloxMemberInfo(type2.GetMethod(this.memberName, BloxBlockData.BindFlags, null, array2, null), type2);
						}
						else
						{
							bloxBlock.mi = new BloxMemberInfo(type2.GetMethod(this.memberName, BloxBlockData.BindFlags, null, new Type[0], null), type2);
						}
					}
				}
			}
			if (((type != typeof(BloxBlock)) ? this.fields.Length : 0) != 0)
			{
				FieldInfo[] array3 = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
				for (int k = 0; k < this.fields.Length; k++)
				{
					if (((this.fields[k].data != null) ? this.fields[k].data.Length : 0) != 0)
					{
						FieldInfo fieldInfo = null;
						if (k < array3.Length && array3[k].Name == this.fields[k].fieldName)
						{
							fieldInfo = array3[k];
						}
						else
						{
							int num = 0;
							while (num < array3.Length)
							{
								if (!(array3[num].Name == this.fields[k].fieldName))
								{
									num++;
									continue;
								}
								fieldInfo = array3[num];
								break;
							}
						}
						if (fieldInfo != null)
						{
							Type type3 = Type.GetType(this.fields[k].typeName);
							if (type3 != null && type3 == fieldInfo.FieldType)
							{
								object obj = BloxSerializer.Deserialize(this.fields[k].data, type3);
								if (obj != null)
								{
									fieldInfo.SetValue(bloxBlock, obj);
								}
							}
						}
					}
				}
			}
			return bloxBlock;
		}
	}
}
