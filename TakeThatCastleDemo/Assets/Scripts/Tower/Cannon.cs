using UnityEngine;
using System.Collections;

public class Cannon : TargetBase
{
    [SerializeField] private GameObject m_TargetPosObj;
	public GameObject bulletPrefab;
	public Transform target;
    public Transform bulletPos;
    public Transform cannonTrans;
    public float time;
    public float attackRange = 5;
    public float attackInterval = 3;

    private float m_FindTargetInterval = 1f;
    private float m_FindTargetTime = 0f;

    private float m_AttackTime = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBullet();
        }

        m_FindTargetTime += Time.deltaTime;
        if (m_FindTargetTime > m_FindTargetInterval)
        {
            m_FindTargetTime = 0f;

            FindTarget();
        }

        m_AttackTime += Time.deltaTime;
        if (m_AttackTime > attackInterval)
        {
            m_AttackTime = 0f;

            if(target != null)
                LaunchBullet();
        }
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, 1 << LayerMask.NameToLayer("Target"));

        if (colliders.Length > 0)
        {
            foreach (var item in colliders)
            {
                TargetBase role = item.gameObject.GetComponent<TargetBase>();
                if (role != null && role != this && role.Status != RoleStatus.Die && role.Camp != RoleCamp.None && role.Camp == RoleCamp.Red)
                {
                    target = role.transform;

                    break;
                }
            }
        }
    }

    public override void Hurt(int value)
    {
        m_HP -= value;

        if (m_HP <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die();
        
        DieCallBack?.Invoke(this);

        Destroy(gameObject);
    }

    private void LaunchBullet()
    {
        Vector3 Vo = CalculateVelocity(target.position, bulletPos.position, time);
        cannonTrans.rotation = Quaternion.LookRotation(Vo);

        GameObject bulletObj = GameObject.Instantiate(bulletPrefab.gameObject) as GameObject;
        bulletObj.transform.position = bulletPos.position;
        bulletObj.GetComponent<Rigidbody>().velocity = Vo;

        CannonBullet bullet = bulletObj.GetComponent<CannonBullet>();
        bullet.targetBase = this;
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    public override GameObject GetTargetPosObj()
    {
        return m_TargetPosObj;
    }
}
	