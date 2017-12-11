using BloxEngine;
using BloxEngine.Databinding;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	[plyCustomEd(typeof(ComparisonDataProvider), Name = "Comparison Check")]
	public class ComparisonCheckEd : DataProviderEd
	{
		private class Data
		{
			public ComparisonDataProvider obj;

			public int idx;
		}

		private static readonly string[] S_ComparisonOpt = new string[8]
		{
			"==",
			"!=",
			"<",
			">",
			"<=",
			">=",
			"and",
			"or"
		};

		private static GUIContent[] GC_Operator = null;

		private static readonly GUIContent GC_Param1 = new GUIContent("Comparison Value 1");

		private static readonly GUIContent GC_Param2 = new GUIContent("Comparison Value 2");

		private static GenericMenu optMenu = null;

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
            ComparisonDataProvider comparisonDataProvider = target as ComparisonDataProvider;
            rect.height = EditorGUIUtility.singleLineHeight;
            if (ComparisonCheckEd.GC_Operator == null)
            {
                ComparisonCheckEd.GC_Operator = new GUIContent[ComparisonCheckEd.S_ComparisonOpt.Length];
                for (int i = 0; i < ComparisonCheckEd.GC_Operator.Length; i++)
                {
                    ComparisonCheckEd.GC_Operator[i] = new GUIContent(ComparisonCheckEd.S_ComparisonOpt[i]);
                }
            }
            Rect r = rect;
            Rect position = rect;
            position.width = 28f;
            r.width = (float)((r.width - position.width) / 2.0);
            position.x += r.width;
            DataProviderEd.DataBindingField(r, null, comparisonDataProvider.param1, comparisonDataProvider, false, null);
            if (GUI.Button(position, ComparisonCheckEd.GC_Operator[(int)comparisonDataProvider.comparisonOpt]))
            {
                ComparisonCheckEd.optMenu = new GenericMenu();
                for (int j = 0; j < ComparisonCheckEd.GC_Operator.Length; j++)
                {
                    ComparisonCheckEd.optMenu.AddItem(ComparisonCheckEd.GC_Operator[j], false, this.OnOptChosen, new Data
                    {
                        obj = comparisonDataProvider,
                        idx = j
                    });
                }
                ComparisonCheckEd.optMenu.DropDown(position);
            }
            r.x = r.x + r.width + position.width;
            DataProviderEd.DataBindingField(r, null, comparisonDataProvider.param2, comparisonDataProvider, false, null);
        }

		private void OnOptChosen(object arg)
		{
			Data data = (Data)arg;
			data.obj.comparisonOpt = (ComparisonOperation)data.idx;
			plyEdUtil.SetDirty(data.obj);
		}
	}
}
