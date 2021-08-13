using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public LoadingUI loadingUI;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            var progress = SceneManager.LoadSceneAsync("Stage1");
            loadingUI.ShowProgress(progress);
        }
    }
}