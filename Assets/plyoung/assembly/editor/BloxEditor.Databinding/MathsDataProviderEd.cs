using BloxEngine;
using BloxEngine.Databinding;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	[plyCustomEd(typeof(MathsDataProvider), Name = "Maths Operation")]
	public class MathsDataProviderEd : DataProviderEd
	{
		private class Data
		{
			public MathsDataProvider obj;

			public int idx;
		}

		public static readonly string[] S_ComparisonOpt = new string[5]
		{
			"+  add",
			"-  subtract",
			"×  multiply",
			"÷  divide",
			"%  modulo"
		};

		private static GUIContent[] GC_Operator = null;

		private static readonly GUIContent GC_Param1 = new GUIContent("Value 1");

		private static readonly GUIContent GC_Param2 = new GUIContent("Value 2");

		private static GenericMenu optMenu = null;

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			MathsDataProvider mathsDataProvider = target as MathsDataProvider;
			if (MathsDataProviderEd.GC_Operator == null)
			{
				MathsDataProviderEd.GC_Operator = new GUIContent[MathsDataProviderEd.S_ComparisonOpt.Length];
				for (int i = 0; i < MathsDataProviderEd.GC_Operator.Length; i++)
				{
					MathsDataProviderEd.GC_Operator[i] = new GUIContent(MathsDataProviderEd.S_ComparisonOpt[i]);
				}
			}
			Rect r = rect;
			Rect position = rect;
			position.width = 20f;
			r.width = (float)((r.width - position.width) / 2.0);
			position.x += r.width;
			DataProviderEd.DataBindingField(r, null, mathsDataProvider.param1, mathsDataProvider, false, null);
			if (GUI.Button(position, MathsDataProviderEd.GC_Operator[(int)mathsDataProvider.opt], plyEdGUI.Styles.MiniButton_LeftText))
			{
				MathsDataProviderEd.optMenu = new GenericMenu();
				for (int j = 0; j < MathsDataProviderEd.GC_Operator.Length; j++)
				{
					MathsDataProviderEd.optMenu.AddItem(MathsDataProviderEd.GC_Operator[j], false, this.OnOptChosen, new Data
					{
						obj = mathsDataProvider,
						idx = j
					});
				}
				MathsDataProviderEd.optMenu.DropDown(position);
			}
			r.x = r.x + r.width + position.width;
			DataProviderEd.DataBindingField(r, null, mathsDataProvider.param2, mathsDataProvider, false, null);
		}

		private void OnOptChosen(object arg)
		{
			Data data = (Data)arg;
			data.obj.opt = (MathsOperation)data.idx;
			plyEdUtil.SetDirty(data.obj);
		}
	}
}
