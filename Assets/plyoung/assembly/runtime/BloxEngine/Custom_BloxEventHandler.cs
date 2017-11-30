using UnityEngine;

namespace BloxEngine
{
	[AddComponentMenu("")]
	public class Custom_BloxEventHandler : BloxEventHandler
	{
		[BloxEvent("Custom", Order = 0, IconName = "blox", YieldAllowed = true)]
		public void Custom()
		{
		}
	}
}
