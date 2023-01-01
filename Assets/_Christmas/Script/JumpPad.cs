using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoNo.Christmas
{

	public class JumpPad : MonoBehaviour, IObstacle
	{
		public void OnEnterEvent(SnowBallBehavior snowball)
		{
			snowball.Jump();
		}

		public void OnStayEvent(SnowBallBehavior snowball)
		{

		}

		public void OnExitEvent(SnowBallBehavior snowball)
		{

		}
	}
}