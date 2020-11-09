using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteBullet : MonoBehaviour
{
    public int hurtValue;
    public RoleCamp camp;
    public Vector3 flyDir;
    public float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        TargetBase target = other.gameObject.GetComponent<TargetBase>();
        if(target != null && target.Camp != camp)
        {
            target.Hurt(hurtValue);
        }
    }
}
