using System.Collections;
using System.Collections.Generic;
using MoNo.Christmas;
using UnityEngine;

public class OutOfStage : MonoBehaviour, IObstacle
{
	public void OnEnterEvent(SnowBallBehavior snowball)
	{
		Destroy(snowball.gameObject);
	}

	public void OnExitEvent(SnowBallBehavior snowball)
	{
		Destroy(snowball.gameObject);
	}

	public void OnStayEvent(SnowBallBehavior snowball)
	{
		Destroy(snowball.gameObject);
	}
}
