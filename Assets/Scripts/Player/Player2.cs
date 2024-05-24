using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2 : MonoBehaviour
{
    private float _walkSpeed = 70f;//歩行速度

    private Rigidbody RBODY;          //プレイヤー制御用Rigidbody

    private float _max_tension = 100f;     //テンションの最大値
    private float _rest_tension = 100f;     //テンションの現在値
    private float _grace_tension = 100f;     //テンションが減った時、一瞬だけ見える部分の内部数値
    private float _grace_tension_counter = 100f;    //攻撃を喰らった後、一瞬だけ見える部分が減り始めるまでの時間

    [SerializeField] private GameObject _rest_tension_gauge ;   //残りテンション
    [SerializeField] private GameObject _grace_tension_gauge ;   //テンションが減った時、一瞬だけ見える部分

    private int _enemy_maximum_quantity = 10;  //敵の最大数
    private int _enemy_rest_quantity = 1;     //敵の現在数
    private int _enemy_respawn_time = 10;       //敵のリスポーン時間
    public GameObject _enemy_prefab;                //敵のプレハブ

    

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;           //ゲームのフレームレート


        RBODY = GetComponent<Rigidbody>();          //汎用
    }

    // Update is called once per frame
    void Update()
    {
        // Waik();
        tension_gauge_UI();
    }

    // public void Waik()
    // {
    //     float speed = _walkSpeed;
    //     Vector3 pos = transform.localPosition;

    //     int command_fb = 0;    // 前後判定 1:後, 0:なし, -1:前
    //     int command_lr = 0;    // 左右判定 1:左, 0:なし, -1:右
    //     // int command_u = 0;    // 上下判定 1:上, 0:なし, -1:下

    //     if(Input.GetKey("up"))                                      //↑を押したときの処理
    //         command_fb += 1;
    //     if(Input.GetKey("down"))                                    //↓を押したときの処理
    //         command_fb -= 1;
    //     if(Input.GetKey("right"))                                   //→を押したときの処理
    //         command_lr += 1;
    //     if(Input.GetKey("left"))                                    //←を押したときの処理
    //         command_lr -= 1;
    //     if(Input.GetKey("f"))                                    //←を押したときの処理
    //         _create_new_enemy();

    //     var _forceX = (command_lr * speed) + (RBODY.velocity.x * -1f);
    //     var _forceZ = (command_fb * speed) + (RBODY.velocity.z * -1f);
    //     RBODY.AddForce(new Vector3(_forceX, 0, _forceZ),ForceMode.Acceleration);
    // }

    //テンションゲージの処理
    public void tension_gauge_UI()
    {

        //テンションの現在値と最大値の割り合いを、ゲージの大きさに反映させる
        _rest_tension_gauge.transform.localScale = new Vector3(_rest_tension / _max_tension ,1 ,1);

        //テンションが最大値以上なら最大値にする
        if(_rest_tension > _max_tension )
        {
            _rest_tension = _max_tension;
        }

        //テンションが０以下なら０にする
        if(_rest_tension < 0)
        {
            _rest_tension = 0;
        }

        //一瞬だけ見える部分の処理
        if(_rest_tension < _grace_tension)
        {
            _grace_tension = _grace_tension - 0.25f ;
            _grace_tension_gauge.transform.localScale = new Vector3(_grace_tension / _max_tension ,1 ,1);
        }
        else
        {
            _grace_tension = _rest_tension;
        }
    }

    //ダメージを受ける処理（敵の武器のスプリクトから呼び出される関数）
    public void Hit(float _damage)
    {
        _rest_tension = _rest_tension - _damage;

    }

    //敵を作る処理
    public void _create_new_enemy()
    {
        //敵の数が最大数以上なら無視する
        if(_enemy_maximum_quantity <= _enemy_rest_quantity)
            return;

        GameObject enemy = (GameObject)Instantiate(_enemy_prefab);

        _enemy_rest_quantity += 1;
    
    }

    //敵が死んだ時、カウントを減らす（敵のスプリクトがら呼び出される）
    public void _enemy_destory_count()
    {
        _enemy_rest_quantity -= 1;
    }
}
