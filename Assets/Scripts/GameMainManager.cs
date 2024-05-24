using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainManager : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    [SerializeField, Tooltip("Rigidbody")] private Player _player;
    [SerializeField, Tooltip("HumanSpawner")] private HumanSpawner _humanSpawner;
    [SerializeField, Tooltip("EnemySpawner")] private List<EnemySpawner> _enemySpawnerList;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    // ---------- Unity組込関数 ----------
    void Awake()
    {
        PlayData.Initialize();
        PlayData.player = _player;
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        _humanSpawner.Initialize();
        for(int i = 0; i < _enemySpawnerList.Count; i++)
        {
            _enemySpawnerList[i].Initialize();
        }
    }

    void Update()
    {
        
    }
    // ---------- Public関数 ----------
    // ---------- Private関数 ----------

}
