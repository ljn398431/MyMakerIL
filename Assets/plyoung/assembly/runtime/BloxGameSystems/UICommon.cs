using BloxEngine;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[SuppressBaseMembersInBlox]
	[BloxBlockIcon("bgs")]
	public static class UICommon
	{
		private static Dictionary<string, GameObject> managedWindows = new Dictionary<string, GameObject>();

		/// <summary> This can be called to skip a screen set to `Wait Screen End Trigger`</summary>
		public static void TriggerSplashScreenEnd()
		{
			SplashScreensManager instance = SplashScreensManager.Instance;
			if ((object)instance != null)
			{
				instance.TriggerScreenEnd();
			}
		}

		[ExcludeFromBlox]
		public static void RegisterManagedWindows(List<GameObject> windows)
		{
			for (int i = 0; i < windows.Count; i++)
			{
				if (!((Object)windows[i] == (Object)null))
				{
					windows[i].SetActive(false);
					if (UICommon.managedWindows.ContainsKey(windows[i].name))
					{
						Debug.LogWarningFormat("The window name [{0}] is already used by an object added by a window manager.", windows[i].name);
					}
					else
					{
						UICommon.managedWindows.Add(windows[i].name, windows[i]);
					}
				}
			}
		}

		[ExcludeFromBlox]
		public static void RemoveManagedWindows(List<GameObject> windows)
		{
			for (int i = 0; i < windows.Count; i++)
			{
				if (!((Object)windows[i] == (Object)null))
				{
					UICommon.managedWindows.Remove(windows[i].name);
				}
			}
		}

		/// <summary> Make the named window object visible/ active . This window must be added to the list of windows of the `WindowManager` component. </summary>
		public static void ShowWindow(string name)
		{
			GameObject gameObject = null;
			if (UICommon.managedWindows.TryGetValue(name, out gameObject))
			{
				gameObject.SetActive(true);
			}
			else
			{
				Debug.LogWarningFormat("[WindowManager] Could not find managed window named: {0}", name);
			}
		}

		/// <summary> Hide (deactivate) the named window object. This window must be added to the list of windows of the `WindowManager` component. </summary>
		public static void HideWindow(string name)
		{
			GameObject gameObject = null;
			if (UICommon.managedWindows.TryGetValue(name, out gameObject))
			{
				gameObject.SetActive(false);
			}
			else
			{
				Debug.LogWarningFormat("[WindowManager] Could not find managed window named: {0}", name);
			}
		}

		/// <summary> Returns True if the named window object is visible (active), else False. This window must be added to the list of windows of the `WindowManager` component. </summary>
		public static bool WindowIsVisible(string name)
		{
			GameObject gameObject = null;
			if (UICommon.managedWindows.TryGetValue(name, out gameObject))
			{
				return gameObject.activeInHierarchy;
			}
			Debug.LogWarningFormat("[WindowManager] Could not find managed window named: {0}", name);
			return false;
		}
	}
}
