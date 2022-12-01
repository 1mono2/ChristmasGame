using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MoNo.Christmas
{
	public class MoveHorizonral : MonoBehaviour
	{
		[SerializeField] Renderer _targetRender;
		[SerializeField] float movePower = 1.0f;
		[SerializeField] float meter = 4f;

		void Start()
		{

			_targetRender.OnBecameVisibleAsObservable()
				.Take(1)
				.Subscribe(_ =>
				{
					MoveStart();
				})
				.AddTo(this);

		}

		void MoveStart()
		{
			Vector3 initPos = this.transform.position;
			Vector3 dir = Vector3.right;
			this.FixedUpdateAsObservable()
				.Subscribe(_ =>
				{
					Vector3 diff = this.transform.position - initPos;
					if (diff.x >= meter)
					{
						dir = Vector3.left;
					}
					else if (-meter > diff.x)
					{
						dir = Vector3.right;
					}

					this.transform.position += movePower * Time.fixedDeltaTime * dir;
				}).AddTo(this);
		}
	}
}