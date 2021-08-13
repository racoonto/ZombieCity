using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1초 동안프로그레스바 0부터 1(100%)까지 이동 시키기
/// 프로그레스바가 100 % 이동한 뒤에 화면 페이드 아웃 시키면서 뒤에 있는 화면 보여주기
/// </summary>
public class LoadingUI : MonoBehaviour
{
    private RectTransform progressBar;
    private Text progressText;
    public int progressBarMaxWidth = 274;

    internal void ShowProgress(AsyncOperation progress)
    {
        DontDestroyOnLoad(transform.root);
        gameObject.SetActive(true);
        StartCoroutine(ShowProgressCo(progress));
    }

    private IEnumerator ShowProgressCo(AsyncOperation progress)
    {
        //1초 동안프로그레스바 0부터 1(100 %)까지 이동 시키기
        progressBar = transform.Find("ProgressBar").GetComponent<RectTransform>();
        progressText = transform.Find("ProgressText").GetComponent<Text>();

        Vector2 size = progressBar.sizeDelta;
        while (progress.progress < 1)
        {
            float progressPercent = progress.progress; // 0 ~ 1
            size.x = Mathf.RoundToInt(progressBarMaxWidth * progressPercent);
            progressBar.sizeDelta = size;
            progressText.text = Mathf.RoundToInt(progressPercent * 100) + "%";
            yield return null;
        }

        size.x = progressBarMaxWidth;
        progressText.text = "100%";
        progressBar.sizeDelta = size;

        //FadeOut CanvasGroup;
        float endTime = Time.time + 1;
        while (Time.time < endTime)
        {
            GetComponent<CanvasGroup>().alpha = endTime - Time.time;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}