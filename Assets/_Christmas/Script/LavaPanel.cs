using System.Collections;
using System.Collections.Generic;
using MoNo.Christmas;
using UnityEngine;

namespace MoNo.Christmas
{
	public class LavaPanel : MonoBehaviour, IObstacle
	{
		public void OnEnterEvent(SnowBallBehavior snowball)
		{
			Debug.Log("Enter lava zone");
		}

		public void OnStayEvent(SnowBallBehavior snowball)
		{
			snowball.DownSize();
		}

		public void OnExitEvent(SnowBallBehavior snowball)
		{
			Debug.Log("Exit lava zone");
		}
	}
}