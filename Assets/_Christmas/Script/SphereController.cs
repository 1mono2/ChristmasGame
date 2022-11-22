using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class SphereController : MonoBehaviour
{
	[SerializeField] FloatingJoystick joystick;
	public float speed;
	Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
		rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
	}


	public void ChangeSphereSize(float unit)
	{
		float multiplyTime = unit * Time.fixedDeltaTime;
		 this.transform.DOBlendableScaleBy(new Vector3(multiplyTime, multiplyTime, multiplyTime), 0);
	}

}
