using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoNo.Christmas
{
	public interface IDuplicatableObstacle
	{
		void OnEnterEvent(SnowBallBehavior snowball);
		void OnStayEvent(SnowBallBehavior snowball);
		void OnExitEvent(SnowBallBehavior snowball);

	}
}

