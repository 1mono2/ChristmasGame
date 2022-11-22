using UnityEngine;
using Lean.Touch;
using System;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System.Collections.Generic;
using MoNo.Utility;
using UnityEngine.PlayerLoop;
using System.Linq;

namespace MoNo.Christmas
{
	public class PlayerBehavior : MonoBehaviour
	{
		[SerializeField] SnowBallBehavior snowBallPref;
		[SerializeField] SnowBallBehavior[] snowBalls;
		[SerializeField] ChaseCamera cam;

		ChainSnowBall chain;

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

			chain = new ChainSnowBall(snowBalls);
			chain.SnowBalls
				.ObserveCountChanged()
				.Subscribe(count =>
				{
					SetCamera();
				});

			SetCamera();

		}

		public void SpawnSnowBall()
		{
			var spawnPos = snowBalls[chain.SnowBalls.Count - 1].transform.position + Vector3.back;
			var spawnedSnowball = Instantiate(snowBallPref, spawnPos, Quaternion.identity, this.transform.parent);
			chain.Append(spawnedSnowball);
		}

		void SetCamera()
		{
			var latestIndex = chain.SnowBalls.Count - 1;
			var headSnowBall = chain.SnowBalls[latestIndex];
			cam?.SetTarget(headSnowBall.gameObject);
			cam?.StartChase();
		}


		public void Move()
		{
			disposableMove?.Dispose();
			disposableMove = this.FixedUpdateAsObservable()
								.Subscribe(_ => {
									FixedUpdatePosition();

									chain.FixedUpdate();

								});
		}

		public void Stop()
		{
			disposableMove?.Dispose();
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
			var horizontalDelta = Vector3.right * magnitude.x * _horizontalMoveSpeed;
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
		public IReadOnlyReactiveCollection<SnowBallBehavior> SnowBalls => snowBalls;
		ReactiveCollection<SnowBallBehavior> snowBalls = new ReactiveCollection<SnowBallBehavior>();
		readonly List<Vector3> finalDeltaPosList = new List<Vector3>() {Vector3.zero }; //  enter initialized value

		int frameDiff = 5;

		public ChainSnowBall(IEnumerable<SnowBallBehavior> snowBalls)
		{
			foreach(var snowBall in snowBalls)
			{
				Append(snowBall);

			}

			// fixed postion before every update.
			FixedUpdate();

		}

		public ChainSnowBall(){}


		public void FixedUpdate()
		{
			int frame = 1;
			for(int i = snowBalls.Count -1; i >= 0 ; i--) // Reverse
			{
				Vector3 deltaPos;
				if (finalDeltaPosList.Count - frame >= 0)
				{
					deltaPos = finalDeltaPosList[finalDeltaPosList.Count - frame];
				}
				else if(finalDeltaPosList.Any())
				{
					deltaPos = finalDeltaPosList[0];
				}
				else
				{
					deltaPos = Vector3.zero;
				}
				snowBalls[i].FixedUpdatePosition(deltaPos);

				// update
				frame += frameDiff;
			}
		}

		public void Append(SnowBallBehavior snowBall)
		{
			snowBall.OnDestroyAsync
				.Subscribe(_ =>
				{
					snowBalls.Remove(snowBall);
				});
			snowBalls.Add(snowBall);
		}

		public void EffectAll()
		{

		}

		public void AddToPositionList(Vector3 vector3)
		{
			if (finalDeltaPosList.Count > 100) finalDeltaPosList.Pop();
			finalDeltaPosList.Add(vector3);
		}

	}
}
