using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour {

    [SerializeField]
    GameObject barGo, btnGo;

    [SerializeField]
    Transform start;

    [SerializeField]
    RectTransform i;
    [SerializeField]
    Text t;

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip clip;


    int progress = 0;

    void Awake()
    {
        if (Game.isLoad)
        {
            barGo.SetActive(false);
            btnGo.SetActive(true);
        }
        else
            StartCoroutine(load());
    }

    IEnumerator load()
    {
        while(progress < 100)
        {
            progress += Random.Range(5, 20);
            if (progress > 100)
                progress = 100;
            float amount = (float)progress / 100;
           // i.fillAmount = amount;
            i.sizeDelta = new Vector2(497.0f * amount, 271);
            t.text = string.Format("{0}%", progress);
            start.transform.localPosition = new Vector3(450 * amount - 250, 7, 0);
            yield return new WaitForSeconds(0.1f);
        }

        barGo.SetActive(false);
        btnGo.SetActive(true);

        Game.isLoad = true;
    }

    public void onStart()
    {
        audioSource.PlayOneShot(clip);
        SceneManager.LoadSceneAsync("GameScene");
    }
}
