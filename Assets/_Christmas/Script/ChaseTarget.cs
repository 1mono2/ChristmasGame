using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MoNo.Christmas
{
	public class ChaseTarget : MonoBehaviour
	{
		[SerializeField] GameObject target;
		Vector3 diff = Vector3.zero;
		IDisposable disposableChase;


		public void StartChase()
		{
			if (!target) { Debug.LogAssertion("Set target"); return; }
			disposableChase?.Dispose();

			disposableChase = this.UpdateAsObservable()
				.Subscribe(_ =>
				{
					if (target != null)
						this.gameObject.transform.position = target.transform.position + diff;
				});
		}

		public void StopChase()
		{
			disposableChase?.Dispose();
		}


		public void SetTarget(GameObject target)
		{
			this.target = target;
			if (diff == Vector3.zero) diff = this.gameObject.transform.position - target.transform.position;
		}

	}
}
