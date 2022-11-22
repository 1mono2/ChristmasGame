using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using DG.Tweening;

namespace MoNo.Christmas
{
	public class ChaseCamera : MonoBehaviour
	{
		GameObject target;
		Vector3 diff = Vector3.zero;
		IDisposable disposableChase;

		private void Start()
		{

		}

		public void StartChase()
		{
			if (!target) { Debug.LogAssertion("Set target"); return; }
			disposableChase?.Dispose();

			disposableChase =  this.UpdateAsObservable()
				.Subscribe(_ =>
				{
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
			if(diff == Vector3.zero) diff = this.gameObject.transform.position - target.transform.position;
			Debug.Log(diff);
		}

	}
}
