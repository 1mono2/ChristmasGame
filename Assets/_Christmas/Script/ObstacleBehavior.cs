using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MoNo.Christmas
{
	public class ObstacleBehavior : MonoBehaviour, IObstacle
	{
		public void OnEnterEvent(SnowBallBehavior snowball)
		{
			snowball.ResetSpeed();
			snowball.Break();
		}

		public void OnExitEvent(SnowBallBehavior snowball)
		{

		}

		public void OnStayEvent(SnowBallBehavior snowball)
		{

		}
	}
}
