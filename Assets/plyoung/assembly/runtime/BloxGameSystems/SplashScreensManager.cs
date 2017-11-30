using BloxEngine;
using BloxEngine.Databinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[DisallowMultipleComponent]
	[AddComponentMenu("Blox/GUI/Splash Screens Manager")]
	[HelpURL("https://plyoung.github.io/blox-splash-screens-manager.html")]
	public class SplashScreensManager : MonoBehaviour
	{
		[Serializable]
		[ExcludeFromBlox]
		public class SplashScreen
		{
			public enum WaitType
			{
				Timeout = 0,
				WatchVariable = 1,
				WaitScreenEndTrigger = 2
			}

			public GameObject target;

			public WaitType waitType;

			public float timeout = 3f;

			public bool playerCanSkip = true;

			public ComparisonDataProvider watchVariable;
		}

		[Serializable]
		[ExcludeFromBlox]
		public class SplashScreenEvent : UnityEvent<int>
		{
		}

		public List<SplashScreen> screens = new List<SplashScreen>();

		public SplashScreenEvent onSpashScreenShown;

		public SplashScreenEvent onSpashScreenHidden;

		public UnityEvent onSpashScreensDone;

		public AutoUnloadOption autoUnloadWhenDone;

		public float minShowTime;

		private int idx = -1;

		private float timer;

		private float showTimer;

		public static SplashScreensManager Instance
		{
			get;
			private set;
		}

		protected void Awake()
		{
			SplashScreensManager.Instance = this;
			this.showTimer = this.minShowTime;
			for (int i = 0; i < this.screens.Count; i++)
			{
				if ((UnityEngine.Object)this.screens[i].target != (UnityEngine.Object)null)
				{
					this.screens[i].target.SetActive(false);
				}
			}
		}

		protected void Start()
		{
			for (int i = 0; i < this.screens.Count; i++)
			{
				if ((UnityEngine.Object)this.screens[i].watchVariable != (UnityEngine.Object)null)
				{
					this.screens[i].watchVariable.Initialize(this);
				}
			}
			this.ShowNextScreen();
		}

		protected void OnDestroy()
		{
			SplashScreensManager.Instance = null;
		}

		private void ShowNextScreen()
		{
			int num = this.idx;
			bool flag = false;
			while (num < this.screens.Count - 1)
			{
				num++;
				if ((UnityEngine.Object)this.screens[num].target != (UnityEngine.Object)null)
				{
					flag = true;
					break;
				}
			}
			if (!flag && this.showTimer > 0.0)
				return;
			if (this.idx >= 0)
			{
				this.screens[this.idx].target.SetActive(false);
				this.onSpashScreenHidden.Invoke(this.idx);
				base.gameObject.SendMessage("bgsOnSpashScreenHidden", this.idx, SendMessageOptions.DontRequireReceiver);
			}
			if (flag)
			{
				this.idx = num;
				this.timer = this.screens[this.idx].timeout;
				this.screens[this.idx].target.SetActive(true);
				this.onSpashScreenShown.Invoke(this.idx);
				base.gameObject.SendMessage("bgsOnSpashScreenShown", this.idx, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				base.enabled = false;
				this.idx = -1;
				this.onSpashScreensDone.Invoke();
				base.gameObject.SendMessage("bgsOnSpashScreensDone", SendMessageOptions.DontRequireReceiver);
				if (this.autoUnloadWhenDone == AutoUnloadOption.GameObject)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (this.autoUnloadWhenDone == AutoUnloadOption.Scene)
				{
					GameGlobal.UnloadScene(base.gameObject.scene.buildIndex);
				}
			}
		}

		protected void LateUpdate()
		{
			this.showTimer -= Time.deltaTime;
			if (this.idx >= 0)
			{
				if (this.screens[this.idx].playerCanSkip && Input.anyKeyDown)
				{
					this.ShowNextScreen();
				}
				else if (this.screens[this.idx].waitType == SplashScreen.WaitType.Timeout)
				{
					this.timer -= Time.deltaTime;
					if (this.timer <= 0.0)
					{
						this.ShowNextScreen();
					}
				}
				else if (this.screens[this.idx].waitType == SplashScreen.WaitType.WatchVariable && this.screens[this.idx].watchVariable.Value)
				{
					this.ShowNextScreen();
				}
			}
		}

		/// <summary> This can be called to skip a screen set to `Wait Screen End Trigger`</summary>
		public void TriggerScreenEnd()
		{
			if ((UnityEngine.Object)SplashScreensManager.Instance != (UnityEngine.Object)null)
			{
				if (SplashScreensManager.Instance.idx >= 0 && SplashScreensManager.Instance.screens[SplashScreensManager.Instance.idx].waitType == SplashScreen.WaitType.WaitScreenEndTrigger)
				{
					SplashScreensManager.Instance.ShowNextScreen();
				}
				else
				{
					Debug.LogWarning("The SplashScreensManager could not end a screen since there is no screen waiting for a ScreenEndTrigger.");
				}
			}
			else
			{
				Debug.LogError("The SplashScreensManager was not active when TriggerScreenEnd was called.");
			}
		}
	}
}
