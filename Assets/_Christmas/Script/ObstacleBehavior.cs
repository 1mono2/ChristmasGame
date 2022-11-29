using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace MoNo.Christmas
{
    public class ObstacleBehavior : MonoBehaviour, IObstacle
    {
        [SerializeField] int num = -1;
        public int Num => num;

		
		public int Event(int currentNum)
		{
            return num;
		}

    }
}
