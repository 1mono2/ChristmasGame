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
		[SerializeField] Axis position = new Axis(true, true, true);
		[SerializeField] Axis rotation = new Axis(true, true, true);
		private Vector3 _diffPosition = Vector3.zero;
		private Quaternion _diffRotation = new();
		private IDisposable _disposableChase;


		public void StartChase()
		{
			if (!target) { Debug.LogAssertion("Set target"); return; }
			_disposableChase?.Dispose();

			_disposableChase = this.UpdateAsObservable()
				.Subscribe(_ =>
				{
					Vector3 nextPosition = Vector3.zero;
					if (target != null)
					{
						
						gameObject.transform.position = target.transform.position + _diffPosition;
					}
				});
		}

		public void StopChase()
		{
			_disposableChase?.Dispose();
		}


		public void SetTarget(GameObject targetObj)
		{
			this.target = targetObj;
			if (_diffPosition == Vector3.zero) _diffPosition = this.gameObject.transform.position - targetObj.transform.position;
		}

	}
	
	[Serializable]
	public struct Axis{
		[SerializeField] private bool _x;
		[SerializeField] private bool _y;
		[SerializeField] private bool _z;

		public bool X => _x;
		public bool Y => _y;
		public bool Z => _z;
		public Axis(bool x, bool y, bool z)
		{
			_x = x;
			_y = y;
			_z = z;
		}
		
		public void SetAxis(bool x, bool y, bool z)
		{
			_x = x;
			_y = y;
			_z = z;
		}
	}
	
}
