using BloxEngine;
using BloxEngine.Databinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace BloxGameSystems
{
	[ExcludeFromBlox(ExceptForSpecifiedMembers = true)]
	[BloxBlockIcon("bgs")]
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	[HelpURL("https://plyoung.github.io/blox-scenes.html")]
	public class Bootstrap : MonoBehaviour
	{
		[HideInInspector]
		public GameObject bloxGlobalPrefab;

		[HideInInspector]
		public List<int> startupScenes = new List<int>();

		[HideInInspector]
		public List<int> autoloadScenes = new List<int>();

		public UnityEvent onBootstrapDone;

		[IncludeInBlox]
		public static Property<bool> IsDone = new Property<bool>(false, false);

		public static bool StartedViaUnityPlayButton = false;

		private List<AsyncOperation> loadingScenes = new List<AsyncOperation>();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void RunOnGameStart()
		{
			if (Application.isEditor && (Object)Object.FindObjectOfType<Bootstrap>() == (Object)null && !((Object)BGSSettings.Instance == (Object)null) && BGSSettings.Instance.autoLoadBootstrapOnUnityPlay)
			{
				Bootstrap.StartedViaUnityPlayButton = true;
				SceneManager.LoadScene("00-bootstrap", LoadSceneMode.Additive);
			}
		}

		protected void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			base.gameObject.name = "Bootstrap";
			BloxGlobal.Create(this.bloxGlobalPrefab);
			PropertiesManager.OnBootstrapAwake();
			if (!Bootstrap.StartedViaUnityPlayButton)
			{
				if (this.startupScenes.Count > 0 && this.startupScenes[0] > 0 && this.startupScenes[0] < SceneManager.sceneCountInBuildSettings)
				{
					SceneManager.LoadScene(this.startupScenes[0], LoadSceneMode.Single);
				}
				else if (SceneManager.sceneCountInBuildSettings > 1)
				{
					SceneManager.LoadScene(1, LoadSceneMode.Single);
				}
				else
				{
					Debug.LogError("There are no scenes for the Bootstrap to load. You need to add at least one game scene to the Build Settings List or via the Blox Game Systems Window.");
				}
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					list.Add(SceneManager.GetSceneAt(i).buildIndex);
				}
				for (int j = 0; j < this.autoloadScenes.Count; j++)
				{
					int num = this.autoloadScenes[j];
					if (!list.Contains(num) && num > 0 && num < SceneManager.sceneCountInBuildSettings)
					{
						list.Add(num);
						SceneManager.LoadScene(num, LoadSceneMode.Additive);
					}
				}
			}
		}

		protected void Start()
		{
			if (!Bootstrap.StartedViaUnityPlayButton)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					list.Add(SceneManager.GetSceneAt(i).buildIndex);
				}
				if (this.startupScenes.Count > 1)
				{
					for (int j = 1; j < this.startupScenes.Count; j++)
					{
						int num = this.startupScenes[j];
						if (!list.Contains(num) && num > 0 && num < SceneManager.sceneCountInBuildSettings)
						{
							list.Add(num);
							AsyncOperation item = SceneManager.LoadSceneAsync(num, LoadSceneMode.Additive);
							this.loadingScenes.Add(item);
						}
					}
				}
				for (int k = 0; k < this.autoloadScenes.Count; k++)
				{
					int num2 = this.autoloadScenes[k];
					if (!list.Contains(num2) && num2 > 0 && num2 < SceneManager.sceneCountInBuildSettings)
					{
						list.Add(num2);
						AsyncOperation item2 = SceneManager.LoadSceneAsync(num2, LoadSceneMode.Additive);
						this.loadingScenes.Add(item2);
					}
				}
			}
		}

		protected void LateUpdate()
		{
			if (this.loadingScenes.Count > 0)
			{
				bool flag = true;
				int num = 0;
				while (num < this.loadingScenes.Count)
				{
					if (this.loadingScenes[num].isDone)
					{
						num++;
						continue;
					}
					flag = false;
					break;
				}
				if (flag)
				{
					this.DoneLoading();
				}
			}
			else if (SplashScreen.isFinished)
			{
				this.DoneLoading();
			}
		}

		private void DoneLoading()
		{
			Bootstrap.IsDone.Value = true;
			if (this.onBootstrapDone != null)
			{
				this.onBootstrapDone.Invoke();
			}
			base.gameObject.SendMessage("OnBootstrapDone", SendMessageOptions.DontRequireReceiver);
			Object.Destroy(base.gameObject);
			if (SceneManager.GetSceneByName("00-bootstrap").isLoaded)
			{
				GameGlobal.UnloadScene("00-bootstrap");
			}
		}
	}
}
