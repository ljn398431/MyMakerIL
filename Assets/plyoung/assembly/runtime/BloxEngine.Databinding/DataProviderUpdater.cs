using plyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BloxEngine.Databinding
{
	[ExcludeFromBlox]
	[AddComponentMenu("")]
	public class DataProviderUpdater : SingletonMonoBehaviour<DataProviderUpdater>
	{
		private List<DataBinding> databinds = new List<DataBinding>();

		public void RegisterProvider(DataBinding db)
		{
			if (!this.databinds.Contains(db))
			{
				this.databinds.Add(db);
			}
		}

		public void RemoveProvider(DataBinding db)
		{
			if (this.databinds.Contains(db))
			{
				this.databinds.Remove(db);
			}
		}

		protected void LateUpdate()
		{
			for (int num = this.databinds.Count - 1; num >= 0; num--)
			{
				if (this.databinds[num] == null)
				{
					this.databinds.RemoveAt(num);
				}
				else
				{
					this.databinds[num].DoUpdate();
				}
			}
		}
	}
}
