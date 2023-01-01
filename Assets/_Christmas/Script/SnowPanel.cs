using System.Collections;
using System.Collections.Generic;
using MoNo.Christmas;
using UnityEngine;

namespace MoNo.Christmas
{
	public class SnowPanel : MonoBehaviour, IObstacle
	{
		public void OnEnterEvent(SnowBallBehavior snowball)
		{
			Debug.Log("Enter snow zone");
		}

		public void OnStayEvent(SnowBallBehavior snowball)
		{
			snowball.UpSize();
		}

		public void OnExitEvent(SnowBallBehavior snowball)
		{
			Debug.Log("Exit snow zone");
		}


	}
}
