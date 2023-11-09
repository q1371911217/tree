using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public enum EGameState
{
    WAIT,
    GAMING,
    SHOOTING,
    END,
}

public class Record
{
    string time;
    string mul;
    float win;
    string mul2;
    public Record(string time, string mul, float win, string mul2)
    {
        this.time = time;
        this.mul = mul;
        this.win = win;
        if(Mathf.FloorToInt(this.win) + 0.1 > win)
        {
            this.win = Mathf.FloorToInt(this.win);
        }
        this.mul2 = mul2;
    }

    public string getTime()
    {
        return time;
    }

    public string getMul()
    {
        return mul;
    }

    public float getWin()
    {
        return win;
    }

    public string getMul2()
    {
        return mul2;
    }

}

public class Game : MonoBehaviour
{

    public static bool isLoad = false;

    Dictionary<int, List<float>> mulDic = new Dictionary<int, List<float>>() { };//{ [0] = { 5.0f},[ 1] = { 2.4f, 7.4f }, [2] = { 1.2f, 5f, 26.6f}, [3] = { 1, 2.4f, 8.5f, 37}, [4] = { 1, 1.2f, 3.2f, 12.8f, 100 } };


    List<Record> recordList = new List<Record>();

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    List<AudioClip> clipList;

    [SerializeField]
    List<Gourd> gourdList;

    [SerializeField]
    Text lblCoin;
    [SerializeField]
    Button btnHelp, btnVolum, btnBack, btnLeft, btnRight, btnPlay, btnRandom;

    [SerializeField]
    Transform svContent, svMulContent;
    [SerializeField]
    GameObject itemPref, mulItemPrefab;

    [SerializeField]
    GameObject bgGoal, bgNoGoal;
    [SerializeField]
    Text lblGoal;

    [SerializeField]
    InputField inputBet;

    [SerializeField]
    Text txtWin;
    [SerializeField]
    GameObject helpLayer;

    [SerializeField]
    Transform giftTrans;
    [SerializeField]
    Text lblGiftCount;

    [SerializeField]
    GameObject bonusLayer;
    [SerializeField]
    Text lblBonus;

    [SerializeField]
    List<Transform> mulTransList;


    EGameState gameState;

    bool volumOpen = true;

    float curCoin = 10000.00f;
    int curBet = 100;  //25-800

    int selectCount = 0;
    int systemCount = 0;
    int giftCount = 0;

    int totalGiftCount = 0;

