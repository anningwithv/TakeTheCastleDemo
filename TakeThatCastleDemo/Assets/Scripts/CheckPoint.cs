using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int index;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO:Check triggered by our soldier

        CheckPointMgr.S.OnCheckPointTriggered(this);
    }
}
