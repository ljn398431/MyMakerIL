using UnityEngine;

namespace BloxEngine.Variables
{
	[ExcludeFromBlox]
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	public class GlobalVariables : plyVariablesBehaviour
	{
		public static GlobalVariables Instance
		{
			get;
			private set;
		}

		protected override void Awake()
		{
			if ((Object)GlobalVariables.Instance == (Object)null)
			{
				base.Awake();
				GlobalVariables.Instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				Debug.LogError("There should be only one instance of the Global Variables component but another one was found. This Global Variables component will not be removed but the Global Variables Instance will not point to it.", base.gameObject);
				base.enabled = false;
			}
		}
	}
}
