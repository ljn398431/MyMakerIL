using System;

namespace BloxEditor
{
	public class BloxEdDef : IComparable<BloxEdDef>
	{
		public string ident;

		public string name;

		public int order;

		public BloxDocEntry bloxdoc;

		private int _identHash;

		public int identHash
		{
			get
			{
				if (this._identHash == 0)
				{
					this._identHash = this.ident.GetHashCode();
				}
				return this._identHash;
			}
		}

		public int CompareTo(BloxEdDef obj)
		{
			if (this.order != obj.order)
			{
				return this.order.CompareTo(obj.order);
			}
			return this.ident.CompareTo(obj.ident);
		}
	}
}
