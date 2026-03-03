using UnityEngine;
using System.Collections;

public class PlayerAttackTrigger : MonoBehaviour
{
    public Collider meleeCollider;
    public float activationTime;

    public void ActivateCollider()
    {
        meleeCollider.enabled = true;
        StartCoroutine(ColliderActivationPeriod());
    }

    private IEnumerator ColliderActivationPeriod()
    {
        yield return new WaitForSeconds(activationTime);

        meleeCollider.enabled = false;
    }
}
