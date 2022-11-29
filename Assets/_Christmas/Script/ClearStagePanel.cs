using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearStagePanel : MonoBehaviour, IObstacle
{
	int num = 0;
	public int Num => num;

	public int Event(int currentNum)
	{
		throw new System.NotImplementedException();
	}

	public int Event(int currentNum, SnowBallBehavior snowBall)
	{

		return 0;
	}

}
