using BloxEngine;
using System.Collections.Generic;
using UnityEngine;

namespace BloxGameSystems
{
	[BloxBlockIcon("bgs")]
	[SuppressBaseMembersInBlox]
	public static class SettingsManager
	{
		private static float[] soundVolumes = new float[10]
		{
			1f,
			1f,
			1f,
			1f,
			1f,
			1f,
			1f,
			1f,
			1f,
			1f
		};

		private static List<SoundVolumeUpdater>[] soundVolumeUpdaters = new List<SoundVolumeUpdater>[10]
		{
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>(),
			new List<SoundVolumeUpdater>()
		};

		/// <summary> Get or Set whether the game is in 
		/// fullscreen mode or not. </summary>
		public static bool Fullscreen
		{
			get
			{
				return Screen.fullScreen;
			}
			set
			{
				Screen.fullScreen = value;
			}
		}

		/// <summary> Get or Set the screen resolution index. This is an integer value representing a resolution from
		/// the list of supported resolutions.
		///
		/// The list of resolutions can be retrieved via ScreenResolutions. 
		/// It will return -1 if the resolution index could not be determined</summary>
		public static int ScreenResolutionIndex
		{
			get
			{
				int result = -1;
				int width;
				Resolution currentResolution;
				if (!Screen.fullScreen)
				{
					width = Screen.width;
				}
				else
				{
					currentResolution = Screen.currentResolution;
					width = currentResolution.width;
				}
				int num = width;
				int height;
				if (!Screen.fullScreen)
				{
					height = Screen.height;
				}
				else
				{
					currentResolution = Screen.currentResolution;
					height = currentResolution.height;
				}
				int num2 = height;
				int num3 = 0;
				while (num3 < Screen.resolutions.Length)
				{
					if (num != Screen.resolutions[num3].width || num2 != Screen.resolutions[num3].height)
					{
						num3++;
						continue;
					}
					result = num3;
					break;
				}
				return result;
			}
			set
			{
				if (value >= 0 && value < Screen.resolutions.Length)
				{
					Screen.SetResolution(Screen.resolutions[value].width, Screen.resolutions[value].height, SettingsManager.Fullscreen);
				}
			}
		}

		/// <summary> An array of quality level names as set up in Quality Settings editor; menu: Edit &gt; Project Settings &gt; Quality </summary>
		public static string[] GFXQualityLevels
		{
			get
			{
				return QualitySettings.names;
			}
		}

		/// <summary> Get or Set the quality level to use by the index into the list of defined quality levels. The 1st defined level's index will be 0, the 2nd will be 1, the 3rd will be 2, and so on.
		/// These quality levels are created in the Quality Settings editor; menu: Edit &gt; Project Settings &gt; Quality </summary>
		public static int GFXQualityLevelIndex
		{
			get
			{
				return QualitySettings.GetQualityLevel();
			}
			set
			{
				QualitySettings.SetQualityLevel(value);
			}
		}

		/// <summary> Set or Get the Main sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Main
		{
			get
			{
				return AudioListener.volume;
			}
			set
			{
				AudioListener.volume = Mathf.Clamp01(value);
			}
		}

		/// <summary> Set or Get the Music sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Music
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Music);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Music, value);
			}
		}

		/// <summary> Set or Get the Ambient sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Ambient
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Ambient);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Ambient, value);
			}
		}

		/// <summary> Set or Get the Effects sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Effects
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Effects);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Effects, value);
			}
		}

		/// <summary> Set or Get the Voice sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Voice
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Voice);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Voice, value);
			}
		}

		/// <summary> Set or Get the GUI sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_GUI
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.GUI);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.GUI, value);
			}
		}

		/// <summary> Set or Get the Other1 sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Other1
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Other1);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Other1, value);
			}
		}

		/// <summary> Set or Get the Other2 sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Other2
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Other2);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Other2, value);
			}
		}

		/// <summary> Set or Get the Other3 sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Other3
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Other3);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Other3, value);
			}
		}

		/// <summary> Set or Get the Other4 sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Other4
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Other4);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Other4, value);
			}
		}

		/// <summary> Set or Get the Other5 sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float SoundVolume_Other5
		{
			get
			{
				return SettingsManager.GetSoundVolume(SoundVolumeType.Other5);
			}
			set
			{
				SettingsManager.SetSoundVolume(SoundVolumeType.Other5, value);
			}
		}

		/// <summary> A list of strings representing the available screen resolutions.
		/// It is in the format "WxH", ex "1920x1080".
		/// Note that this will return a test entry of "640x480"
		/// when running in the Unity Editor since Unity does 
		/// not allow changing of resolution when running
		/// the inside the editor.</summary>
		public static List<string> ScreenResolutions()
		{
			List<string> list = new List<string>(Screen.resolutions.Length);
			for (int i = 0; i < Screen.resolutions.Length; i++)
			{
				list.Add(Screen.resolutions[i].width + "x" + Screen.resolutions[i].height);
			}
			return list;
		}

		[ExcludeFromBlox]
		public static void RegisterVolumeUpdater(SoundVolumeType type, SoundVolumeUpdater target)
		{
			if (!SettingsManager.soundVolumeUpdaters[(int)type].Contains(target))
			{
				SettingsManager.soundVolumeUpdaters[(int)type].Add(target);
				target.UpdateVolume(SettingsManager.soundVolumes[(int)type]);
			}
		}

		[ExcludeFromBlox]
		public static void RemoveVolumeUpdater(SoundVolumeType type, SoundVolumeUpdater target)
		{
			SettingsManager.soundVolumeUpdaters[(int)type].Remove(target);
		}

		/// <summary> Set main sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static void SetMainSoundVolume(float value)
		{
			AudioListener.volume = Mathf.Clamp01(value);
		}

		/// <summary> Get main sound volume. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float GetMainSoundVolume()
		{
			return AudioListener.volume;
		}

		/// <summary> Set sound volume of specified sound type. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static void SetSoundVolume(SoundVolumeType type, float value)
		{
			SettingsManager.soundVolumes[(int)type] = Mathf.Clamp01(value);
			for (int i = 0; i < SettingsManager.soundVolumeUpdaters[(int)type].Count; i++)
			{
				SettingsManager.soundVolumeUpdaters[(int)type][i].UpdateVolume(SettingsManager.soundVolumes[(int)type]);
			}
		}

		/// <summary> Get sound volume of specified sound type. The value is a float value between 0 (no sound) and 1 (full). So (0.5) is half the sound volume.</summary>
		public static float GetSoundVolume(SoundVolumeType type)
		{
			return SettingsManager.soundVolumes[(int)type];
		}
	}
}
