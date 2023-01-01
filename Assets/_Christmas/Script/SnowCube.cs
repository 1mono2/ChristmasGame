using System.Collections;
using System.Collections.Generic;
using MoNo.Christmas;
using UnityEngine;

public class SnowCube : MonoBehaviour, IDuplicatableObstacle
{
	public void OnEnterEvent(SnowBallBehavior snowball)
	{
		Destroy(this.gameObject);

	}

	public void OnExitEvent(SnowBallBehavior snowball)
	{
	}

	public void OnStayEvent(SnowBallBehavior snowball)
	{

	}

}
