using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	public class plyVar_List : plyVarValueHandler
	{
		[SerializeField]
		private string plyVarTypeName;

		[SerializeField]
		private string baseTypeName;

		[SerializeField]
		private string[] storedEntriesData;

		[NonSerialized]
		public List<plyVar> variables = new List<plyVar>();

		[NonSerialized]
		private Type _plyVarType;

		[NonSerialized]
		private Type _baseType;

		[NonSerialized]
		private Type _genericType;

		[NonSerialized]
		private IList list;

		public override Type variableType
		{
			get
			{
				return this._genericType;
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
			this._genericType = typeof(List<>).MakeGenericType(baseType);
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
			return this.list;
		}

		public override void SetValue(plyVar wrapper, object v)
		{
			if (v == null)
			{
				throw new Exception("Can't convert null to List.");
			}
			IList list = v as IList;
			if (list == null)
			{
				throw new Exception("Can't convert " + v.GetType().Name + " to List.");
			}
			this.list = list;
		}

		public override void ClearValue(plyVar wrapper)
		{
			this.storedEntriesData = null;
			IList obj = this.list;
			if (obj != null)
			{
				obj.Clear();
			}
		}

		public override string EncodeValues(plyVar wrapper)
		{
			if (this._baseType != null && this._plyVarType != null)
			{
				this.baseTypeName = this._baseType.AssemblyQualifiedName;
				this.plyVarTypeName = this._plyVarType.AssemblyQualifiedName;
				if (typeof(UnityEngine.Object).IsAssignableFrom(this._baseType))
				{
					this.storedEntriesData = new string[0];
				}
				else
				{
					this.storedEntriesData = new string[this.variables.Count];
					for (int i = 0; i < this.variables.Count; i++)
					{
						this.storedEntriesData[i] = this.variables[i].ValueHandler.EncodeValues(wrapper);
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
				Debug.LogErrorFormat("Could not find the List's generic type [{0}] or plyVar type [{1}]", this.baseTypeName, this.plyVarTypeName);
				this._genericType = null;
			}
			else
			{
				this._genericType = typeof(List<>).MakeGenericType(this._baseType);
				this.variables = new List<plyVar>();
				this.list = (IList)Activator.CreateInstance(this._genericType);
				if (this.storedEntriesData != null)
				{
					if (typeof(UnityEngine.Object).IsAssignableFrom(this._baseType))
					{
						if (((wrapper.objRefs != null) ? wrapper.objRefs.Length : 0) != 0)
						{
							for (int i = 0; i < wrapper.objRefs.Length; i++)
							{
								plyVar plyVar = plyVar.Create(this._plyVarType);
								plyVar.name = i.ToString();
								plyVar.ValueHandler.SetStoredType(this._baseType);
								this.variables.Add(plyVar);
								this.list.Add(wrapper.objRefs[i]);
							}
						}
					}
					else
					{
						for (int j = 0; j < this.storedEntriesData.Length; j++)
						{
							plyVar plyVar2 = plyVar.Create(this._plyVarType);
							plyVar2.name = j.ToString();
							plyVar2.ValueHandler.DecodeValues(wrapper, this.storedEntriesData[j]);
							this.variables.Add(plyVar2);
							this.list.Add(plyVar2.GetValue());
						}
					}
				}
			}
		}

		public IList GetValue()
		{
			return this.list;
		}

		public void SetValue(object value)
		{
			this.SetValue(null, value);
		}
	}
}
