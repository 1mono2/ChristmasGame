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

		public async void OnEnterEvent(SnowBallBehavior snowball)
		{
			CoinManager.I.AddCoin(1, this.transform.position);
			_coinObj.SetActive(false);
			var _breakEffect = Instantiate(_breakEffectPref, this.transform.position, Quaternion.identity);
			_breakEffect.Play();
			await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
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