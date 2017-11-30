using BloxEngine;
using UnityEngine;

namespace BloxGameSystems
{
	[ExcludeFromBlox]
	[DisallowMultipleComponent]
	[AddComponentMenu("Blox/Systems/Area Trigger")]
	[HelpURL("https://plyoung.github.io/blox-area-trigger.html")]
	public class AreaTrigger : MonoBehaviour
	{
		public enum AreaKind
		{
			Rectangular = 0,
			Circular = 1
		}

		public AreaKind areaKind = AreaKind.Circular;

		[Tooltip("Will only trigger for GameObjects in these layers.")]
		public LayerMask triggerLayerMask = -1;

		[Tooltip("A RigidBody is needed on either the Trigger or the object(s) which can interact with it. Turn this on to have one added to the Trigger at runtime.")]
		public bool addRigidbody = true;

		[Tooltip("An optional identifier which will be made available as an Event Variable in the Area Trigger's Blox Events.")]
		public string ident = "";

		private Collider thisCollider;

		private AreaTrigger_BloxEventHandler handler;

		protected void Awake()
		{
			if (this.areaKind == AreaKind.Circular)
			{
				SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
				sphereCollider.isTrigger = true;
				sphereCollider.radius = 1f;
				this.thisCollider = sphereCollider;
			}
			else if (this.areaKind == AreaKind.Rectangular)
			{
				BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
				boxCollider.isTrigger = true;
				boxCollider.size = Vector3.one;
				this.thisCollider = boxCollider;
			}
			if (this.addRigidbody)
			{
				Rigidbody rigidbody = base.gameObject.GetComponent<Rigidbody>();
				if ((Object)rigidbody == (Object)null)
				{
					rigidbody = base.gameObject.AddComponent<Rigidbody>();
				}
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				rigidbody.useGravity = false;
				rigidbody.interpolation = RigidbodyInterpolation.None;
				rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
		}

		protected void Start()
		{
			this.handler = base.gameObject.GetComponent<AreaTrigger_BloxEventHandler>();
		}

		private void OnTriggerEnter(Collider col)
		{
			if ((1 << col.gameObject.layer & this.triggerLayerMask) != 0)
			{
				AreaTrigger_BloxEventHandler obj = this.handler;
				if ((object)obj != null)
				{
					obj.OnAreaTriggerEnter(col, this.ident);
				}
				AreaTrigger_BloxEventHandler component = col.gameObject.GetComponent<AreaTrigger_BloxEventHandler>();
				if ((object)component != null)
				{
					component.OnAreaTriggerEnter(this.thisCollider, this.ident);
				}
			}
		}

		private void OnTriggerExit(Collider col)
		{
			if ((1 << col.gameObject.layer & this.triggerLayerMask) != 0)
			{
				AreaTrigger_BloxEventHandler obj = this.handler;
				if ((object)obj != null)
				{
					obj.OnAreaTriggerExit(col, this.ident);
				}
				AreaTrigger_BloxEventHandler component = col.gameObject.GetComponent<AreaTrigger_BloxEventHandler>();
				if ((object)component != null)
				{
					component.OnAreaTriggerExit(this.thisCollider, this.ident);
				}
			}
		}

		private void OnTriggerStay(Collider col)
		{
			if ((1 << col.gameObject.layer & this.triggerLayerMask) != 0)
			{
				AreaTrigger_BloxEventHandler obj = this.handler;
				if ((object)obj != null)
				{
					obj.OnAreaTriggerStay(col, this.ident);
				}
				AreaTrigger_BloxEventHandler component = col.gameObject.GetComponent<AreaTrigger_BloxEventHandler>();
				if ((object)component != null)
				{
					component.OnAreaTriggerStay(this.thisCollider, this.ident);
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(base.transform.position - Vector3.up * 0.1f, base.transform.position + Vector3.up * 1.75f);
			Gizmos.DrawCube(base.transform.position - Vector3.up * 0.1f, new Vector3(0.1f, 0.1f, 0.1f));
			Gizmos.DrawIcon(base.transform.position + Vector3.up * 2f, "BloxEngine/BGS/trigger.png");
			if (this.areaKind == AreaKind.Circular)
			{
				float radius = Mathf.Max(base.transform.localScale.x, Mathf.Max(base.transform.localScale.y, base.transform.localScale.z));
				Gizmos.color = new Color(1f, 0f, 1f, 0.15f);
				Gizmos.DrawSphere(base.transform.position, radius);
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere(base.transform.position, radius);
			}
			else if (this.areaKind == AreaKind.Rectangular)
			{
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.color = new Color(1f, 0f, 1f, 0.15f);
				Gizmos.DrawCube(Vector3.zero, Vector3.one);
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}
		}
	}
}
