using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoNo.Christmas;
using UnityEngine;


namespace MoNo.Christmas
{
	public class ClearStagePanel : MonoBehaviour, IObstacle
	{
		int num = 0;
		public int Num => num;

		public int Event(PlayerBehavior player)
		{
			player.Stop();
			var material = GetComponent<Renderer>().material;
			material.DOColor(Color.white, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			return 0;

		}

		private void OnDestroy()
		{
			this.transform.DOKill();
		}

	}
}