    private void Start()
    {
        gameState = EGameState.WAIT;
       
        lblCoin.text = string.Format("{0:N2}", curCoin);
        inputBet.text = curBet.ToString();

        //{[0] = { 5.0f},[ 1] = { 2.4f, 7.4f }, [2] = { 1.2f, 5f, 26.6f}, [3] = { 1, 2.4f, 8.5f, 37}, [4] = { 1, 1.2f, 3.2f, 12.8f, 100 } };
        mulDic.Add(1, new List<float>() { 5.0f });
        mulDic.Add(2, new List<float>() { 2.4f, 7.4f });
        mulDic.Add(3, new List<float>() { 1.2f, 5f, 26.6f });
        mulDic.Add(4, new List<float>() { 1, 2.4f, 8.5f, 37 });
        mulDic.Add(5, new List<float>() { 1, 1.2f, 3.2f, 12.8f, 100 });


        foreach (Gourd gourd in gourdList)
        {
            gourd.transform.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                audioSource.PlayOneShot(clipList[0]);
                if (selectCount < 5 || gourd.isSelect())
                {
                    selectCount += gourd.select();
                    updateMulLayer();
                }
                    

            });
        }

        inputBet.onEndEdit.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value))
                value = "100";
            else if (int.Parse(value) > curCoin)
                value = ((int)curCoin).ToString();
            else if(int.Parse(value) < 1)
            {
                value = "1";
            }
            curBet = int.Parse(value);
            inputBet.text = value;
            
        });

        updateBet();
    }

    void updateMulLayer()
    {
        for(int i = 0;i< 5; i++)
        {
            Transform trans;
            trans = mulTransList[i];
            if (i < selectCount)
            {
                trans.gameObject.SetActive(true);
                trans.Find("Text").GetComponent<Text>().text = string.Format("{0}x", mulDic[selectCount][i].ToString());
            }
            else
            {
                trans.gameObject.SetActive(false);
            }
        }
    }

    void updateMoney(float addCoin)
    {
        curCoin += addCoin;
        if (curCoin < 1000)
            curCoin = 10000.00f;
        lblCoin.text = string.Format("{0:N2}", curCoin);
    }

    void updateBet()
    {
        inputBet.text = curBet.ToString();
    }

    void updateRecord()
    {
        for (int i = 0; i < recordList.Count; i++)
        {
            Transform cell;
            if (i < svContent.childCount)
            {
                cell = svContent.GetChild(i);
            }
            else
            {
                cell = GameObject.Instantiate(itemPref).transform;
                cell.gameObject.SetActive(true);
                cell.name = i.ToString();
                cell.SetParent(svContent);
                cell.localPosition = Vector3.zero;
                cell.localRotation = Quaternion.identity;
                cell.localScale = Vector3.one;
            }
            cell.Find("lblTime").GetComponent<Text>().text = recordList[i].getTime();
            cell.Find("lblWin").GetComponent<Text>().text = string.Format("{0:N2}", recordList[i].getWin().ToString());
            cell.Find("bgPower/lblPower").GetComponent<Text>().text = recordList[i].getMul();

            //Transform mulCell;
            //if(i<svMulContent.childCount)
            //{
            //    mulCell = svMulContent.GetChild(i);
            //}
            //else
            //{
            //    mulCell = GameObject.Instantiate(mulItemPrefab).transform;
            //    mulCell.gameObject.SetActive(true);
            //    mulCell.name = i.ToString();
            //    mulCell.SetParent(svMulContent);
            //    mulCell.localPosition = Vector3.zero;
            //    mulCell.localRotation = Quaternion.identity;
            //    mulCell.localScale = Vector3.one;
            //}
            //mulCell.Find("Text").GetComponent<Text>().text = recordList[i].getMul2();
            //mulCell.Find("Image/Text").GetComponent<Text>().text = (i + 1).ToString();
        }
    }

    float mul;
    int sameCount;
    void gameStart()
    {
        foreach (Gourd gourd in gourdList)
        {
            gourd.clearSystem();
        }
        while (systemCount < 5)
        {
            int rand = Random.Range(0, gourdList.Count);
            if (!gourdList[rand].isSystemSelect())
            {
                gourdList[rand].systemSelect();
                systemCount++;
            }
        }

        while(giftCount < 1)
        {
            int giftRand = Random.Range(0, gourdList.Count);
            if (!gourdList[giftRand].isSystemSelect())
            {
                gourdList[giftRand].giftSelect();
                giftCount++;
            }                
        }

        sameCount = 0;
        foreach(Gourd gourd in gourdList)
        {
            if(gourd.isReward())
            {
                sameCount++;
            }
            if(gourd.isGift())
            {
                GameObject newGo = GameObject.Instantiate(gourd.gameObject);
                newGo.transform.SetParent(giftTrans.parent);
                newGo.transform.localPosition = gourd.transform.localPosition;
                newGo.transform.localRotation = Quaternion.identity;
                newGo.transform.localScale = Vector3.one;
                newGo.transform.DOLocalMove(giftTrans.localPosition, 0.5f).OnComplete(() =>
                {
                    GameObject.Destroy(newGo);
                    totalGiftCount++;
                    giftTrans.gameObject.SetActive(true);
                    lblGiftCount.text = totalGiftCount.ToString();
                });
            }
        }
        if(sameCount > 0)
        {
           
            mul = mulDic[selectCount][sameCount - 1];
            float win = mul * curBet;
            bgGoal.gameObject.SetActive(true);
            lblGoal.text = string.Format("WIN {0:N2}", win);
            txtWin.transform.localPosition = new Vector3(-191, -50, 0);
            txtWin.text = string.Format("+{0:N2}", win);
            txtWin.transform.DOLocalMoveY(-20, 1f).OnComplete(() => {
                txtWin.text = "";
            });
            audioSource.PlayOneShot(clipList[1]);
            updateMoney(win);

            if(totalGiftCount < 5)
            {
                Record record = new Record(System.DateTime.Now.ToString("HH:mm:ss"), string.Format("X{0:N2}", mul), win, string.Format("{0:N2}x", mul));
                recordList.Insert(0, record);
                if (recordList.Count > 30)
                {
                    recordList.RemoveAt(recordList.Count - 1);
                }
                updateRecord();
            }
           
        }
        else
        {
            Record record = new Record(System.DateTime.Now.ToString("HH:mm:ss"), "X0", 0, "0x");
            recordList.Insert(0, record);
            if (recordList.Count > 30)
            {
                recordList.RemoveAt(recordList.Count - 1);
            }
            updateRecord();
            bgNoGoal.gameObject.SetActive(true);
        }

        StartCoroutine(settle());
    }

    IEnumerator settle()
    {
        yield return new WaitForSeconds(1);

        bgGoal.SetActive(false);
        bgNoGoal.SetActive(false);

       // yield return new WaitForSeconds(0.5f);
       
        //foreach(Gourd gourd in gourdList)
        //{
        //    gourd.clearSystem();
        //}
        systemCount = 0;
        giftCount = 0;
        
        if (totalGiftCount >= 5)
        {
            float rewardNum = 0;
            if (sameCount == 0)
                rewardNum = curBet * 5;
            else
            {
                rewardNum = curBet * mul * 5;
            }
            bonusLayer.gameObject.SetActive(true);
            lblBonus.text = string.Format("{0:N2}", rewardNum);
            float win = mul * curBet * 5;
            Record record = new Record(System.DateTime.Now.ToString("HH:mm:ss"), string.Format("X{0:N2}", mul * 5), win, string.Format("{0:N2}x", mul * 5));
            recordList.Insert(0, record);
            if (recordList.Count > 30)
            {
                recordList.RemoveAt(recordList.Count - 1);
            }
            updateRecord();

            updateMoney(rewardNum);
            totalGiftCount = 0;
            giftTrans.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
            bonusLayer.gameObject.SetActive(false);
        }

        gameState = EGameState.WAIT;
    }
   
    public void onBtnClick(string name)
    {
        audioSource.PlayOneShot(clipList[0]);
        if (name == "btnHelp")
        {            
            helpLayer.gameObject.SetActive(true);
        }
        else if(name == "btnVolum")
        {
            volumOpen = !volumOpen;
            audioSource.volume = volumOpen ? 1 : 0;
            btnVolum.transform.Find("spDisable").gameObject.SetActive(!volumOpen);
        }
        else if(name == "btnBack")
        {
            SceneManager.LoadSceneAsync("LoginScene");
        }
        else if(name == "btnLeft")
        {
            int tmpBet = curBet / 2;
            if(tmpBet >= 1)
            {
                curBet = tmpBet;
                updateBet();
            }
        }
        else if(name == "btnRight")
        {
            int tmpBet = curBet * 2;
            if (tmpBet <= curCoin)
            {
                curBet = tmpBet;
                updateBet();
            }
        }
        else if(name == "btnPlay")
        {
            if(gameState == EGameState.WAIT)
            {
                if (selectCount < 1)
                    return;

                if(curBet <= curCoin)
                {
                    gameState = EGameState.GAMING;
                    updateMoney(-curBet);
                    gameStart();
                }
                else
                {
                    curBet = Mathf.FloorToInt(curCoin);
                    gameState = EGameState.GAMING;
                    updateMoney(-curBet);
                    inputBet.text = curBet.ToString();
                    gameStart();
                }
            }
        }
        else if(name == "btnRandom")
        {
            if (gameState == EGameState.WAIT)
            {
                selectCount = 0;
                foreach (Gourd gourd in gourdList)
                {
                    gourd.clear();
                }

                while (selectCount < 5)
                {
                    int rand = Random.Range(0, gourdList.Count);
                    if (!gourdList[rand].isSelect())
                    {
                        gourdList[rand].select();
                        selectCount++;
                    }

                }
                updateMulLayer();
            }
        }     

    }

    public void playSound()
    {
        audioSource.PlayOneShot(clipList[0]);
    }
}
