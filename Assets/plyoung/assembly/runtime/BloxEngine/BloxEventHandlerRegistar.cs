using UnityEngine;

namespace BloxEngine
{
	[ExcludeFromBlox]
	public class BloxEventHandlerRegistar
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RegisterEventsHandlers()
		{
			BloxGlobal.RegisterEventHandlerType("Custom", typeof(Custom_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/Awake", typeof(Common_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/Start", typeof(Common_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/OnDestroy", typeof(Common_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/OnEnable", typeof(Common_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/OnDisable", typeof(Common_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/Update", typeof(Update_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/LateUpdate", typeof(UpdateLate_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Common/FixedUpdate", typeof(UpdateFixed_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnCollisionEnter", typeof(Collision3D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnCollisionExit", typeof(Collision3D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnCollisionStay", typeof(Collision3D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/with Info/OnCollisionEnter(...)", typeof(Collision3D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/with Info/OnCollisionExit(...)", typeof(Collision3D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/with Info/OnCollisionStay(...)", typeof(Collision3D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnCollisionEnter2D", typeof(Collision2D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnCollisionExit2D", typeof(Collision2D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnCollisionStay2D", typeof(Collision2D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/with Info/OnCollisionEnter2D(...)", typeof(Collision2D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/with Info/OnCollisionExit2D(...)", typeof(Collision2D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/with Info/OnCollisionStay2D(...)", typeof(Collision2D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnControllerColliderHit", typeof(CharController_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Collision/OnParticleCollision", typeof(Particle_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/OnTriggerEnter", typeof(Trigger3D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/OnTriggerExit", typeof(Trigger3D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/OnTriggerStay", typeof(Trigger3D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/with Info/OnTriggerEnter(...)", typeof(Trigger3D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/with Info/OnTriggerExit(...)", typeof(Trigger3D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/with Info/OnTriggerStay(...)", typeof(Trigger3D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/OnTriggerEnter2D", typeof(Trigger2D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/OnTriggerExit2D", typeof(Trigger2D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/OnTriggerStay2D", typeof(Trigger2D_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/with Info/OnTriggerEnter2D(...)", typeof(Trigger2D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/with Info/OnTriggerExit2D(...)", typeof(Trigger2D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Trigger/with Info/OnTriggerStay2D(...)", typeof(Trigger2D_nfo_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Input/OnMouseDrag", typeof(Mouse_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Input/OnMouseEnter", typeof(Mouse_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Input/OnMouseExit", typeof(Mouse_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Input/OnMouseOver", typeof(Mouse_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Input/OnMouseUp", typeof(Mouse_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Input/OnMouseDown", typeof(Mouse_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Input/OnMouseUpAsButton", typeof(Mouse_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnApplicationFocus", typeof(Application_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnApplicationPause", typeof(Application_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnApplicationQuit", typeof(Application_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnLevelWasLoaded", typeof(Application_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnAnimatorMove", typeof(Animator_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnAnimatorIK", typeof(Animator_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnAudioFilterRead", typeof(Audio_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnBecameInvisible", typeof(Renderer_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnBecameVisible", typeof(Renderer_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnJointBreak", typeof(Joint_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnTransformParentChanged", typeof(Transform_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnTransformChildrenChanged", typeof(Transform_BloxEventHandler));
			BloxGlobal.RegisterEventHandlerType("Misc/OnGUI", typeof(OnGUI_BloxEventHandler));
		}
	}
}
