using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/Helpers/Simple Follower")]
	[HelpURL("https://plyoung.github.io/blox-components.html")]
	public class SimpleFollower : MonoBehaviour
	{
		public Transform target;

		private Transform tr;

		protected void Awake()
		{
			this.tr = base.transform;
		}

		protected void LateUpdate()
		{
			if ((Object)this.target != (Object)null)
			{
				this.tr.position = this.target.position;
			}
		}
	}
}
