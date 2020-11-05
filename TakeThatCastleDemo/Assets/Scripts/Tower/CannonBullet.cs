using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : MonoBehaviour
{
    public float damageRange = 2f;
    public int damage = 5;

    private void OnCollisionEnter(Collision collision)
    {
        List<RoleController> redRoleList = SoldierSpawnMgr.S.GetRoleControllerInRange(transform.position, damageRange, RoleCamp.Red);
        redRoleList.ForEach(i => i.Hurt(damage));

        Destroy(gameObject);
    }
}
