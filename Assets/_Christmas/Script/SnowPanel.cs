using System.Collections;
using System.Collections.Generic;
using MoNo.Christmas;
using UnityEngine;

public class SnowPanel : MonoBehaviour, IObstacle
{
	public void OnEnterEvent(SnowBallBehavior1 snowball)
	{
		Debug.Log("Enter snow zone");
	}

	public void OnStayEvent(SnowBallBehavior1 snowball)
	{
		snowball.UpSize();
	}

	public void OnExitEvent(SnowBallBehavior1 snowball)
	{
		Debug.Log("Exit snow zone");
	}


}
