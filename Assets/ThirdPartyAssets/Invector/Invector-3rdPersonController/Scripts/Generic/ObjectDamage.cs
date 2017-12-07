using UnityEngine;
using System.Collections;

public class ObjectDamage : MonoBehaviour 
{
    [Tooltip("Apply damage to the Player Health")]
	public int damage;
    [Tooltip("Activated Ragdoll when hit the Player (Only works with ThirdPersonController)")]
    public bool activateRagdoll;

	void OnCollisionEnter(Collision hit)
	{
		if(hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
		{
			// apply damage to PlayerHealth
			hit.transform.root.SendMessage ("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			// activate the Ragdoll 
            if(activateRagdoll)
			    hit.transform.root.SendMessage ("ActivateRagdoll", SendMessageOptions.DontRequireReceiver);
		}
	}
}