using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using UniRx.Diagnostics;

public class SnowBallBehavior : MonoBehaviour
{
	public IReadOnlyReactiveProperty<float> Radius => radius;
	ReactiveProperty<float> radius = new ReactiveProperty<float>(1f);

	public IObservable<Unit> OnDestroyAsync => onDestroyAsync;
	private readonly AsyncSubject<Unit> onDestroyAsync = new AsyncSubject<Unit>();

	[SerializeField] Vector2 _endPoint = new Vector2(-4, 4);
	[SerializeField] float sphereRotateSpeed = 1f;

	const float radiusCriterion = 0.25f;
	const float ballSizeIncreaseUnit = 0.2f;
	const float ballSizeDecreaseUnit = -0.5f;

	void Start()
	{
		radius.Value = this.transform.localScale.x / 2;
		// if it make the snowball smaller than the criterion, the snowball is destroyed.
		radius
			.Where(_radius => _radius < radiusCriterion)
			.Subscribe(_radius =>
			{
				OnDestroy();
			});

		this.OnTriggerEnterAsObservable()
			.Where(collider => collider.CompareTag("Snow"))
			.Subscribe(_ => Debug.Log("Enter Snow Zone"));

		this.OnTriggerEnterAsObservable()
			.Where(collider => collider.CompareTag("Magma"))
			.Subscribe(_ => Debug.Log("Enter Magma Zone"));

		this.OnTriggerStayAsObservable()
			.Where(collider => collider.CompareTag("Snow"))
			.Subscribe(_ => ChangeSphereSize(ballSizeIncreaseUnit));

		this.OnTriggerStayAsObservable()
			.Where(collider => collider.CompareTag("Magma"))
			.Subscribe(_ => ChangeSphereSize(ballSizeDecreaseUnit));

		this.OnTriggerExitAsObservable()
			.Where(collider => collider.CompareTag("Snow"))
			.Subscribe(_ => Debug.Log("Exit Snow Zone"));

		this.OnTriggerExitAsObservable()
			.Where(collider => collider.CompareTag("Magma"))
			.Subscribe(_ => Debug.Log("Exit Magma Zone"));


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

	void RotateBall()
	{

		this.transform.RotateAroundLocal(Vector3.right, sphereRotateSpeed * Time.fixedDeltaTime);
		// var axis = Vector3.Cross(deltaDelta.normalized, Vector3.down); //  find axis from direcition using Cross()
		// this.transform.RotateAroundLocal(dir, sphereRotateSpeed * Time.fixedDeltaTime);
	}

	public void ChangeSphereSize(float unit)
	{
		float multiplyTime = unit * Time.fixedDeltaTime;
		this.transform.DOBlendableScaleBy(new Vector3(multiplyTime, multiplyTime, multiplyTime), 0);
		radius.Value = this.transform.localScale.x / 2;
	}



	private void OnDestroy()
	{
		radius.Dispose();

		onDestroyAsync.OnNext(Unit.Default);
		onDestroyAsync.OnCompleted();
		onDestroyAsync.Dispose();

	}

}
