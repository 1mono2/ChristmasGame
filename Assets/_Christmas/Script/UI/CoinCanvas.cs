using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoNo.Christmas
{
	public class CoinCanvas : MonoBehaviour
	{
		Camera _camera;
		[SerializeField] TextMeshProUGUI _coinText;
		[SerializeField] Image _coinImage;
		[SerializeField] Image _coinImagePref;
		[SerializeField] float _coinMoveSpeed = 0.8f;

		void Start()
		{
			_camera = Camera.main;
			if (_coinText == null) Debug.LogWarning("Don't have _coinText");
			CoinManager.I.coinCount
				.Subscribe(count =>
				{
					_coinText.text = count.ToString();
				}).AddTo(this);

			CoinManager.I.coinWorldPosition
			.Skip(1)
				.Subscribe(worldPosition =>
				{
					var coinImage = Instantiate(_coinImagePref, _camera.WorldToScreenPoint(worldPosition), Quaternion.identity, this.transform);
					coinImage.rectTransform.DOMove(_coinImage.transform.position, _coinMoveSpeed)
					.OnComplete(() =>
					{
						Destroy(coinImage.gameObject);
					});
				}).AddTo(this);
		}


	}
}
