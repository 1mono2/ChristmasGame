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
	public class SnowBallBehavior : MonoBehaviour
	{
		public SphereCollider Collider => sphereCollider;
		[SerializeField] SphereCollider sphereCollider;
		[SerializeField] new Rigidbody rigidbody;
		[SerializeField] LeanMultiUpdate lean;
		[Header("Move property")]
		[SerializeField] float _damping = 10f;
		[SerializeField] float _horizontalMoveSpeed = 0.2f;
		[SerializeField] float _moveSpeed = 15f;
		Vector3 _remainingDelta = Vector3.zero;
		IDisposable _disposableMove;
		[SerializeField] Vector2 _endPoint = new(-4, 4);
		[SerializeField] float sphereRotateSpeed = 10f;
		[SerializeField] float _jumpPower = 1.0f;
		

		[Header("Radius property")]
		readonly ReactiveProperty<float> _radius = new(1f);
		public IReadOnlyReactiveProperty<float> Radius => _radius;
		const float RadiusCriterion = 0.25f;
		const float BallSizeIncreaseUnit = 2.0f;
		const float BallSizeDecreaseUnit = -2.5f;

		[Header("Effect")]
		[SerializeField] ParticleSystem meltingSnowPref;
		[SerializeField] ParticleSystem breakSnowPref;

		[HideInInspector] public UnityEvent OnDisapearEvent => _onDisapearEvent;
		readonly UnityEvent _onDisapearEvent = new();



		void Start()
		{
			lean.OnDelta.AddListener(MoveHorizontal);
			_radius.Value = this.transform.localScale.x / 2;
			// if it make the snowball smaller than the criterion, the snowball is destroyed.
			_radius
				.Where(radius => radius < RadiusCriterion)
				.Subscribe(_ =>
				{
					var breakSnow = Instantiate(breakSnowPref, this.transform.position, Quaternion.identity);
					breakSnow.Play();
					OnDisapear();
				}).AddTo(this);

		}

		public void Move()
		{
			_disposableMove?.Dispose();
			_disposableMove = this.FixedUpdateAsObservable()
								.Subscribe(_ =>
								{
									FixedUpdatePosition();
								});
		}

		public void Stop()
		{
			_disposableMove?.Dispose();
		}

		public void ResetSpeed()
		{
			_remainingDelta = Vector3.zero;
		}


		private void FixedUpdatePosition()
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

			RotateBall(deltaDelta);
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
		
		void RotateBall(Vector3 delta)
		{

			//this.transform.Rotate(Vector3.right, sphereRotateSpeed * Time.fixedDeltaTime);
			var axis = Vector3.Cross(delta, Vector3.down); //  find axis from direcition using Cross()
			this.transform.Rotate(axis, sphereRotateSpeed * Time.fixedDeltaTime, Space.World);
		}

		public void UpSize()
		{
			ChangeSphereSize(BallSizeIncreaseUnit);
			var meltingSnowPos = this.transform.position + _radius.Value * Vector3.down;
			var meltingSnow = Instantiate(meltingSnowPref, meltingSnowPos, Quaternion.identity);
			meltingSnow.Play();
		}

		public void DownSize()
		{
			ChangeSphereSize(BallSizeDecreaseUnit);
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
			Transform thisTransform = transform;
			float multiplyTime = unit * Time.fixedDeltaTime;
			thisTransform.DOBlendableScaleBy(Vector3.one * multiplyTime, 0);
			_radius.Value = thisTransform.localScale.x / 2;
		}

		public void Jump()
		{
			rigidbody.AddForce(new Vector3(0, 1, -0.5f) * _jumpPower, ForceMode.Force);
		}

		public void OnDisapear()
		{
			_onDisapearEvent.Invoke();
			_onDisapearEvent.RemoveAllListeners();
			this.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			this.transform.DOKill();
			Debug.Log("Destroy: " + gameObject.name);
		}

	}
}