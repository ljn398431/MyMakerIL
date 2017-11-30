namespace BloxEngine
{
	[BloxBlock("Flow/Goto First", BloxBlockType.Action, Order = 320, Style = "red")]
	public class GotoFirst_Block : BloxBlock
	{
		protected override object RunBlock()
		{
			base.flowSig = BloxFlowSignal.Continue;
			return null;
		}
	}
}
