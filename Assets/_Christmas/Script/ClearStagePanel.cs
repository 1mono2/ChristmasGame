using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoNo.Christmas;
using UnityEngine;


namespace MoNo.Christmas
{
	public class ClearStagePanel : MonoBehaviour, IObstacle
	{

		public void OnEnterEvent(SnowBallBehavior1 snowball)
		{
			snowball.Stop();
			var material = GetComponent<Renderer>().material;
			material.DOColor(Color.white, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			GameManager1.I.gameProgressState1.Value = GameManager1.GameProgressState1.Result;
		}

		public void OnExitEvent(SnowBallBehavior1 snowball)
		{

		}

		public void OnStayEvent(SnowBallBehavior1 snowball)
		{

		}

		private void OnDestroy()
		{
			this.transform.DOKill();
		}

	}
}