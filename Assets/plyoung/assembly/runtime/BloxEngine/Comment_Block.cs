namespace BloxEngine
{
	[BloxBlock("Common/Comment", BloxBlockType.Action, Order = 200, OverrideRenderFields = 1, Style = "grey")]
	public class Comment_Block : BloxBlock
	{
		public string message = "";

		public override void CopyTo(BloxBlock block)
		{
			base.CopyTo(block);
			(block as Comment_Block).message = this.message;
		}

		protected override void InitBlock()
		{
		}

		protected override object RunBlock()
		{
			return null;
		}
	}
}
