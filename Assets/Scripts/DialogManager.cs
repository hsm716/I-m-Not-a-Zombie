using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TalkDataManager talkManager;
    public GameObject talkPanel;
    public Text TalkText;
    public GameObject scanObject;

    public GameObject startMessage;

    public bool isAction;
    public int talkIndex;
    void Awake()
    {
        isAction = true;
        Action(startMessage);
    }
    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjData objData = scanObject.GetComponent<ObjData>();
        objData.first_Check = true;
        Talk(objData.ID, objData.isNPC);
        talkPanel.SetActive(isAction);

    }
    void Talk(int id,bool isNpc)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);

        if(talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            return;
        }
        if (isNpc)
        {
            TalkText.text = talkData;
        }
        else
        {
            TalkText.text = talkData;
        }
        isAction = true;
        talkIndex++;
    }
}
