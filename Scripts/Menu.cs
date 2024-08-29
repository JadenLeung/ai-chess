using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class Menu : MonoBehaviour
{
    public int test = 0;
    public static string [] Mmedals;
    public static string whitebrain = "user";
    public static string blackbrain = "user";

    public static int fishhelp = 0;
    public TMP_Dropdown whitedrop;
    public TMP_Dropdown blackdrop;
    public Slider slider;
    public TextMeshProUGUI slidertext;
    public TextMeshProUGUI slidertitle;
    public TextMeshProUGUI sliderdesc;
    public string[] values = {"user", "GPT", "gemini", "fish", "random"};

    public List<string> models = new List<string> {"GPT", "gemini"};
    public void PlayGame(){
        SceneManager.LoadSceneAsync(1);
    }
    public static void GoMenu(){
        SceneManager.LoadSceneAsync(0);
    }

    public void showStuff(bool show) {
        if (slider) {
            slider.gameObject.SetActive(show);
            slidertext.gameObject.SetActive(show);
            slidertitle.gameObject.SetActive(show);
            sliderdesc.gameObject.SetActive(show);
        }
    }

    void Start() {
        if (whitedrop) {
            whitedrop.value = Array.IndexOf(values, whitebrain);
            blackdrop.value = Array.IndexOf(values, blackbrain);
            slider.value = fishhelp;
            slidertext.text = fishhelp + "%";
        }
        if (slider) {
            slider.onValueChanged.AddListener((v) => {
                slidertext.text = ((int)v / 5 * 5).ToString() + "%";
                fishhelp = ((int)v / 5 * 5);
            });
        }
    }
   
    void Update(){
        if (models.Contains(whitebrain) || models.Contains(blackbrain)) {
            showStuff(true);
        } else {
            showStuff(false);
        }
        test = Game.cool;
        if(Input.GetKeyDown("space")){ //space
            Debug.Log(fishhelp);
        }

    }
    public static void setMedals(){
        Mmedals = Game.medals;
    }
    public void handleDropW(int index) {
        whitebrain = values[index];
    }
    public void handleDropB(int index) {
        blackbrain = values[index];
    }

    public void handleSlide(int val) {
        fishhelp = val;
    }
}
