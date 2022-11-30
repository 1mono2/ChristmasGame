using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;

public class SnowBallBehavior : MonoBehaviour
{
	[SerializeField] ParticleSystem meltingSnowPref;
	[SerializeField] ParticleSystem breakSnowPref;

	public IReadOnlyReactiveProperty<float> Radius => radius;

	readonly ReactiveProperty<float> radius = new(1f);

	public IObservable<Unit> OnDestroyAsync => onDestroyAsync;
	private readonly AsyncSubject<Unit> onDestroyAsync = new();

	[SerializeField] Vector2 _endPoint = new(-4, 4);
	[SerializeField] float sphereRotateSpeed = 1f;

	const float radiusCriterion = 0.25f;
	const float ballSizeIncreaseUnit = 1.0f;
	const float ballSizeDecreaseUnit = -1.5f;

	void Start()
	{
		radius.Value = this.transform.localScale.x / 2;
		// if it make the snowball smaller than the criterion, the snowball is destroyed.
		radius
			.Where(_radius => _radius < radiusCriterion)
			.Subscribe(_radius =>
			{
				Destroy(this.gameObject);
			}).AddTo(this);

		// Obstacle
		this.OnCollisionEnterAsObservable()
			.Where(collision => collision.collider.CompareTag("Magma"))
			.Subscribe(_ =>
			{
				Break(-20);
			}).AddTo(this);

		// Snow & Magma
		this.OnTriggerEnterAsObservable()
			.Where(collider => collider.CompareTag("Snow"))
			.Subscribe(_ =>
			{
				Debug.Log("Enter Snow Zone");
			}).AddTo(this);

		this.OnTriggerEnterAsObservable()
			.Where(collider => collider.CompareTag("Magma"))
			.Subscribe(_ =>
			{
				Debug.Log("Enter Magma Zone");
			}).AddTo(this);

		this.OnTriggerStayAsObservable()
			.Where(collider => collider.CompareTag("Snow"))
			.BatchFrame(1, FrameCountType.Update)
			.Subscribe(_ => ChangeSphereSize(ballSizeIncreaseUnit)).AddTo(this);

		this.OnTriggerStayAsObservable()
			.Where(collider => collider.CompareTag("Magma"))
			.BatchFrame(1, FrameCountType.Update)
			.Subscribe(_ => ChangeSphereSize(ballSizeDecreaseUnit)).AddTo(this);

		this.OnTriggerExitAsObservable()
			.Where(collider => collider.CompareTag("Snow"))
			.Subscribe(_ =>
			{
				Debug.Log("Exit Snow Zone");
			}).AddTo(this);

		this.OnTriggerExitAsObservable()
			.Where(collider => collider.CompareTag("Magma"))
			.Subscribe(_ =>
			{
				Debug.Log("Exit Magma Zone");
			}).AddTo(this);

		// Disapper this gameobject, entering the "disappear zone"
		this.OnTriggerEnterAsObservable()
		.Where(collider => collider.CompareTag("Disappear"))
		.Subscribe(_ => Destroy(this.gameObject)).AddTo(this);

	}

	public void FixedUpdatePosition(Vector3 deltaPos)
	{

		// 前のBallの大きさの変更でPositionを変更する挙動は複雑になりそうなのでパス
		Transform finalTransform = this.transform;
		if (finalTransform.position.x < _endPoint.x && Mathf.Sign(deltaPos.x) < 0) // left side
		{
			deltaPos = new Vector3(0, deltaPos.y, deltaPos.z);
		}
		else if (finalTransform.position.x > _endPoint.y && Mathf.Sign(deltaPos.x) > 0)
		{
			deltaPos = new Vector3(0, deltaPos.y, deltaPos.z);
		}
		finalTransform.position += deltaPos;

		RotateBall();
	}

	public void FixedUpdateTransPos(Vector3 transformPos)
	{
		var newPos = new Vector3(transformPos.x, transformPos.y + radius.Value, transformPos.z);
		this.transform.position = newPos;

		RotateBall();
	}

	void RotateBall()
	{

		this.transform.RotateAroundLocal(Vector3.right, sphereRotateSpeed * Time.fixedDeltaTime);
		// var axis = Vector3.Cross(deltaDelta.normalized, Vector3.down); //  find axis from direcition using Cross()
		// this.transform.RotateAroundLocal(dir, sphereRotateSpeed * Time.fixedDeltaTime);
	}

	public void ChangeSphereSize(float unit)
	{
		float multiplyTime = unit * Time.fixedDeltaTime;
		this.transform.DOBlendableScaleBy(Vector3.one * multiplyTime, 0);
		radius.Value = this.transform.localScale.x / 2;
		var meltingSnow = Instantiate(meltingSnowPref, this.transform.position, Quaternion.identity);
		meltingSnow.Play();
	}

	public void Break(float unit)
	{
		float multiplyTime = unit * Time.fixedDeltaTime;
		this.transform.DOBlendableScaleBy(Vector3.one * multiplyTime, 0);
		radius.Value = this.transform.localScale.x / 2;
		var breakSnow = Instantiate(breakSnowPref, this.transform.position, Quaternion.identity);
		breakSnow.Play();
	}


	public void AddPos(Vector3 vector3)
	{
		this.transform.position += vector3;
	}

	private void OnDestroy()
	{
		onDestroyAsync.OnNext(Unit.Default);
		onDestroyAsync.OnCompleted();
		onDestroyAsync.Dispose();
		Debug.Log("Destroy: " + gameObject.name);
	}

}
