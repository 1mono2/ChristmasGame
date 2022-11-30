using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MoNo.Christmas
{
	public class ObstacleBehavior : MonoBehaviour, IObstacle
	{
		public void OnEnterEvent(SnowBallBehavior1 snowball)
		{
			snowball.ResetSpeed();
			snowball.Break();
		}

		public void OnExitEvent(SnowBallBehavior1 snowball)
		{

		}

		public void OnStayEvent(SnowBallBehavior1 snowball)
		{

		}
	}
}
