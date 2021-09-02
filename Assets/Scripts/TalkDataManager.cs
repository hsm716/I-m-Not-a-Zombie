using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkDataManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
        
    }
    void GenerateData()
    {
        talkData.Add(101, new string[] { "눈을 떠보니 나는 숲속에 쓰려져 있었고", "정신은 이상할 정도로 말짱했다.", "그리고..", "몸의 변화가 생겼다는것이 얼마 지나지않아 느껴졌다.", "분명 총을 맞았지만 다친 곳이 없었고,..", "오히려 다치기 전보다 힘이 솟았다.", "나 : ( 우선 주변을 둘러보자.. )" });
        talkData.Add(102, new string[] { "!?!?...", "조....좀비??", "이곳도 역시 위험해.. 이 숲을 나가야겠어" });
        talkData.Add(103, new string[] { "(좀비들 : 크르르... ㄹㄹ....ㄹ..ㅡ)" , "플레이어 : ( 길을 막고있다.. 돌파하자 )", "플레이어 : (지금 힘이면 충분히 이길 것 같아)", "(뒤를 보고 있으니 해치우자!) [ C키 - >공격 ]"});
        talkData.Add(104, new string[] { "플레이어 : 해..해냈다..!! ", "플레이어 : 한시라도 빨리 나가야겠어.." });
        talkData.Add(105, new string[] { "( 뭐야?? 무슨 소리지??)", "( 조..좀비다!! 너무 많아 어서 도망가자 )", "( 저기 다리를 건너서 퇴로를 차단하자 )", "[ Z키 -> 달리기 ] 를 이용하여 빠르게 이동할 수 있습니다." });
    }

    public string GetTalk(int id,int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }

}
