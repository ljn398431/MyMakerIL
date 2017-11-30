using plyLibEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BloxEditor
{
	[InitializeOnLoad]
	public class BloxEdGUI
	{
		public class StyleDefs
		{
			public GUIStyle ToolbarBloxName;

			public GUIStyle EventListElement;

			public GUIStyle PropsPanel;

			public GUIStyle PropsHead;

			public GUIStyle ActionLabel;

			public GUIStyle ActionBoldLabel;

			public GUIStyle FieldLabel;

			public GUIStyle ValueLabel;

			public GUIStyle IconLabel;

			public GUIStyle[] Event = new GUIStyle[2];

			public GUIStyle[] Value = new GUIStyle[5];

			public GUIStyle[] Container = new GUIStyle[3];

			public GUIStyle[] Select = new GUIStyle[4];

			public GUIStyle CanvasBlock;

			public Dictionary<string, GUIStyle[]> Action = new Dictionary<string, GUIStyle[]>();

			public GUIStyle Error;

			public GUIStyle Horizontal;

			public GUIStyle DocParamBold;

			public GUIStyle DocParamNormal;

			public GUIStyle EventsPopupDocsHead;

			private int _fontSize = 13;

			public int FontSize
			{
				get
				{
					return this._fontSize;
				}
				set
				{
					if (this._fontSize != value)
					{
						this._fontSize = value;
						this._fontSize = Mathf.Clamp(this._fontSize, 1, 32);
						EditorPrefs.SetInt("Blox.BlocksFontSize", this._fontSize);
						this.UpdateStyles();
					}
				}
			}

			public StyleDefs()
			{
				plyEdGUI.UseLightText(GUI.skin.label.normal.textColor);
				this._fontSize = EditorPrefs.GetInt("Blox.BlocksFontSize", this._fontSize);
				this.ToolbarBloxName = new GUIStyle(plyEdGUI.Styles.GUIToolbarLabel)
				{
					fontSize = 11,
					fontStyle = FontStyle.Bold,
					padding = new RectOffset(0, 10, 1, 0)
				};
				this.EventListElement = new GUIStyle(plyEdGUI.Styles.ListElement)
				{
					font = null,
					fontSize = 13
				};
				this.PropsPanel = new GUIStyle(GUI.skin.FindStyle("WindowBackground"))
				{
					padding = new RectOffset(0, 0, 0, 0)
				};
				this.PropsHead = new GUIStyle(GUI.skin.label)
				{
					fontSize = 12,
					alignment = TextAnchor.MiddleLeft,
					padding = new RectOffset(6, 6, 0, 0)
				};
				this.PropsHead.normal.textColor = this.PropsHead.normal.textColor - new Color(0f, 0f, 0f, 0.3f);
				this.ActionLabel = new GUIStyle(GUI.skin.label)
				{
					richText = false,
					stretchWidth = false,
					stretchHeight = true,
					alignment = TextAnchor.MiddleLeft,
					fontSize = this._fontSize,
					fontStyle = FontStyle.Normal,
					clipping = TextClipping.Clip,
					wordWrap = false,
					imagePosition = ImagePosition.ImageLeft,
					border = new RectOffset(0, 0, 0, 0),
					overflow = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(5, 0, 0, 0),
					margin = new RectOffset(0, 0, 0, 0),
					contentOffset = new Vector2(0f, 0f),
					normal = 
					{
						textColor = BloxEdGUI.BlockFontColour()
					}
				};
				this.ActionBoldLabel = new GUIStyle(this.ActionLabel)
				{
					fontStyle = FontStyle.Bold
				};
				this.FieldLabel = new GUIStyle(this.ActionLabel)
				{
					padding = new RectOffset(5, 3, 0, 0)
				};
				this.ValueLabel = new GUIStyle(this.ActionLabel)
				{
					padding = new RectOffset(0, 0, 0, 0)
				};
				this.IconLabel = new GUIStyle(this.ActionLabel)
				{
					font = plyEdGUI.Styles.icoFont,
					fontSize = this._fontSize + 1
				};
				this.Event[0] = new GUIStyle
				{
					richText = false,
					stretchWidth = false,
					stretchHeight = false,
					alignment = TextAnchor.UpperLeft,
					fontSize = 14,
					fontStyle = FontStyle.Bold,
					clipping = TextClipping.Clip,
					wordWrap = false,
					imagePosition = ImagePosition.ImageLeft,
					border = new RectOffset(27, 3, 32, 11),
					overflow = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(5, 6, 29, 8),
					margin = new RectOffset(0, 0, 0, 0),
					contentOffset = new Vector2(10f, -21f),
					normal = 
					{
						textColor = BloxEdGUI.BlockFontColour(),
						background = BloxEdGUI.LoadBlockTexture("event0")
					},
					onNormal = 
					{
						textColor = BloxEdGUI.BlockFontColour(),
						background = BloxEdGUI.LoadBlockTexture("event0")
					}
				};
				this.Event[1] = new GUIStyle(this.Event[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("event1")
					},
					onNormal = 
					{
						background = BloxEdGUI.LoadBlockTexture("event1")
					}
				};
				this.Value[0] = new GUIStyle
				{
					richText = false,
					stretchWidth = false,
					stretchHeight = true,
					alignment = TextAnchor.MiddleLeft,
					fontSize = this._fontSize,
					fontStyle = FontStyle.Normal,
					clipping = TextClipping.Clip,
					wordWrap = false,
					imagePosition = ImagePosition.ImageLeft,
					border = new RectOffset(3, 2, 2, 2),
					overflow = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(4, 4, 3, 3),
					margin = new RectOffset(0, 0, 0, 0),
					contentOffset = new Vector2(0f, 0f),
					normal = 
					{
						textColor = BloxEdGUI.BlockFontColour(),
						background = BloxEdGUI.LoadBlockTexture("value0")
					}
				};
				this.Value[1] = new GUIStyle(this.Value[0])
				{
					padding = new RectOffset(2, 4, 3, 3),
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("value1")
					}
				};
				this.Value[2] = new GUIStyle(this.Value[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("value2")
					}
				};
				this.Value[3] = new GUIStyle(this.Value[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("value3")
					}
				};
				this.Value[4] = new GUIStyle(this.Value[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("value4")
					}
				};
				GUIStyle[] array = new GUIStyle[3];
				this.Action.Add("default", array);
				array[0] = new GUIStyle
				{
					richText = false,
					stretchWidth = false,
					stretchHeight = true,
					alignment = TextAnchor.MiddleLeft,
					fontSize = this._fontSize,
					fontStyle = FontStyle.Normal,
					clipping = TextClipping.Clip,
					wordWrap = false,
					imagePosition = ImagePosition.ImageLeft,
					border = new RectOffset(20, 5, 5, 5),
					overflow = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(6, 14, 7, 5),
					margin = new RectOffset(0, 0, -3, -3),
					contentOffset = new Vector2(0f, 0f),
					normal = 
					{
						textColor = BloxEdGUI.BlockFontColour(),
						background = BloxEdGUI.LoadBlockTexture("act_default.action0")
					}
				};
				array[1] = new GUIStyle(array[0])
				{
					padding = new RectOffset(6, 14, 7, 5),
					border = new RectOffset(20, 0, 5, 5),
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("act_default.action1")
					}
				};
				array[2] = new GUIStyle(array[0])
				{
					padding = new RectOffset(3, 6, 7, 4),
					border = new RectOffset(2, 3, 5, 5),
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("act_default.action2")
					}
				};
				this.Action.Add("grey", new GUIStyle[3]
				{
					new GUIStyle(array[0])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_grey.action0")
						}
					},
					new GUIStyle(array[1])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_grey.action1")
						}
					},
					new GUIStyle(array[2])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_grey.action2")
						}
					}
				});
				this.Action.Add("red", new GUIStyle[3]
				{
					new GUIStyle(array[0])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_red.action0")
						}
					},
					new GUIStyle(array[1])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_red.action1")
						}
					},
					new GUIStyle(array[2])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_red.action2")
						}
					}
				});
				this.Action.Add("orange", new GUIStyle[3]
				{
					new GUIStyle(array[0])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_orange.action0")
						}
					},
					new GUIStyle(array[1])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_orange.action1")
						}
					},
					new GUIStyle(array[2])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_orange.action2")
						}
					}
				});
				this.Action.Add("debug", new GUIStyle[3]
				{
					new GUIStyle(array[0])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_debug.action0")
						}
					},
					new GUIStyle(array[1])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_debug.action1")
						}
					},
					new GUIStyle(array[2])
					{
						normal = 
						{
							background = BloxEdGUI.LoadBlockTexture("act_debug.action2")
						}
					}
				});
				this.Container[0] = new GUIStyle
				{
					richText = false,
					stretchWidth = false,
					stretchHeight = false,
					alignment = TextAnchor.MiddleLeft,
					fontSize = this._fontSize,
					fontStyle = FontStyle.Normal,
					clipping = TextClipping.Clip,
					wordWrap = false,
					imagePosition = ImagePosition.ImageLeft,
					border = new RectOffset(24, 5, 5, 5),
					overflow = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(6, 14, 7, 5),
					margin = new RectOffset(0, 0, -3, -3),
					contentOffset = new Vector2(0f, 0f),
					normal = 
					{
						textColor = BloxEdGUI.BlockFontColour(),
						background = BloxEdGUI.LoadBlockTexture("container0")
					}
				};
				this.Container[1] = new GUIStyle(this.Container[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("container1")
					}
				};
				this.Container[2] = new GUIStyle(this.Container[0])
				{
					border = new RectOffset(27, 5, 3, 11),
					overflow = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(5, 0, 0, 8),
					margin = new RectOffset(0, 0, -3, -3),
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("container2")
					}
				};
				this.Select[0] = new GUIStyle(this.Value[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("select0")
					}
				};
				this.Select[1] = new GUIStyle(array[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("select1")
					}
				};
				this.Select[2] = new GUIStyle(this.Container[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("select2")
					}
				};
				this.Select[3] = new GUIStyle(this.Container[2])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("select3")
					}
				};
				this.Error = new GUIStyle(this.Value[0])
				{
					normal = 
					{
						background = BloxEdGUI.LoadBlockTexture("error0")
					}
				};
				this.Horizontal = new GUIStyle
				{
					stretchHeight = false,
					stretchWidth = false,
					richText = false,
					wordWrap = false,
					clipping = TextClipping.Clip,
					margin = new RectOffset(0, 0, -3, -3)
				};
				this.DocParamBold = new GUIStyle(GUI.skin.FindStyle("ProjectBrowserHeaderBgTop"))
				{
					font = GUI.skin.label.font,
					fontSize = GUI.skin.label.fontSize,
					fontStyle = FontStyle.Bold,
					alignment = TextAnchor.MiddleLeft,
					richText = false,
					wordWrap = true,
					margin = new RectOffset(0, 0, 1, 0),
					padding = new RectOffset(5, 5, 2, 2),
					stretchHeight = false,
					stretchWidth = true,
					fixedHeight = 0f,
					fixedWidth = 0f,
					normal = 
					{
						textColor = GUI.skin.label.normal.textColor
					}
				};
				this.DocParamNormal = new GUIStyle(this.DocParamBold)
				{
					fontStyle = FontStyle.Normal,
					richText = true,
					margin = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(5, 5, 5, 5)
				};
				this.EventsPopupDocsHead = new GUIStyle(plyEdGUI.Styles.TreeViewListButtons);
				this.CanvasBlock = new GUIStyle();
			}

			private void UpdateStyles()
			{
				this.ActionLabel.fontSize = this._fontSize;
				this.ActionBoldLabel.fontSize = this._fontSize;
				this.FieldLabel.fontSize = this._fontSize;
				this.ValueLabel.fontSize = this._fontSize;
				this.IconLabel.fontSize = this._fontSize + 1;
				this.Value[0].fontSize = this._fontSize;
				this.Value[1].fontSize = this._fontSize;
				this.Value[2].fontSize = this._fontSize;
				this.Value[3].fontSize = this._fontSize;
				this.Value[4].fontSize = this._fontSize;
				this.Container[0].fontSize = this._fontSize;
				this.Container[1].fontSize = this._fontSize;
				this.Container[2].fontSize = this._fontSize;
				this.Select[0].fontSize = this._fontSize;
				this.Select[1].fontSize = this._fontSize;
				this.Select[2].fontSize = this._fontSize;
				this.Select[3].fontSize = this._fontSize;
				this.Error.fontSize = this._fontSize;
				foreach (KeyValuePair<string, GUIStyle[]> item in this.Action)
				{
					for (int i = 0; i < item.Value.Length; i++)
					{
						item.Value[i].fontSize = this._fontSize;
					}
				}
			}
		}

		private static StyleDefs _styles;

		private static bool needRefreshedInit;

		private static BloxEdGUI _instance;

		public Texture2D folderIcon;

		public Texture2D unityIcon;

		public Texture2D bloxIcon;

		public Texture2D bloxFadedIcon;

		public Texture2D bloxGameSystems;

		public Dictionary<string, Texture2D> namedIcons = new Dictionary<string, Texture2D>();

		private const int buildInThemesCount = 4;

		private static string themesRoot;

		private static string themeName;

		private static Color[] _fontColours;

		private static GUIContent[] _blockThemeNames;

		public static StyleDefs Styles
		{
			get
			{
				return BloxEdGUI._styles ?? (BloxEdGUI._styles = new StyleDefs());
			}
		}

		public static BloxEdGUI Instance
		{
			get
			{
				if (BloxEdGUI._instance == null || BloxEdGUI.needRefreshedInit)
				{
					BloxEdGUI.Create();
				}
				return BloxEdGUI._instance;
			}
		}

		public static GUIContent[] BlockThemeNames
		{
			get
			{
				if (BloxEdGUI._blockThemeNames == null)
				{
					BloxEdGUI.LoadThemes();
				}
				return BloxEdGUI._blockThemeNames;
			}
		}

		private static BloxEdGUI Create()
		{
			if (BloxEdGUI._instance == null)
			{
				BloxEdGUI._instance = new BloxEdGUI();
			}
			BloxEdGUI.needRefreshedInit = false;
			string str = "";
			try
			{
				str = (plyEdGUI.UseLightText(GUI.skin.label.normal.textColor) ? "_p" : "");
			}
			catch
			{
				BloxEdGUI.needRefreshedInit = true;
			}
			BloxEdGUI._instance.folderIcon = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.folder" + str + ".png", typeof(BloxEdGUI).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			BloxEdGUI._instance.unityIcon = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.unity" + str + ".png", typeof(BloxEdGUI).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			BloxEdGUI._instance.bloxIcon = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox2" + str + ".png", typeof(BloxEdGUI).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			BloxEdGUI._instance.bloxFadedIcon = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.blox_faded" + str + ".png", typeof(BloxEdGUI).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			BloxEdGUI._instance.bloxGameSystems = plyEdGUI.LoadTextureResource("BloxEditor.res.icons.bgs" + str + ".png", typeof(BloxEdGUI).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			if (BloxEdGUI._instance.namedIcons.ContainsKey("folder"))
			{
				BloxEdGUI._instance.namedIcons["folder"] = BloxEdGUI._instance.folderIcon;
				BloxEdGUI._instance.namedIcons["blox"] = BloxEdGUI._instance.bloxIcon;
				BloxEdGUI._instance.namedIcons["unity"] = BloxEdGUI._instance.unityIcon;
				BloxEdGUI._instance.namedIcons["bgs"] = BloxEdGUI._instance.bloxGameSystems;
			}
			else
			{
				BloxEdGUI._instance.namedIcons.Add("folder", BloxEdGUI._instance.folderIcon);
				BloxEdGUI._instance.namedIcons.Add("blox", BloxEdGUI._instance.bloxIcon);
				BloxEdGUI._instance.namedIcons.Add("unity", BloxEdGUI._instance.unityIcon);
				BloxEdGUI._instance.namedIcons.Add("bgs", BloxEdGUI._instance.bloxGameSystems);
			}
			return BloxEdGUI._instance;
		}

		static BloxEdGUI()
		{
			BloxEdGUI.needRefreshedInit = true;
			BloxEdGUI.themesRoot = null;
			BloxEdGUI.themeName = null;
			BloxEdGUI._fontColours = null;
			BloxEdGUI._blockThemeNames = null;
			string @string = EditorPrefs.GetString("EditorTheme-Refresh");
			EditorPrefs.SetString("EditorTheme-Refresh", @string + "|BloxEditor.BloxEdGUI.OnEditorThemeChanged");
		}

		private static void OnEditorThemeChanged()
		{
			BloxEdGUI._styles = null;
			BloxEdGUI._instance = null;
		}

		private static void LoadThemes()
		{
			BloxEdGUI._fontColours = new Color[4]
			{
				Color.white,
				Color.black,
				Color.white,
				Color.white
			};
			BloxEdGUI._blockThemeNames = new GUIContent[4]
			{
				new GUIContent("Default"),
				new GUIContent("Blox1"),
				new GUIContent("Blox2"),
				new GUIContent("Mecanim")
			};
			BloxEdGUI.themesRoot = plyEdUtil.PackagesRelativePath + "Blox/editor/themes/";
			string text = plyEdUtil.PackagesFullPath + "Blox/editor/themes/";
			if (Directory.Exists(text))
			{
				string[] directories = Directory.GetDirectories(text);
				for (int i = 0; i < directories.Length; i++)
				{
					string text2 = directories[i].Replace(text, "");
					Color white = Color.white;
					try
					{
						bool flag = false;
						string[] array = File.ReadAllLines(directories[i] + "/text_colour.txt");
						for (int j = 0; j < array.Length; j++)
						{
							string text3 = array[j];
							if (text3.StartsWith("#") && ColorUtility.TryParseHtmlString(text3, out white))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Debug.LogWarning("Could not read the text colour from: " + directories[i] + "/text_colour.txt");
						}
					}
					catch (Exception)
					{
						Debug.LogWarning("Could not read the text colour from: " + directories[i] + "/text_colour.txt");
					}
					ArrayUtility.Add<Color>(ref BloxEdGUI._fontColours, white);
					ArrayUtility.Add<GUIContent>(ref BloxEdGUI._blockThemeNames, new GUIContent(text2));
				}
			}
		}

		public static void UpdateBlockTheme()
		{
			if (BloxEdGlobal.BlockTheme < 0 || BloxEdGlobal.BlockTheme >= BloxEdGUI.BlockThemeNames.Length)
			{
				BloxEdGlobal.BlockTheme = 0;
			}
			BloxEdGUI.themeName = BloxEdGUI.BlockThemeNames[BloxEdGlobal.BlockTheme].text;
			BloxEdGUI.Styles.ActionLabel.normal.textColor = BloxEdGUI.BlockFontColour();
			BloxEdGUI.Styles.ActionBoldLabel.normal.textColor = BloxEdGUI.BlockFontColour();
			BloxEdGUI.Styles.FieldLabel.normal.textColor = BloxEdGUI.BlockFontColour();
			BloxEdGUI.Styles.ValueLabel.normal.textColor = BloxEdGUI.BlockFontColour();
			BloxEdGUI.Styles.IconLabel.normal.textColor = BloxEdGUI.BlockFontColour();
			for (int i = 0; i < BloxEdGUI.Styles.Event.Length; i++)
			{
				BloxEdGUI.Styles.Event[i].normal.background = BloxEdGUI.LoadBlockTexture("event" + i);
				BloxEdGUI.Styles.Event[i].onNormal.background = BloxEdGUI.LoadBlockTexture("event" + i);
				BloxEdGUI.Styles.Event[i].normal.textColor = BloxEdGUI.BlockFontColour();
				BloxEdGUI.Styles.Event[i].onNormal.textColor = BloxEdGUI.BlockFontColour();
			}
			for (int j = 0; j < BloxEdGUI.Styles.Value.Length; j++)
			{
				BloxEdGUI.Styles.Value[j].normal.background = BloxEdGUI.LoadBlockTexture("value" + j);
				BloxEdGUI.Styles.Value[j].normal.textColor = BloxEdGUI.BlockFontColour();
			}
			foreach (KeyValuePair<string, GUIStyle[]> item in BloxEdGUI.Styles.Action)
			{
				for (int k = 0; k < item.Value.Length; k++)
				{
					item.Value[k].normal.background = BloxEdGUI.LoadBlockTexture("act_" + item.Key + ".action" + k);
					item.Value[k].normal.textColor = BloxEdGUI.BlockFontColour();
				}
			}
			for (int l = 0; l < BloxEdGUI.Styles.Container.Length; l++)
			{
				BloxEdGUI.Styles.Container[l].normal.background = BloxEdGUI.LoadBlockTexture("container" + l);
				BloxEdGUI.Styles.Container[l].normal.textColor = BloxEdGUI.BlockFontColour();
			}
			for (int m = 0; m < BloxEdGUI.Styles.Select.Length; m++)
			{
				BloxEdGUI.Styles.Select[m].normal.background = BloxEdGUI.LoadBlockTexture("select" + m);
				BloxEdGUI.Styles.Select[m].normal.textColor = BloxEdGUI.BlockFontColour();
			}
			BloxEdGUI.Styles.Error.normal.background = BloxEdGUI.LoadBlockTexture("error0");
			BloxEditorWindow instance = BloxEditorWindow.Instance;
			if ((object)instance != null)
			{
				instance.Repaint();
			}
		}

		private static Texture2D LoadBlockTexture(string name)
		{
			if (BloxEdGUI.themeName == null)
			{
				if (BloxEdGlobal.BlockTheme < 0 || BloxEdGlobal.BlockTheme >= BloxEdGUI.BlockThemeNames.Length)
				{
					BloxEdGlobal.BlockTheme = 0;
				}
				BloxEdGUI.themeName = BloxEdGUI.BlockThemeNames[BloxEdGlobal.BlockTheme].text;
			}
			if (BloxEdGlobal.BlockTheme < 4)
			{
				return plyEdGUI.LoadTextureResource("BloxEditor.res.skin." + BloxEdGUI.themeName + "." + name + ".png", typeof(BloxEditorWindow).Assembly, FilterMode.Point, TextureWrapMode.Clamp);
			}
			return plyEdGUI.LoadTexture(BloxEdGUI.themesRoot + BloxEdGUI.themeName + "/" + name.Replace(".", "/") + ".png");
		}

		private static Color BlockFontColour()
		{
			if (BloxEdGUI._fontColours == null)
			{
				BloxEdGUI.LoadThemes();
			}
			if (BloxEdGlobal.BlockTheme < 0 || BloxEdGlobal.BlockTheme >= BloxEdGUI.BlockThemeNames.Length)
			{
				BloxEdGlobal.BlockTheme = 0;
			}
			return BloxEdGUI._fontColours[BloxEdGlobal.BlockTheme];
		}
	}
}
