using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    private const int MAX_ENEMY_NUM = 150;
    private const int INIT_ENEMY_NUM = 50;
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    [SerializeField, Tooltip("Parent")] private Transform _parent;
    [SerializeField, Tooltip("プレハブ")] private Enemy _enemyPrefab;
    private List<Enemy> _enemyList = default;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    private int _respornTimer = 0;
    // ---------- Unity組込関数 ----------
    public void FixedUpdate()
    {
        if(_respornTimer <= 0 && _enemyList.Count < MAX_ENEMY_NUM)
        {
            EnemyRespawn();
            ResetRespawnTimer();
        }
        _respornTimer--;
    }
    // ---------- Public関数 ----------
    public void Initialize()
    {
        _enemyList = new List<Enemy>();
        for(int i = 0; i < INIT_ENEMY_NUM; i++)
        {
            EnemyRespawn();
        }
    }
    // ---------- Private関数 ----------
    private void EnemyRespawn()
    {
        Enemy enemy = Instantiate(_enemyPrefab);
        float posX = Random.Range(-90f, 150f);
        float posZ = Random.Range(20f, 260f);
        Vector3 setPos = new Vector3(posX, 0, posZ);
        
        Vector3 addPos = setPos - PlayData.player.transform.localPosition;
        addPos = addPos.normalized * 30f;
        setPos += addPos;
        enemy.transform.localPosition = setPos;


        enemy.AddOnDieCallback(()=>
        {
            _enemyList.Remove(enemy);
        });
        _enemyList.Add(enemy);
    }
    private void ResetRespawnTimer()
    {
        _respornTimer = Random.Range(10, 60);
    }
}
