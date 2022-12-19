using System.Collections;
using System.Collections.Generic;
using MoNo.Utility;
using UniRx;
using UnityEngine;

namespace MoNo.Christmas
{


	public class CoinManager : SingletonMonoBehaviour<CoinManager>
	{
		protected override bool DontDestroy => true;

		IntReactiveProperty _coinCount = new(0);
		public IReadOnlyReactiveProperty<int> coinCount => _coinCount;
		Vector3ReactiveProperty _coinWorldPosition = new(Vector3.zero);
		public IReadOnlyReactiveProperty<Vector3> coinWorldPosition => _coinWorldPosition;

		const string SAVE_COIN_COUNT = "CoinCount";

		// Start is called before the first frame update
		void Start()
		{
			if (PlayerPrefs.HasKey(SAVE_COIN_COUNT))
			{
				_coinCount.Value = PlayerPrefs.GetInt(SAVE_COIN_COUNT);
			}
			else
			{
				PlayerPrefs.SetInt(SAVE_COIN_COUNT, 0);
				_coinCount.Value = 0;
			}

		}

		public void AddCoin(int addCount, Vector3 worldPosition)
		{
			_coinCount.Value += addCount;
			_coinWorldPosition.Value = worldPosition;
		}
		private void OnApplicationQuit()
		{
			PlayerPrefs.SetInt(SAVE_COIN_COUNT, _coinCount.Value);
		}

	}

}
