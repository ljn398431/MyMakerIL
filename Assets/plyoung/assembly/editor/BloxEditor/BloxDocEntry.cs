using System;
using UnityEngine;

namespace BloxEditor
{
	[Serializable]
	public class BloxDocEntry
	{
		[Serializable]
		public class Parameter
		{
			public string name;

			public string type;

			public string doc;
		}

		[SerializeField]
		public string ident;

		[SerializeField]
		public string url;

		[SerializeField]
		public string doc;

		[SerializeField]
		public Parameter[] parameters = new Parameter[0];

		[NonSerialized]
		public bool inited;

		[NonSerialized]
		public bool getonly = true;

		[NonSerialized]
		public string context;

		[NonSerialized]
		public string returns;

		private int _identHash;

		public int identHash
		{
			get
			{
				if (this._identHash == 0 && this.ident != null)
				{
					this._identHash = this.ident.GetHashCode();
				}
				return this._identHash;
			}
		}

		public override int GetHashCode()
		{
			return this.identHash;
		}
	}
}
