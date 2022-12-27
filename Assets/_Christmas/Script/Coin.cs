using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoNo.Christmas;
using UnityEngine;
using UnityEngine.VFX;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace MoNo.Christmas
{
	// This class is used to control the coin
	public class Coin : MonoBehaviour, IDuplicatableObstacle
	{
		[SerializeField] GameObject _coinObj;
		[SerializeField] VisualEffect _breakEffectPref;

		public void OnEnterEvent(SnowBallBehavior snowball)
		{
			var position = this.transform.position;
			CoinManager.I.AddCoin(1, position);
			_coinObj.SetActive(false);
			var breakEffect = Instantiate(_breakEffectPref, position, Quaternion.identity);
			breakEffect.Play();
			UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy()).Forget();
			Destroy(this.gameObject);
		}

		public void OnStayEvent(SnowBallBehavior snowball)
		{

		}

		public void OnExitEvent(SnowBallBehavior snowball)
		{

		}

	}


}