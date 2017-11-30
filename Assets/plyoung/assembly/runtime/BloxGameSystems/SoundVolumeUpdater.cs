using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[AddComponentMenu("Blox/Helpers/Sound Volume Updater")]
	[HelpURL("https://plyoung.github.io/blox-components.html")]
	public class SoundVolumeUpdater : MonoBehaviour
	{
		[Tooltip("The AudioSource for which this updater will control the volume")]
		public AudioSource target;

		[Tooltip("The sound volume type this updater should bind to. It will set the target AudioSource to whatever changes takes place on the specified volume type only.")]
		public SoundVolumeType volumeType = SoundVolumeType.GUI;

		protected void Reset()
		{
			this.target = base.GetComponent<AudioSource>();
		}

		protected void Awake()
		{
			if ((Object)this.target == (Object)null)
			{
				this.target = base.GetComponent<AudioSource>();
				if ((Object)this.target == (Object)null)
				{
					Debug.LogError("[SoundVolumeUpdater] Could not find any AudioSource on the object.", base.gameObject);
					return;
				}
			}
			SettingsManager.RegisterVolumeUpdater(this.volumeType, this);
		}

		private void OnDestroy()
		{
			SettingsManager.RemoveVolumeUpdater(this.volumeType, this);
		}

		public void UpdateVolume(float v)
		{
			if (!((Object)this.target == (Object)null))
			{
				this.target.volume = v;
			}
		}
	}
}
