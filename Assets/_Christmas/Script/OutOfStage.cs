using System.Collections;
using System.Collections.Generic;
using MoNo.Christmas;
using UnityEngine;

public class OutOfStage : MonoBehaviour, IObstacle
{
	public void OnEnterEvent(SnowBallBehavior snowball)
	{
		snowball.OnDisapear();
	}

	public void OnExitEvent(SnowBallBehavior snowball)
	{
		snowball.OnDisapear();
	}

	public void OnStayEvent(SnowBallBehavior snowball)
	{
		snowball.OnDisapear();
	}
}
