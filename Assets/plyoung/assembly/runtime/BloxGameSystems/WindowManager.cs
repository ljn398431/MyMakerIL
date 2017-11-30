using BloxEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[DisallowMultipleComponent]
	[AddComponentMenu("Blox/GUI/Window Manager")]
	[HelpURL("https://plyoung.github.io/blox-window-manager.html")]
	public class WindowManager : MonoBehaviour
	{
		[Serializable]
		[ExcludeFromBlox]
		public class WindowEvent : UnityEvent<string>
		{
		}

		public List<GameObject> windows = new List<GameObject>();

		public WindowEvent onWindowShown;

		public WindowEvent onWindowHidden;

		protected void Awake()
		{
			UICommon.RegisterManagedWindows(this.windows);
		}

		protected void OnDestroy()
		{
			UICommon.RemoveManagedWindows(this.windows);
		}

		/// <summary> Make the named window object visible/ active . This window must be added to the list of windows of the `WindowManager` component. </summary>
		public void ShowWindow(string name)
		{
			UICommon.ShowWindow(name);
			this.onWindowShown.Invoke(name);
			base.gameObject.SendMessage("bgsOnWindowShown", name, SendMessageOptions.DontRequireReceiver);
		}

		/// <summary> Hide (deactivate) the named window object. This window must be added to the list of windows of the `WindowManager` component. </summary>
		public void HideWindow(string name)
		{
			UICommon.HideWindow(name);
			this.onWindowHidden.Invoke(name);
			base.gameObject.SendMessage("bgsonWindowHidden", name, SendMessageOptions.DontRequireReceiver);
		}

		/// <summary> Returns True if the named window object is visible (active), else False. This window must be added to the list of windows of the `WindowManager` component. </summary>
		public bool WindowIsVisible(string name)
		{
			return UICommon.WindowIsVisible(name);
		}
	}
}
