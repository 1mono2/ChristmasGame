using System.Collections;
using System.Collections.Generic;
using MoNo.Christmas;
using UnityEngine;

public class LavaPanel : MonoBehaviour, IObstacle
{
	public void OnEnterEvent(SnowBallBehavior1 snowball)
	{
		Debug.Log("Enter lava zone");
	}

	public void OnStayEvent(SnowBallBehavior1 snowball)
	{
		snowball.DownSize();
	}

	public void OnExitEvent(SnowBallBehavior1 snowball)
	{
		Debug.Log("Exit lava zone");
	}
}
