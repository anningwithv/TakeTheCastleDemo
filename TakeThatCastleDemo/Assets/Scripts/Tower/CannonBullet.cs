using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : MonoBehaviour
{
    public float damageRange = 2f;
    public int damage = 5;
    public TargetBase targetBase;
    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (/*other.gameObject.layer == LayerMask.NameToLayer("Target") || */other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            List<RoleController> redRoleList = SoldierSpawnMgr.S.GetRoleControllerInRange(transform.position, damageRange, RoleCamp.Red);
            redRoleList.ForEach(i => i.Hurt(damage));

            GameObject effectObj = GameObject.Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
            Destroy(effectObj, 5f);
            //TargetBase target = other.gameObject.GetComponent<TargetBase>();
            //if (target == targetBase)
            //{
            //    return;
            //}
            //else
            //{
            //    if(target.Camp == RoleCamp.Red)
            //    {
            //        List<RoleController> redRoleList = SoldierSpawnMgr.S.GetRoleControllerInRange(transform.position, damageRange, RoleCamp.Red);
            //        redRoleList.ForEach(i => i.Hurt(damage));

            //        GameObject.Instantiate(explosionEffect, transform.position, Quaternion.identity);

            //        Destroy(gameObject);
            //    }
            //}
        }
    }
}
