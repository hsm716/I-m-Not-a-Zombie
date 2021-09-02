using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameCamera;
    public GameObject menuCamera;
    public GameObject minimapCamera;
    public GameObject minimapObj;
    public Text killTxt;
    public Text hpTxt;
    public Text zombiePowerTxt;
    public Text timeTxt;
    public Text scoreTxt;
    public Text curLocTxt;
    public Text weaponDegree;

    public Image zombiePowerBar;
    public Image hpBar;

    public Image skill1_img;
    public Image skill2_img;

    public Player player;

    public int killNum;

    public float playTime;
    public int playTime_i;
    public bool isFight;
    public GameObject menuPanel;
    public GameObject gamePanel;

    public Image EquipImage;
    public Sprite weapon1;
    public Sprite weapon2;
    public Sprite weapon3;
    public Text leftBullet;
    public GameObject blocks;


    public Text LevelText;

    public Text EventMSG;
    int eventShowCount=0;

    //백신재료 얻은 개수
    public int count = 0;

    public Image[] VACCINE = new Image[7];

    //maxScore
    public Text maxScore_text;

    //pause 메뉴
    public bool isPause = false;
    public GameObject pauseMenu;
    //Death 메뉴
    public bool isDeath = false;
    public GameObject deathMenu;
    //Clear 메뉴
    public bool isClear = false;
    public GameObject clearMenu;

    public GameObject helpMenu;

    public Text result_text1;
    public Text result_text2;

    public GameObject clearZone;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        for (int i = 0; i < 7; i++)
        {
            Color col = VACCINE[i].color;
            col.a = 0.0f;
            VACCINE[i].color = col;
        }
        count = 0;
        maxScore_text.text = ""+PlayerPrefs.GetInt("maxScore");
    }
    public void GameStart()
    {
        minimapObj.SetActive(true);
        gameCamera.SetActive(true);
        minimapCamera.SetActive(true);
        menuCamera.SetActive(false);


        gamePanel.SetActive(true);
        menuPanel.SetActive(false);
        for(int i = 0; i < 7; i++)
        {
            Color col = VACCINE[i].color;
            col.a = 0.3f;
            VACCINE[i].color = col;
        }
        isFight = true;
    }
    public void GameExit()
    {
        Application.Quit();
    }
    void Update()
    {
        if (isFight)
        {
            playTime += Time.deltaTime;
            playTime_i = (int)playTime;
        }
    }
    void LateUpdate()
    {
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        timeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        hpTxt.text = player.curHealth + " / " + player.maxHealth;
        zombiePowerTxt.text = (int)player.curZombiePower + " / " + player.maxZombiePower;

        hpBar.fillAmount= player.curHealth / (float)player.maxHealth;
        zombiePowerBar.fillAmount= player.curZombiePower / 100f;
        

        killTxt.text = "x" + killNum.ToString();

        scoreTxt.text = string.Format("{0:0000000}", player.score);

        curLocTxt.text = player.curLocation;

        skill1_img.fillAmount = player.skillDelay / 5f;
        skill2_img.fillAmount = player.skillDelay2 / 10f;
        if (player.compactZombie)
        {
            clearZone.SetActive(false);
        }
        if (player.isDeath==true)
        {
            
            isDeath = true;
            isFight = false;
            deathMenu.SetActive(true);
            Time.timeScale = 0f;
            if (player.score >= PlayerPrefs.GetInt("maxScore"))
            {
                PlayerPrefs.SetInt("maxScore", player.score);
            }
            result_text1.text = "최종 스코어 : " + player.score;
        }
        if (player.isClear == true)
        {
            
            isClear = true;
            isFight = false;
            clearMenu.SetActive(true);
            Time.timeScale = 0f;
            if (player.score >= PlayerPrefs.GetInt("maxScore"))
            {
                PlayerPrefs.SetInt("maxScore", player.score);
            }
            result_text2.text = "최종 스코어 : " + player.score;
        }
        if (player.isPause)
        {
            Time.timeScale = 0f;
            isPause = true;
            isFight = false;
            pauseMenu.SetActive(true);
        }
        else if(player.isPause==false&&isDeath==false&&isClear==false)
        {
            Time.timeScale = 1f;
            isFight = true;
            isPause = false;
            pauseMenu.SetActive(false);
        }
       

        if (player.weaponState == 0)
            weaponDegree.text = "";
        else
        {
            weaponDegree.text = player.curdurability_degree[player.weaponState - 1] + "%";
            if (player.curdurability_degree[player.weaponState - 1] >= 70)
                weaponDegree.color = new Color(0f, 255f, 0f);
            else if(player.curdurability_degree[player.weaponState - 1] >= 30)
                weaponDegree.color = new Color(255f, 255f, 0f);
            else
                weaponDegree.color = new Color(255f, 0f, 0f);

        }
        count = 0;
        for (int i = 0; i < 7; i++) {
            if (player.vaccines[i]==true) {
                Color col = VACCINE[i].color;
                col.a= 1.0f;
                VACCINE[i].color = col;
                count++;
            }
        }
        if (count == 7)
        {
            blocks.SetActive(false);
            
        }

        switch (player.weaponState)
        {
            case 0:
                EquipImage.sprite = weapon1;
                leftBullet.text = "- / -";
                break;
            case 1:
                EquipImage.sprite = weapon2;
                leftBullet.text = "- / -";
                break;
            case 2:
                EquipImage.sprite = weapon3;
                leftBullet.text = "" + player.bullet_mount;
                break;

        }
        switch (player.zombie_level)
        {
            case 1:
                player.hpGenAmount = 2f;
                skill1_img.enabled = false;
                skill2_img.enabled = false;
                LevelText.text = "Lv1";
                break;
            case 2:
                player.hpGenAmount = 3f;
                skill1_img.enabled = true;
                LevelText.text = "Lv2";
                break;
            case 3:
                player.hpGenAmount = 4f;
                LevelText.text = "Lv3";
                break;
            case 4:
                player.hpGenAmount = 5f;
                skill2_img.enabled = true;
                LevelText.text = "Lv4";
                break;
            case 5:
                player.hpGenAmount = 6f;
                LevelText.text = "Lv5";
                break;
            case 6:
                player.hpGenAmount = 7f;
                LevelText.text = "Lv6";
                break;
            case 7:
                player.hpGenAmount = 8f;
                player.transform.localScale= new Vector3(transform.localScale.x * 2, transform.localScale.y * 2, transform.localScale.z * 2);
                LevelText.text = "Lv MAX";
                break;
        }
    }
    public void play_btn()
    {
        player.isPause = false;
    }
    public void retry_btn()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void help_btnX()
    {
        helpMenu.SetActive(false);
    }
    public void help_btn()
    {
        helpMenu.SetActive(true);
    }
}
