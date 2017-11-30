namespace BloxEditor
{
	public abstract class BloxBlockDrawer
	{
		public abstract void DrawHead(BloxEditorWindow ed, BloxBlockEd bdi);

		public abstract void DrawFields(BloxEditorWindow ed, BloxBlockEd bdi);

		public abstract void DrawProperties(BloxEditorWindow ed, BloxBlockEd bdi);
	}
}
