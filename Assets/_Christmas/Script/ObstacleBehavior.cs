using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MoNo.Christmas
{
	public class ObstacleBehavior : MonoBehaviour, IObstacle
	{
		[SerializeField] int num = -1;
		public int Num => num;


		public int Event(PlayerBehavior player)
		{
			return num;
		}

	}
}
