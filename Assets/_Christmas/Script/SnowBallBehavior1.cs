using System;
using DG.Tweening;
using Lean.Touch;
using MoNo.Utility;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace MoNo.Christmas
{
	public class SnowBallBehavior1 : MonoBehaviour
	{
		public SphereCollider Collider => collider;
		[SerializeField] SphereCollider collider;
		[SerializeField] LeanMultiUpdate lean;
		[Header("Move property")]
		[SerializeField] float _damping = 10f;
		[SerializeField] float _horizontalMoveSpeed = 0.2f;
		[SerializeField] float _moveSpeed = 15f;
		Vector3 _remainingDelta = Vector3.zero;
		IDisposable disposableMove;
		[SerializeField] Vector2 _endPoint = new(-4, 4);
		[SerializeField] float sphereRotateSpeed = 10f;

		[Header("Radius property")]
		readonly ReactiveProperty<float> radius = new(1f);
		public IReadOnlyReactiveProperty<float> Radius => radius;
		const float radiusCriterion = 0.25f;
		const float ballSizeIncreaseUnit = 1.0f;
		const float ballSizeDecreaseUnit = -1.5f;

		[Header("Effect")]
		[SerializeField] ParticleSystem meltingSnowPref;
		[SerializeField] ParticleSystem breakSnowPref;

		[HideInInspector] public UnityEvent OnDestroyEvent => onDestroyEvent;
		readonly UnityEvent onDestroyEvent = new();



		void Start()
		{
			lean.OnDelta.AddListener(MoveHorizontal);
			radius.Value = this.transform.localScale.x / 2;
			// if it make the snowball smaller than the criterion, the snowball is destroyed.
			radius
				.Where(_radius => _radius < radiusCriterion)
				.Subscribe(_radius =>
				{
					var breakSnow = Instantiate(breakSnowPref, this.transform.position, Quaternion.identity);
					breakSnow.Play();
					Destroy(this.gameObject);
				}).AddTo(this);

		}

		public void Move()
		{
			disposableMove?.Dispose();
			disposableMove = this.FixedUpdateAsObservable()
								.Subscribe(_ =>
								{
									FixedUpdatePosition();
								});
		}

		public void Stop()
		{
			disposableMove?.Dispose();
		}

		public void ResetSpeed()
		{
			_remainingDelta = Vector3.zero;
		}



		public void FixedUpdatePosition()
		{
			MoveForward();
			var fact = DampenFactor(_damping, Time.fixedDeltaTime);
			var newDelta = Vector3.Lerp(_remainingDelta, Vector3.zero, fact);
			var deltaDelta = _remainingDelta - newDelta;
			_remainingDelta = newDelta;

			Transform finalTransform = this.transform;
			if (finalTransform.position.x < _endPoint.x && Mathf.Sign(deltaDelta.x) < 0) // left side
			{
				deltaDelta = new Vector3(0, deltaDelta.y, deltaDelta.z);
			}
			else if (finalTransform.position.x > _endPoint.y && Mathf.Sign(deltaDelta.x) > 0)
			{
				deltaDelta = new Vector3(0, deltaDelta.y, deltaDelta.z);
			}
			finalTransform.position += deltaDelta;

			RotateBall();
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

		void RotateBall()
		{

			this.transform.RotateAroundLocal(Vector3.right, sphereRotateSpeed * Time.fixedDeltaTime);
			// var axis = Vector3.Cross(deltaDelta.normalized, Vector3.down); //  find axis from direcition using Cross()
			// this.transform.RotateAroundLocal(dir, sphereRotateSpeed * Time.fixedDeltaTime);
		}

		public void UpSize()
		{
			ChangeSphereSize(ballSizeIncreaseUnit);
			var meltingSnowPos = this.transform.position + radius.Value * Vector3.down;
			var meltingSnow = Instantiate(meltingSnowPref, meltingSnowPos, Quaternion.identity);
			meltingSnow.Play();
		}

		public void DownSize()
		{
			ChangeSphereSize(ballSizeDecreaseUnit);
			var meltingSnow = Instantiate(meltingSnowPref, this.transform.position, Quaternion.identity);
			meltingSnow.Play();
		}
		public void Break()
		{
			ChangeSphereSize(-20);
			var breakSnow = Instantiate(breakSnowPref, this.transform.position, Quaternion.identity);
			breakSnow.Play();
		}

		void ChangeSphereSize(float unit)
		{
			float multiplyTime = unit * Time.fixedDeltaTime;
			this.transform.DOBlendableScaleBy(Vector3.one * multiplyTime, 0);
			radius.Value = this.transform.localScale.x / 2;
		}



		private void OnDestroy()
		{
			this.transform.DOKill();
			onDestroyEvent.Invoke();
			onDestroyEvent.RemoveAllListeners();
			Debug.Log("Destroy: " + gameObject.name);
		}

	}
}