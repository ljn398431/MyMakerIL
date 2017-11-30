using BloxEngine;
using plyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloxGameSystems
{
	/// <summary> Game Global is a helper which provides access to global and commonly used properties. Most of these will also have Blocks which you will find under `GameSystems &gt; GameGlobal` and can also be accessed by the Data Binding system. </summary>
	[SuppressBaseMembersInBlox]
	[BloxBlockIcon("bgs")]
	[AddComponentMenu("")]
	[HelpURL("https://plyoung.github.io/tbrpg-components.html")]
	public class GameGlobal : SingletonMonoBehaviour<GameGlobal>
	{
		/// <summary> Set this boolean to True to "pause" the game. Game systems with Player and NPC control are expected to make use of this flag but it is still up to individual systems to decide whether to honour this flag or not. The default state is False.</summary>
		public static bool GamePaused;

		private List<AsyncOperation> loadingScenes = new List<AsyncOperation>();

		private List<AsyncOperation> unloadingScenes = new List<AsyncOperation>();

		/// <summary> Returns True while there are scene loading </summary>
		public static bool ScenesAreLoading
		{
			get;
			private set;
		}

		protected void LateUpdate()
		{
			if (this.loadingScenes.Count > 0)
			{
				for (int num = this.loadingScenes.Count - 1; num >= 0; num--)
				{
					if (this.loadingScenes[num].isDone)
					{
						this.loadingScenes.RemoveAt(num);
					}
				}
				if (this.loadingScenes.Count == 0)
				{
					GameGlobal.ScenesAreLoading = false;
				}
			}
			if (this.unloadingScenes.Count > 0)
			{
				for (int num2 = this.unloadingScenes.Count - 1; num2 >= 0; num2--)
				{
					if (this.unloadingScenes[num2].isDone)
					{
						this.unloadingScenes.RemoveAt(num2);
					}
				}
				if (this.unloadingScenes.Count == 0)
				{
					Resources.UnloadUnusedAssets();
				}
			}
		}

		public static void LoadScene(string sceneName)
		{
			GameGlobal.ScenesAreLoading = true;
			AsyncOperation item = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
			SingletonMonoBehaviour<GameGlobal>.Instance.loadingScenes.Add(item);
		}

		public static void UnloadScene(string sceneName)
		{
			AsyncOperation item = SceneManager.UnloadSceneAsync(sceneName);
			SingletonMonoBehaviour<GameGlobal>.Instance.unloadingScenes.Add(item);
		}

		public static void UnloadScene(int idx)
		{
			AsyncOperation item = SceneManager.UnloadSceneAsync(idx);
			SingletonMonoBehaviour<GameGlobal>.Instance.unloadingScenes.Add(item);
		}
	}
}
