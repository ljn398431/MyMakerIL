using BloxEngine.Databinding;
using plyLibEditor;
using UnityEditor;
using UnityEngine;

namespace BloxEditor.Databinding
{
	[plyCustomEd(typeof(GraphMappedValuesProvider), Name = "Graph Mapped Values")]
	public class GraphMappedValuesProviderEd : DataProviderEd
	{
		private static readonly GUIContent GC_Param1 = new GUIContent("Input Value");

		private static readonly GUIContent GC_InputY = new GUIContent("Input is Y");

		private static readonly GUIContent GC_Curve = new GUIContent("Graph");

		private static readonly GUIContent GC_Label = new GUIContent();

		public override float EditorHeight(DataProvider target, bool isSetter)
		{
			return (float)((EditorGUIUtility.singleLineHeight + 2.0) * 3.0);
		}

		protected override void Draw(Rect rect, DataProvider target, bool isSetter)
		{
			GraphMappedValuesProvider graphMappedValuesProvider = target as GraphMappedValuesProvider;
			GraphMappedValuesProviderEd.GC_Label.text = DataProviderEd.GetDataBindingLabel(graphMappedValuesProvider.param1.dataprovider);
			rect.height = EditorGUIUtility.singleLineHeight;
			DataProviderEd.DataBindingField(rect, GraphMappedValuesProviderEd.GC_Param1, graphMappedValuesProvider.param1, graphMappedValuesProvider, false, null);
			rect.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			graphMappedValuesProvider.yIsInput = EditorGUI.Toggle(rect, GraphMappedValuesProviderEd.GC_InputY, graphMappedValuesProvider.yIsInput);
			rect.y += (float)(EditorGUIUtility.singleLineHeight + 2.0);
			graphMappedValuesProvider.curve = plyEdGUI.GraphMappedValuesField(rect, GraphMappedValuesProviderEd.GC_Curve, graphMappedValuesProvider.curve);
		}
	}
}
