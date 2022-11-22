using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rando = UnityEngine.Random;
using TMPro;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using MoNo.Utility;
using UniRx.Diagnostics;

namespace MoNo.Christmas {

	public enum GameProgressState
	{
		BeforeStart,
		Going,
		Presenting,
		Result,
		GameOver,
		nothing,
	}
	[System.Serializable]
	public class GameProgressStateReactiveProperty : ReactiveProperty<GameProgressState>
	{
		public GameProgressStateReactiveProperty() { }
		public GameProgressStateReactiveProperty(GameProgressState initialValue) : base(initialValue) { }

	}

	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		protected override bool DontDestroy => false;

		// property
		public GameProgressStateReactiveProperty gameProgressState { get { return _gameProgressState; } set { _gameProgressState = value; } }


		[Header("Realistic object")]
		[SerializeField] PlayerBehavior _player;
		[SerializeField] GameObject[] _stages;
		[SerializeField] ParticleSystem _confettiPref;

		[Header("System object")]
		// field
		GameProgressStateReactiveProperty _gameProgressState = new GameProgressStateReactiveProperty(GameProgressState.nothing);
		[SerializeField] CapsuleCollider _triggerCollider;
		[SerializeField] GameObject _camStopperPref;
		[SerializeField] List<Transform> _camStopperPosList;

		//[Header("UI object")]
		//[SerializeField] Canvas _startCanvas;
		//[SerializeField] RectTransform pinsCount;
		//[SerializeField] TextMeshProUGUI pinsCountText;
		//[SerializeField] TextMeshProUGUI addPinPref;
		//[SerializeField] TextMeshProUGUI multiplyRateText;
		//[SerializeField] StartCanvasBehavior startCanvas;
		//[SerializeField] Canvas goingCanvas;
		//[SerializeField] BowlingCanvasBehavior bowlingCanvas;
		//[SerializeField] ResultCanvasBehavior resultCanvas;
		//[SerializeField] GameOverCanvasBehavior gameOverCanvas;

		[SerializeField] int _initItemsQuantity = 10;


		// hide state
		int _multiplyRate = 0; // 0 ~10
		List<float> _multiplyRateCriterion = new List<float>() { 1, 1.2f, 1.4f, 1.6f, 1.8f, 2, 2.2f, 2.4f, 2.6f, 2.8f, 3 }; // 11 rank

		const string SAVE_STAGE_INDEX = "StageIndex";



		void Start()
		{
			DOTween.SetTweensCapacity(1500, 50);

			_gameProgressState.Value = GameProgressState.BeforeStart;

			_gameProgressState
				.Where(state => state == GameProgressState.BeforeStart)
				.Subscribe(state =>
				{
					OnBeforeStart();
					Debug.Log("before start");
				});

			_gameProgressState
				.Where(state => state == GameProgressState.Going)
				.Subscribe(state =>
				{
					OnGoing();
					Debug.Log("going");
				});


			_gameProgressState
			  .Where(state => state == GameProgressState.Presenting)
			  .Subscribe(state =>
			  {
				  OnPresenting();
				  Debug.Log("presenting");
			  });


			_gameProgressState
				.Where(state => state == GameProgressState.Result)
				.Subscribe(state =>
				{
					OnResult();
					Debug.Log("result");
				});

			_gameProgressState
				.Where(state => state == GameProgressState.GameOver)
				.Subscribe(state =>
				{
					OnGameOver();
					Debug.Log("game over");
				});



		}

		void OnBeforeStart(){}

