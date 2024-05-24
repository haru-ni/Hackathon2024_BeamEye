using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    [SerializeField, Tooltip("Parent")] private int _maxEnemyNum = 150;
    [SerializeField, Tooltip("Parent")] private int _InitEnemyNum = 50;
    [SerializeField, Tooltip("同時スポーン数")] private int _spawn_num = 3;
    [SerializeField, Tooltip("Parent")] private int _minSpawnTime = 10;
    [SerializeField, Tooltip("Parent")] private int _maxSpawnTime = 60;    
    [SerializeField, Tooltip("Parent")] private Transform _parent;
    [SerializeField, Tooltip("プレハブ")] private Enemy _enemyPrefab;
    private List<Enemy> _enemyList = default;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    private int _respornTimer = 0;
    private bool _isInitialize = false;
    // ---------- Unity組込関数 ----------
    public void FixedUpdate()
    {
        if(!_isInitialize)
            Initialize();

        if(_respornTimer <= 0 && _enemyList.Count < _maxEnemyNum)
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
        for(int i = 0; i < _InitEnemyNum; i++)
        {
            EnemyRespawn();
        }
        _isInitialize = true;
    }
    // ---------- Private関数 ----------
    private void EnemyRespawn()
    {
        float posX = Random.Range(-90f, 150f);
        float posZ = Random.Range(20f, 260f);

        for(int i = 0; i < _spawn_num; i++)
        {
            Enemy enemy = Instantiate(_enemyPrefab);

            float addX = Random.Range(-3f, 3f);
            float addZ = Random.Range(-3f, 3f);

            Vector3 setPos = new Vector3(posX + addX, enemy.GetInitPosY(), posZ + addZ);
        
            Vector3 addPos = setPos - PlayData.player.transform.localPosition;
            addPos = addPos.normalized * 30f;
            setPos += addPos;
            enemy.transform.localPosition = setPos;

            enemy.transform.parent = _parent;


            enemy.AddOnDieCallback(()=>
            {
                _enemyList.Remove(enemy);
            });
            _enemyList.Add(enemy);
        }
    }
    private void ResetRespawnTimer()
    {
        _respornTimer = Random.Range(10, 60);
    }
}
