using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {

	public GameObject bulletPrefab;
	public Transform target;
    public Transform bulletPos;
    public float time;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchBullet();
        }

    }
    private void LaunchBullet()
    {
        Vector3 Vo = CalculateVelocity(target.position, bulletPos.position, time);
        transform.rotation = Quaternion.LookRotation(Vo);

        GameObject bulletObj = GameObject.Instantiate(bulletPrefab.gameObject) as GameObject;
        bulletObj.transform.position = bulletPos.position;
        bulletObj.GetComponent<Rigidbody>().velocity = Vo;
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
 //   public float h = 25;
	//public float gravity = -18;
 //   public float scale = 1;
	//public bool debugPath;
 //   Rigidbody bullet;

 //   void Start() {
	//	//bullet.useGravity = false;
 //       h = Vector3.Distance(new Vector3(transform.position.x, target.position.y, transform.position.z), target.position)* scale;
	//}

	//void Update() {
	//	if (Input.GetKeyDown (KeyCode.Space)) {
	//		Launch ();
	//	}

	//	if (debugPath) {
	//		DrawPath ();
	//	}
	//}

	//void Launch()
 //   {
 //       GameObject bulletObj = GameObject.Instantiate(bulletPrefab.gameObject) as GameObject;
 //       bulletObj.transform.position = bulletPos.position;

 //       bullet = bulletObj.GetComponent<Rigidbody>();

 //       Physics.gravity = Vector3.up * gravity;
	//	bullet.useGravity = true;
	//	bullet.velocity = CalculateLaunchData ().initialVelocity;

 //       transform.LookAt(bullet.velocity);
	//}

	//LaunchData CalculateLaunchData() {
	//	float displacementY = target.position.y - bullet.position.y;
	//	Vector3 displacementXZ = new Vector3 (target.position.x - bullet.position.x, 0, target.position.z - bullet.position.z);
	//	float time = Mathf.Sqrt(-2*h/gravity) + Mathf.Sqrt(2*(displacementY - h)/gravity);
	//	Vector3 velocityY = Vector3.up * Mathf.Sqrt (-2 * gravity * h);
	//	Vector3 velocityXZ = displacementXZ / time;

	//	return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
	//}

	//void DrawPath() {
	//	LaunchData launchData = CalculateLaunchData ();
	//	Vector3 previousDrawPoint = bullet.position;

	//	int resolution = 30;
	//	for (int i = 1; i <= resolution; i++) {
	//		float simulationTime = i / (float)resolution * launchData.timeToTarget;
	//		Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up *gravity * simulationTime * simulationTime / 2f;
	//		Vector3 drawPoint = bullet.position + displacement;
	//		Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
	//		previousDrawPoint = drawPoint;
	//	}
	//}

	//struct LaunchData {
	//	public readonly Vector3 initialVelocity;
	//	public readonly float timeToTarget;

	//	public LaunchData (Vector3 initialVelocity, float timeToTarget)
	//	{
	//		this.initialVelocity = initialVelocity;
	//		this.timeToTarget = timeToTarget;
	//	}
		
	//}
}
	