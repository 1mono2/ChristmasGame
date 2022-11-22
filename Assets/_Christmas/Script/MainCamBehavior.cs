using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

namespace MoNo.Christmas
{
    public class MainCamBehavior : MonoBehaviour
    {

        [SerializeField] Camera _mainCam;



        IDisposable _disposableChase;
        Tweener _shakeRotationTweener;

        [Header("Make camera shaking")]
        [SerializeField] float _duration = 1.0f;
        [SerializeField] float _strength = 1.0f;
        [SerializeField] int _vibrato = 3;


        // property
        public Camera mainCam => _mainCam;



        void Start()
        {
            GameManager.I.gameProgressState
                .Where(state => state == GameProgressState.Presenting)
                .Subscribe(state =>
                {

                    // move to side position
                    Vector3 diff = Vector3.zero;
                    //mainCam.transform.DOLocalMove(new Vector3(0, 0, 0), 1f)
                    //                    .SetRelative(true);


                    

                });

            this.OnTriggerEnterAsObservable()
                .Where(collider => collider.CompareTag("CamStopper"))
                .Subscribe(_ =>
                {
                    _disposableChase?.Dispose();
                });


        }

        public void ShakeCamera()
        {

            _shakeRotationTweener?.Kill();

            _shakeRotationTweener = mainCam.DOShakeRotation(_duration, _strength, _vibrato)
                                            .SetRelative(true);
        }


        private void OnDestroy()
        {
            _disposableChase?.Dispose();
            _shakeRotationTweener?.Kill();
        }

    }
}
