using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoNo.Utility;
using TMPro;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rando = UnityEngine.Random;

namespace MoNo.Christmas
{
	public class GameManager1 : SingletonMonoBehaviour<GameManager1>
	{
		protected override bool DontDestroy => false;

		// property
		public GameProgressStateReactiveProperty1 gameProgressState1 { get { return _gameProgressState1; } set { _gameProgressState1 = value; } }


		[Header("Realistic object")]
		[SerializeField] SnowBallBehavior1 _snowBall;
		[SerializeField] GameObject[] _stages;
		[SerializeField] ParticleSystem _confettiPref;
		[SerializeField] ChaseTarget _mainCamera;

		[Header("System object")]
		// field
		GameProgressStateReactiveProperty1 _gameProgressState1 = new(GameProgressState1.nothing);
		SphereCollider _snowBallCollider;

		[Header("UI object")]
		[SerializeField] StartCanvasBehavior _startCanvas;
		//[SerializeField] RectTransform pinsCount;
		//[SerializeField] TextMeshProUGUI pinsCountText;
		//[SerializeField] TextMeshProUGUI addPinPref;
		//[SerializeField] TextMeshProUGUI multiplyRateText;
		//[SerializeField] StartCanvasBehavior startCanvas;
		//[SerializeField] Canvas goingCanvas;
		//[SerializeField] BowlingCanvasBehavior bowlingCanvas;
		//[SerializeField] ResultCanvasBehavior resultCanvas;
		//[SerializeField] GameOverCanvasBehavior gameOverCanvas;


		// hide state
		readonly int _multiplyRate = 0; // 0 ~10
		readonly List<float> _multiplyRateCriterion = new() { 1, 1.2f, 1.4f, 1.6f, 1.8f, 2, 2.2f, 2.4f, 2.6f, 2.8f, 3 }; // 11 rank

		const string SAVE_STAGE_INDEX = "StageIndex";



		void Start()
		{
			DOTween.SetTweensCapacity(1500, 50);
			_snowBallCollider = _snowBall.Collider;
			_gameProgressState1.Value = GameProgressState1.BeforeStart;

			_gameProgressState1
				.Where(state => state == GameProgressState1.BeforeStart)
				.Subscribe(state =>
				{
					OnBeforeStart();
					Debug.Log("before start");
				});

			_gameProgressState1
				.Where(state => state == GameProgressState1.Going)
				.Subscribe(state =>
				{
					OnGoing();
					Debug.Log("going");
				});


			_gameProgressState1
			  .Where(state => state == GameProgressState1.AfterGoal)
			  .Subscribe(state =>
			  {
				  OnAfterGoal();
				  Debug.Log("AfterGoal");
			  });


			_gameProgressState1
				.Where(state => state == GameProgressState1.Result)
				.Subscribe(state =>
				{
					OnResult();
					Debug.Log("result");
				});

			_gameProgressState1
				.Where(state => state == GameProgressState1.GameOver)
				.Subscribe(state =>
				{
					OnGameOver();
					Debug.Log("game over");
				});



		}

		void OnBeforeStart()
		{
			_startCanvas.tapToStart.OnFinger.AddListener(leanFinger =>
			{
				_startCanvas.gameObject.SetActive(false);
				_gameProgressState1.Value = GameProgressState1.Going;
			});

			_startCanvas.levelText.text = $"Level {SceneManager.GetActiveScene().buildIndex}";

			SetCamera();
		}


		void OnGoing()
		{
			//goingCanvas.gameObject.SetActive(true);

			_snowBall.OnDestroyEvent.AddListener(() =>
			{
				_gameProgressState1.Value = GameProgressState1.GameOver;
			});

			// Collision Event
			_snowBallCollider.OnCollisionEnterAsObservable()
				.Subscribe(col =>
				{
					if (col.collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnEnterEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnCollisionStayAsObservable()
				.Subscribe(col =>
				{
					if (col.collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnStayEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnCollisionExitAsObservable()
				.Subscribe(col =>
				{
					if (col.collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnExitEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			// Trigger Event
			_snowBallCollider.OnTriggerEnterAsObservable()
				.Subscribe(collider =>
				{
					if (collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnEnterEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnTriggerStayAsObservable()
				.Subscribe(collider =>
				{
					if (collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnStayEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnTriggerExitAsObservable()
				.Subscribe(collider =>
				{
					if (collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnExitEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			// move
			_snowBall.Move();

		}

		void OnAfterGoal()
		{

			//goingCanvas.gameObject.SetActive(false);
			//bowlingCanvas.gameObject.SetActive(true);
			//multiplyRateText.text = $"x{multiplyRateCriterion[multiplyRate]}";

			var camStopperPos = Vector3.zero;
			Instantiate(_confettiPref, camStopperPos + new Vector3(-4, 0, 5), Quaternion.Euler(-45, 90, 0));
			Instantiate(_confettiPref, camStopperPos + new Vector3(4, 0, 5), Quaternion.Euler(-45, -90, 0));

			_stages[_multiplyRate].GetComponent<Renderer>().material.DOColor(Color.white, 1).SetLoops(-1, LoopType.Yoyo);

		}

		void OnGameOver()
		{
			_snowBall.Stop();
			//gameOverCanvas.retryButtonAction += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			//gameOverCanvas.gameObject.SetActive(true);
		}

		void OnResult()
		{
			//bowlingCanvas.gameObject.SetActive(false);

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

		void SetCamera()
		{
			_mainCamera.SetTarget(_snowBall.gameObject);
			_mainCamera.StartChase();
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


		public enum GameProgressState1
		{
			BeforeStart,
			Going,
			AfterGoal,
			Result,
			GameOver,
			nothing,
		}
		[System.Serializable]
		public class GameProgressStateReactiveProperty1 : ReactiveProperty<GameProgressState1>
		{
			public GameProgressStateReactiveProperty1() { }
			public GameProgressStateReactiveProperty1(GameProgressState1 initialValue) : base(initialValue) { }
		}
	}

}