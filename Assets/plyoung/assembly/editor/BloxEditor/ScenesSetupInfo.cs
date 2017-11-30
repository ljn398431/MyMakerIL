using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BloxEditor
{
	[Serializable]
	public class ScenesSetupInfo
	{
		[Serializable]
		public class SceneInfo
		{
			public bool isActive;

			public bool isLoaded;

			public string path;
		}

		[SerializeField]
		private SceneInfo[] data = new SceneInfo[0];

		public SceneSetup[] sceneSetup
		{
			get
			{
				SceneSetup[] array = new SceneSetup[this.data.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new SceneSetup
					{
						isActive = this.data[i].isActive,
						isLoaded = this.data[i].isLoaded,
						path = this.data[i].path
					};
				}
				return array;
			}
			set
			{
				List<SceneInfo> list = new List<SceneInfo>();
				bool flag = false;
				for (int i = 0; i < value.Length; i++)
				{
					if (!string.IsNullOrEmpty(value[i].path))
					{
						if (value[i].isActive)
						{
							flag = true;
						}
						list.Add(new SceneInfo
						{
							isActive = value[i].isActive,
							isLoaded = value[i].isLoaded,
							path = value[i].path
						});
					}
				}
				if (!flag && list.Count > 0)
				{
					list[0].isActive = true;
				}
				this.data = list.ToArray();
			}
		}
	}
}