		void OnGoing()
		{
			//goingCanvas.gameObject.SetActive(true);

			// move
			_player.Move();


			_itemManager.currentItemNum
				.Skip(1)
				.Subscribe(i =>
				{
					// display number of items
					//pinsCountText.text = i.ToString();
					if (i <= 0)
					{
						_gameProgressState.Value = GameProgressState.GameOver;
					}
				}).AddTo(this);

			_triggerCollider.OnTriggerEnterAsObservable()
				 .Subscribe(triggerObj =>
				 {
					 if (triggerObj.TryGetComponent<IObstacle>(out var obstacle))
					 {
						 int num = obstacle.Event(_itemManager.currentItemNum.Value);
								//var addPinText = Instantiate(addPinPref, goingCanvas.gameObject.transform);
						 if (num > 0)
						 {
							 for (int i = 0; i < num; i++)
							 {
								 _itemManager.GenerateItem(_itemManager.transform.position + new Vector3(0, 0, -2), _itemManager.transform);
							 }
									//addPinText.text = $"+" + num.ToString();
								}
						 else
						 {
							 for (int i = 0; i < -num; i++)
							 {
								 _itemManager.DeleteItem();
							 }
									//addPinText.text = num.ToString();
						 }

					 }

					 if(triggerObj.TryGetComponent<PanelController>(out var panel))
					 {
						 panel.DisAppear();
					 }
				 }).AddTo(_triggerCollider).AddTo(this);



		  

			// goal flag stands
			_triggerCollider.OnTriggerEnterAsObservable()
				.Where(trrigerObj => trrigerObj.CompareTag("GoalFlag"))
				.Subscribe(triggerObj =>
				{
					_gameProgressState.Value = GameProgressState.Presenting;
					
				}).AddTo(_triggerCollider).AddTo(this);
		}

		void OnPresenting()
		{

			// move
			_player.Move();

			//goingCanvas.gameObject.SetActive(false);
			//bowlingCanvas.gameObject.SetActive(true);


	

			//multiplyRateText.text = $"x{multiplyRateCriterion[multiplyRate]}";

			Vector3 camStopperPos = _camStopperPosList[_multiplyRate].position;
			Instantiate(_camStopperPref, camStopperPos, Quaternion.identity);
			Instantiate(_confettiPref, camStopperPos + new Vector3(-4, 0, 5), Quaternion.Euler(-45, 90, 0));
			Instantiate(_confettiPref, camStopperPos + new Vector3(4, 0, 5), Quaternion.Euler(-45, -90, 0));

			_stages[_multiplyRate].GetComponent<Renderer>().material.DOColor(Color.white, 1).SetLoops(-1, LoopType.Yoyo);

			_triggerCollider.OnTriggerEnterAsObservable()
				.Where(obj => obj.CompareTag("CamStopper"))
				.Subscribe(_ =>
				{
					_player.Stop();
					_gameProgressState.Value = GameProgressState.Result;
				}).AddTo(this).AddTo(_triggerCollider);
		}

		void OnGameOver()
		{
			_player.Stop();
			//gameOverCanvas.retryButtonAction += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			//gameOverCanvas.gameObject.SetActive(true);
		}

		void OnResult()
		{
			//bowlingCanvas.gameObject.SetActive(false);

			var resultScoreCount = _itemManager.finalItemsNum;
			//resultCanvas.gameObject.SetActive(true);
			//if (LoadData.I.isShowAd == true)
			//{
			//    resultCanvas.nextLevelButtonAction += ShowAds;
			//}
			//else
			//{
			//    resultCanvas.nextLevelButtonAction += NextStage;
			//}
			SaveStageIndex();
		}

		void SaveStageIndex()
		{
			PlayerPrefs.SetInt(SAVE_STAGE_INDEX, SceneManager.GetActiveScene().buildIndex);
		}

		void ShowAds()
		{
			//InterstitialAds.I.OnAdClosed.AddListener(NextStage);
			//InterstitialAds.I.ShowIfLoaded();
		}

		public void NextStage()
		{

			var currentStageIndex = SceneManager.GetActiveScene().buildIndex;
			int nextStageIndex;

			if (currentStageIndex < SceneManager.sceneCountInBuildSettings - 1)
			{
				nextStageIndex = currentStageIndex + 1;
			}
			else
			{
				nextStageIndex = 1;  // Return the First Stage without Preload
			}

			SceneManager.LoadScene(nextStageIndex);

		}


	}
}