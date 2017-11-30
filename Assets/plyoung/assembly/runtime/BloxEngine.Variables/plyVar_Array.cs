using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_Array : plyVarValueHandler
	{
		[SerializeField]
		private string plyVarTypeName;

		[SerializeField]
		private string baseTypeName;

		[SerializeField]
		private string[] storedEntriesDataData;

		[NonSerialized]
		public List<plyVar> variables = new List<plyVar>();

		[NonSerialized]
		private Type _plyVarType;

		[NonSerialized]
		private Type _baseType;

		[NonSerialized]
		private Type _arrayType;

		[NonSerialized]
		private Array arr;

		public override Type variableType
		{
			get
			{
				return this._arrayType;
			}
		}

		public Type baseType
		{
			get
			{
				return this._baseType;
			}
		}

		public Type plyVarType
		{
			get
			{
				return this._plyVarType;
			}
		}

		public void SetStoredType(Type baseType, Type plyVarType)
		{
			this._baseType = baseType;
			this._plyVarType = plyVarType;
			this._arrayType = this._baseType.MakeArrayType();
			this.baseTypeName = this._baseType.AssemblyQualifiedName;
			this.plyVarTypeName = this._plyVarType.AssemblyQualifiedName;
		}

		public void UpdateVarNames()
		{
			for (int i = 0; i < this.variables.Count; i++)
			{
				this.variables[i].name = i.ToString();
			}
		}

		public override void ReleaseStoredData()
		{
			this.variables = null;
			this.plyVarTypeName = null;
			this.baseTypeName = null;
		}

		public override object GetValue(plyVar wrapper)
		{
			return this.arr;
		}

		public override void SetValue(plyVar wrapper, object v)
		{
			if (v == null)
			{
				throw new Exception("Can't convert null to Array.");
			}
			Array array = v as Array;
			if (array == null)
			{
				throw new Exception("Can't convert " + v.GetType().Name + " to Array.");
			}
			this.arr = array;
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedEntriesDataData = null;
			this.arr = ((this._baseType == null) ? null : Array.CreateInstance(this._baseType, 0));
		}

		public override string EncodeValues(plyVar wrapper)
		{
			if (this._baseType != null && this._plyVarType != null)
			{
				this.baseTypeName = this._baseType.AssemblyQualifiedName;
				this.plyVarTypeName = this._plyVarType.AssemblyQualifiedName;
				if (typeof(UnityEngine.Object).IsAssignableFrom(this._baseType))
				{
					this.storedEntriesDataData = new string[0];
				}
				else
				{
					this.storedEntriesDataData = new string[this.variables.Count];
					for (int i = 0; i < this.variables.Count; i++)
					{
						this.storedEntriesDataData[i] = this.variables[i].ValueHandler.EncodeValues(wrapper);
					}
				}
				return base.EncodeValues(wrapper);
			}
			Debug.LogError("The List's generic type or plyVar type was not set.");
			return "";
		}

		public override void DecodeValues(plyVar wrapper, string data)
		{
			JsonUtility.FromJsonOverwrite(data, this);
			this._baseType = (string.IsNullOrEmpty(this.baseTypeName) ? null : Type.GetType(this.baseTypeName));
			this._plyVarType = (string.IsNullOrEmpty(this.plyVarTypeName) ? null : Type.GetType(this.plyVarTypeName));
			if (this._baseType == null || this._plyVarType == null)
			{
				Debug.LogErrorFormat("Could not find the Array's element type [{0}] or plyVar type [{1}]", this.baseTypeName, this.plyVarTypeName);
				this._arrayType = null;
			}
			else
			{
				this._arrayType = this._baseType.MakeArrayType();
				this.variables = new List<plyVar>();
				if (this.storedEntriesDataData == null)
				{
					this.arr = Array.CreateInstance(this._baseType, 0);
				}
				else if (typeof(UnityEngine.Object).IsAssignableFrom(this._baseType))
				{
					if (((wrapper.objRefs != null) ? wrapper.objRefs.Length : 0) != 0)
					{
						this.arr = Array.CreateInstance(this._baseType, wrapper.objRefs.Length);
						for (int i = 0; i < wrapper.objRefs.Length; i++)
						{
							plyVar plyVar = plyVar.Create(this._plyVarType);
							plyVar.name = i.ToString();
							plyVar.ValueHandler.SetStoredType(this._baseType);
							this.variables.Add(plyVar);
							this.arr.SetValue(wrapper.objRefs[i], i);
						}
					}
				}
				else
				{
					this.arr = Array.CreateInstance(this._baseType, this.storedEntriesDataData.Length);
					for (int j = 0; j < this.storedEntriesDataData.Length; j++)
					{
						if (!string.IsNullOrEmpty(this.storedEntriesDataData[j]))
						{
							plyVar plyVar2 = plyVar.Create(this._plyVarType);
							plyVar2.name = j.ToString();
							plyVar2.ValueHandler.DecodeValues(wrapper, this.storedEntriesDataData[j]);
							this.variables.Add(plyVar2);
							this.arr.SetValue(plyVar2.GetValue(), j);
						}
					}
				}
			}
		}

		public Array GetValue()
		{
			return this.arr;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
