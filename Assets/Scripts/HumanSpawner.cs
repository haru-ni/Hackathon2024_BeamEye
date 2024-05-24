using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    private const int MAX_CIVILIAN_NUM = 150;
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    [SerializeField, Tooltip("Parent")] private Transform _parent;
    [SerializeField, Tooltip("プレハブ")] private Civilian _civilianPrefab;

    private List<Civilian> _civilianist = default;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    // ---------- Unity組込関数 ----------
    // ---------- Public関数 ----------
    public void Initialize()
    {
        for(int i = 0; i < MAX_CIVILIAN_NUM; i++)
        {
            Civilian human = Instantiate(_civilianPrefab);
            float posX = Random.Range(-90f, 150f);
            float posZ = Random.Range(20f, 260f);
            Vector3 setPos = new Vector3(posX, 0, posZ);
            
            Vector3 addPos = setPos - PlayData.player.transform.localPosition;
            addPos = addPos.normalized * 30f;
            setPos += addPos;
            human.transform.localPosition = setPos;

            human.transform.parent = _parent;
        }
    }
    // ---------- Private関数 ----------
}
