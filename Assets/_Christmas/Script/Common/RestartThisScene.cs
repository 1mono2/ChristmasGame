using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class RestartThisScene : MonoBehaviour
{

    void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.R))
            .Subscribe(_ => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }).AddTo(this);
    }

}
