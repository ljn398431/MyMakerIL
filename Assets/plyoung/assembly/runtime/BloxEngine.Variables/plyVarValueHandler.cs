using System;
using UnityEngine;

namespace BloxEngine.Variables
{
	[Serializable]
	[ExcludeFromBlox]
	public class plyVarValueHandler
	{
		public virtual Type variableType
		{
			get
			{
				return typeof(object);
			}
		}

		public virtual void ReleaseStoredData()
		{
		}

		public virtual void SetStoredType(Type t)
		{
		}

		public virtual object GetValue(plyVar wrapper)
		{
			return null;
		}

		public virtual void SetValue(plyVar wrapper, object v)
		{
		}

		public virtual void ClearValue(plyVar wrapper)
		{
		}

		public virtual string EncodeValues(plyVar wrapper)
		{
			return JsonUtility.ToJson(this);
		}

		public virtual void DecodeValues(plyVar wrapper, string data)
		{
			JsonUtility.FromJsonOverwrite(data, this);
		}
	}
}
