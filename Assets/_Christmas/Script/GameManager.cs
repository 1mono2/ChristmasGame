using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
//using GoogleMobileAds.Placement;
using MoNo.Utility;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rando = UnityEngine.Random;

namespace MoNo.Christmas
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		protected override bool DontDestroy => false;

		// property
		public GameProgressStateReactiveProperty gameProgressState1 { get => _gameProgressState;
			set => _gameProgressState = value;
		}


		[Header("Realistic object")]
		[SerializeField] SnowBallBehavior _snowBall;
		[SerializeField] GameObject[] _stages;
		[SerializeField] ParticleSystem _confettiPref;
		[SerializeField] ChaseTarget _mainCamera;

		[Header("System object")]
		// field
		GameProgressStateReactiveProperty _gameProgressState = new(GameProgressState.Nothing);
		SphereCollider _snowBallCollider;

		[Header("UI object")]
		[SerializeField] StartCanvasBehavior _startCanvas;
		[SerializeField] GoingCanvas _goingCanvas;
		[SerializeField] ResultCanvasBehavior _resultCanvas;
		[SerializeField] GameOverCanvasBehavior _gameOverCanvas;


		// hide state
		readonly int _multiplyRate = 0; // 0 ~10
		readonly List<float> _multiplyRateCriterion = new() { 1, 1.2f, 1.4f, 1.6f, 1.8f, 2, 2.2f, 2.4f, 2.6f, 2.8f, 3 }; // 11 rank

		const string SAVE_STAGE_INDEX = "StageIndex";

		protected override void Awake()
		{
			base.Awake();
			if (LoadData.I == null)
			{
				SceneManager.LoadScene("PreLoad");
				return;
			}

		}

		void Start()
		{
			_snowBallCollider = _snowBall.Collider;
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

		void OnBeforeStart()
		{
			_startCanvas.tapToStart.OnFinger.AddListener(leanFinger =>
			{
				_startCanvas.gameObject.SetActive(false);
				_gameProgressState.Value = GameProgressState.Going;
			});

			_startCanvas.levelText.text = $"Level {SceneManager.GetActiveScene().buildIndex}";

			SetCamera();
		}


		void OnGoing()
		{
			_goingCanvas.gameObject.SetActive(true);

			_snowBall.Radius
				.Select(x => x * 2)
				.Subscribe(radius =>
				{
					_goingCanvas.countText.text = radius.ToString("F2");
				}).AddTo(this).AddTo(_snowBall.gameObject);

			_snowBall.OnDisapearEvent.AddListener(() =>
			{
				_gameProgressState.Value = GameProgressState.GameOver;
			});

			// Collision Event
			_snowBallCollider.OnCollisionEnterAsObservable()
			.Where(collision => collision.gameObject.TryGetComponent(out IObstacle obstacle))
				.ThrottleFirstFrame(1)
				.Subscribe(col =>
				{
					if (col.collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnEnterEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnCollisionStayAsObservable()
			.Where(collision => collision.gameObject.TryGetComponent(out IObstacle obstacle))
			.ThrottleFirstFrame(1)
				.Subscribe(col =>
				{
					if (col.collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnStayEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnCollisionExitAsObservable()
				.Where(collision => collision.gameObject.TryGetComponent(out IObstacle obstacle))
				.ThrottleFirstFrame(1)
				.Subscribe(col =>
				{
					if (col.collider.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnExitEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			// Trigger Event
			_snowBallCollider.OnTriggerEnterAsObservable()
				.Where(collider1 => collider1.gameObject.TryGetComponent(out IObstacle obstacle))
				.ThrottleFirstFrame(1)
				.Subscribe(collider1 =>
				{
					if (collider1.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnEnterEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnTriggerStayAsObservable()
			.Where(collider1 => collider1.gameObject.TryGetComponent(out IObstacle obstacle))
				.ThrottleFirstFrame(1)
				.Subscribe(collider1 =>
				{
					if (collider1.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnStayEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnTriggerExitAsObservable()
			.Where(collider1 => collider1.gameObject.TryGetComponent(out IObstacle obstacle))
				.ThrottleFirstFrame(1)
				.Subscribe(collider1 =>
				{
					if (collider1.TryGetComponent(out IObstacle obstacle))
					{
						obstacle.OnExitEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			// Trigger Event
			_snowBallCollider.OnTriggerEnterAsObservable()
				.Subscribe(collider1 =>
				{
					if (collider1.gameObject.TryGetComponent(out IDuplicatableObstacle duplicatable))
					{
						duplicatable.OnEnterEvent(_snowBall);
					}

				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnTriggerStayAsObservable()
				.Subscribe(collider1 =>
				{
					if (collider1.gameObject.TryGetComponent(out IDuplicatableObstacle duplicatable))
					{
						duplicatable.OnStayEvent(_snowBall);
					}
				}).AddTo(this).AddTo(_snowBallCollider);

			_snowBallCollider.OnTriggerExitAsObservable()
			.Subscribe(collider1 =>
			{
				if (collider1.gameObject.TryGetComponent(out IDuplicatableObstacle duplicatable))
				{
					duplicatable.OnExitEvent(_snowBall);
				}
			}).AddTo(this).AddTo(_snowBallCollider);



			// move
			_snowBall.Move();

		}

		void OnGameOver()
		{
			_goingCanvas.gameObject.SetActive(false);
			_snowBall.Stop();
			_gameOverCanvas.retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
			_gameOverCanvas.gameObject.SetActive(true);
		}

		void OnResult()
		{
			_goingCanvas.gameObject.SetActive(false);
			_resultCanvas.gameObject.SetActive(true);

			if (LoadData.I.isShowAd == true)
			{
				
				_resultCanvas.nextLevelButton.onClick.AddListener(ShowAds);
			}
			else
			{
				_resultCanvas.nextLevelButton.onClick.AddListener(NextStage);
			}
			SaveStageIndex();
		}

		void SetCamera()
		{
			_mainCamera.SetTarget(_snowBall.gameObject);
			_mainCamera.StartChase();
		}

		static void SaveStageIndex()
		{
			PlayerPrefs.SetInt(SAVE_STAGE_INDEX, SceneManager.GetActiveScene().buildIndex);
		}

		void ShowAds()
		{
			InterstitialAds.I.OnAdClosed.AddListener(NextStage); 

			InterstitialAds.I.ShowIfLoaded();
			
		}

		private void NextStage()
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

		public enum GameProgressState
		{
			BeforeStart,
			Going,
			Result,
			GameOver,
			Nothing,
		}
		[System.Serializable]
		public class GameProgressStateReactiveProperty : ReactiveProperty<GameProgressState>
		{
			public GameProgressStateReactiveProperty() { }
			public GameProgressStateReactiveProperty(GameProgressState initialValue) : base(initialValue) { }
		}
	}

}