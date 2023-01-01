using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoNo.Christmas;
using UnityEngine;


namespace MoNo.Christmas
{
	public class ClearStagePanel : MonoBehaviour, IObstacle
	{

		public void OnEnterEvent(SnowBallBehavior snowball)
		{
			snowball.Stop();
			var material = GetComponent<Renderer>().material;
			material.DOColor(Color.white, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			GameManager.I.gameProgressState1.Value = GameManager.GameProgressState.Result;
		}

		public void OnExitEvent(SnowBallBehavior snowball)
		{

		}

		public void OnStayEvent(SnowBallBehavior snowball)
		{

		}

		private void OnDestroy()
		{
			this.transform.DOKill();
		}

	}
}