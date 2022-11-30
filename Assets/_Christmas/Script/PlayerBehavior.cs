using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Touch;
using MoNo.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MoNo.Christmas
{
	public class PlayerBehavior : MonoBehaviour
	{
		[SerializeField] SnowBallBehavior snowBallPref;
		[SerializeField] SnowBallBehavior[] snowBalls;
		[SerializeField] ChaseTarget cam;
		[SerializeField] public ChaseTarget triggerCollider;

		public ChainSnowBall chain;

		[Header("Move property")]
		[SerializeField] float _damping = 10f;
		[SerializeField] float _horizontalMoveSpeed = 1.0f;
		[SerializeField] float _moveSpeed = 1.0f;
		Vector3 _remainingDelta = Vector3.zero;
		IDisposable disposableMove;

		[SerializeField] LeanMultiUpdate lean;


		private void Start()
		{
			lean.OnDelta.AddListener(MoveHorizontal);

			chain = new ChainSnowBall(ChainSnowBall.ProcessMode.Delta, snowBalls);
			chain.SnowBalls
				.ObserveCountChanged()
				.Subscribe(count =>
				{
					SetCamera();
					SetTriggerCollider();
				}).AddTo(this);

			SetCamera();
			SetTriggerCollider();

			// chain.mode
			// 	.Where(mode => mode == ChainSnowBall.ProcessMode.Transform)
			// 	.Subscribe(_ =>
			// 	{
			// 		_moveSpeed /= 1.5f;
			// 	}).AddTo(this);

			// chain.mode
			// .Where(mode => mode == ChainSnowBall.ProcessMode.Delta)
			// .Subscribe(_ =>
			// {
			// 	_moveSpeed *= 1.5f;
			// }).AddTo(this);
		}

		public void SpawnSnowBall()
		{
			var spawnPos = chain.SnowBalls[^1].transform.position;
			var spawnedSnowball = Instantiate(snowBallPref, spawnPos, Quaternion.identity, this.transform.parent);
			chain.Append(spawnedSnowball);
		}

		public void DeleteSnowBall()
		{
			Destroy(chain.LatestSnowBall());
		}

		void SetTriggerCollider()
		{
			var latestIndex = chain.SnowBalls.Count - 1;
			if (latestIndex < 0) return;
			var headSnowBall = chain.SnowBalls[latestIndex].gameObject;
			triggerCollider.SetTarget(headSnowBall);
			triggerCollider.StartChase();
		}

		void SetCamera()
		{
			var latestIndex = chain.SnowBalls.Count - 1;
			if (latestIndex < 0) return;
			var headSnowBall = chain.SnowBalls[latestIndex].gameObject;
			cam.SetTarget(headSnowBall);
			cam.StartChase();
		}


		public void Move()
		{
			disposableMove?.Dispose();
			disposableMove = this.FixedUpdateAsObservable()
								.Subscribe(_ =>
								{
									FixedUpdatePosition();

									chain.FixedUpdate();

								});
		}

		public void Stop()
		{
			disposableMove?.Dispose();
		}

		public void ResetDelta()
		{
			_remainingDelta = Vector3.zero;
		}


		void FixedUpdatePosition()
		{
			MoveForward();
			//var finalTransform = this.transform;
			var fact = DampenFactor(_damping, Time.fixedDeltaTime);
			var newDelta = Vector3.Lerp(_remainingDelta, Vector3.zero, fact);
			var deltaDelta = _remainingDelta - newDelta;
			_remainingDelta = newDelta;

			chain.AddToPositionList(deltaDelta);
		}

		void MoveForward()
		{
			_remainingDelta += new Vector3(0, 0, _moveSpeed * Time.fixedDeltaTime);
		}


		void MoveHorizontal(Vector2 magnitude)
		{
			var horizontalDelta = _horizontalMoveSpeed * magnitude.x * Vector3.right;
			_remainingDelta += horizontalDelta;


		}

		static float DampenFactor(float speed, float elapsed)
		{
			if (speed < 0.0f)
			{
				return 1.0f;
			}

#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				return 1.0f;
			}
#endif

			return 1.0f - Mathf.Pow((float)System.Math.E, -speed * elapsed);
		}


	}

	public sealed class ChainSnowBall
	{
		public enum ProcessMode
		{
			Transform,
			Delta,
		}
		public class ProcessModeReactiveProperty : ReactiveProperty<ProcessMode>
		{
			public ProcessModeReactiveProperty() { }
			public ProcessModeReactiveProperty(ProcessMode initialValue) : base(initialValue) { }

		}
		public ProcessModeReactiveProperty mode = new();

		public IReadOnlyReactiveCollection<SnowBallBehavior> SnowBalls => snowBalls;

		readonly ReactiveCollection<SnowBallBehavior> snowBalls = new();
		readonly List<Vector3> deltaPosList = new() { Vector3.zero }; //  enter initialized value
		readonly List<Vector3> firstObjPosList = new() { Vector3.zero };
		readonly int frameDiff = 10;

		public ChainSnowBall(ProcessMode mode, IEnumerable<SnowBallBehavior> snowBalls)
		{
			foreach (var snowBall in snowBalls)
			{
				Append(snowBall);
			}

			SubscribeMode();
			this.mode.Value = mode;

			// fixed postion before every update.
			FixedUpdate();
		}

		public ChainSnowBall(ProcessMode mode)
		{
			SubscribeMode();
			this.mode.Value = mode;
		}


		public void FixedUpdate()
		{
			int frame = 1;

			// controlling first snowball via delta mode restrictly.
			if (deltaPosList.Count - frame >= 0 && snowBalls.Count > 0)
			{
				int index = deltaPosList.Count - 1;
				Vector3 deltaPos = deltaPosList[index];

				SnowBallBehavior latestSnowball = snowBalls[^1];
				latestSnowball.FixedUpdatePosition(deltaPos);
				firstObjPosList.Add(latestSnowball.transform.position - new Vector3(0, latestSnowball.Radius.Value, 0));
				frame += frameDiff;
			}

			// after second snowball
			switch (mode.Value)
			{
				case ProcessMode.Transform:
					for (int i = snowBalls.Count - 2; i >= 0; i--) // Reverse. From foreword snowball
					{
						Vector3 pos;
						if (firstObjPosList.Count - frame >= 0)
						{
							int index = firstObjPosList.Count - frame;
							pos = firstObjPosList[index];
						}
						else if (deltaPosList.Any())
						{
							pos = deltaPosList[0];
						}
						else
						{
							pos = Vector3.zero;
						}
						snowBalls[i].FixedUpdateTransPos(pos);

						// update
						frame += frameDiff;
					}

					break;

				case ProcessMode.Delta:
					for (int i = snowBalls.Count - 2; i >= 0; i--) // Reverse
					{
						Vector3 deltaPos;
						if (deltaPosList.Count - frame >= 0)
						{
							deltaPos = deltaPosList[^frame];
						}
						else if (deltaPosList.Any())
						{
							deltaPos = deltaPosList[0];
						}
						else
						{
							deltaPos = Vector3.zero;
						}
						snowBalls[i].FixedUpdatePosition(deltaPos);

						// update
						frame += frameDiff;
					}
					break;
			}

		}

		public void Append(SnowBallBehavior currentSnowBall)
		{

			// When snowball destroyed, it's removed from Snowballs List
			currentSnowBall.OnDestroyAsync
				.Subscribe(_ =>
				{
					snowBalls.Remove(currentSnowBall);
				}).AddTo(currentSnowBall);
			snowBalls.Add(currentSnowBall);
		}

		public GameObject LatestSnowBall()
		{
			return snowBalls[^1].gameObject;
		}

		public void AddToPositionList(Vector3 vector3)
		{
			if (deltaPosList.Count > 100) deltaPosList.Pop();
			deltaPosList.Add(vector3);
		}

		void SubscribeMode()
		{
			this.mode
			.Where(mode => mode == ProcessMode.Transform)
			.Subscribe(_ => { Debug.Log("Switch Transform mode"); });

			this.mode
				.Where(mode => mode == ProcessMode.Delta)
				.Subscribe(_ => { Debug.Log("Switch Delta mode"); });
		}

	}
}
