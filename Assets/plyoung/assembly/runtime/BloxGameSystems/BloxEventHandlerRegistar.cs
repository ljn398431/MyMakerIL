using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	public class BloxEventHandlerRegistar
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RegisterEventsHandlers()
		{
			BloxGlobal.RegisterEventHandlerType("Game Systems/Startup/OnBootstrapDone", typeof(Bootstrap_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Game Systems/Startup/OnSpashScreenShown", typeof(SplashScreensManager_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Game Systems/Startup/OnSpashScreenHidden", typeof(SplashScreensManager_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Game Systems/Startup/OnSpashScreensDone", typeof(SplashScreensManager_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Game Systems/AreaTrigger/OnAreaTriggerEnter", typeof(AreaTrigger_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Game Systems/AreaTrigger/OnAreaTriggerExit", typeof(AreaTrigger_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Game Systems/AreaTrigger/OnAreaTriggerStay", typeof(AreaTrigger_BloxEventHandler));
		}
	}
}
