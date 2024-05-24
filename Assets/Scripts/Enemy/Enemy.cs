using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : HitableObject
{
    //定数宣言
    private float _destroy_time = 100f;     //消失までの時間
    //変数宣言
    private int _attack_counter = 0;    //攻撃時間の計測
    private float _destroy_counter = 0f;     //消失までの時間
    private float _attackSpeed = 0f;
    //プロパティ宣言
    // private Animator _animator;      //アニメーターの取得
    private bool _isAttackHit = false;
    [SerializeField, Tooltip("初期HP")] private int _initHp = 100;
    [SerializeField, Tooltip("移動速度")] private float _move_speed = 1f;
    [SerializeField, Tooltip("旋回性能")] private float _roll_speed = 0.05f;
    [SerializeField, Tooltip("攻撃力")] private float _attack_power = 30f;
    [SerializeField, Tooltip("大体の攻撃時間")] private int _attack_end_time = 180; 
    [SerializeField, Tooltip("攻撃開始時間")] private int _hitStartTime = 60;
    [SerializeField, Tooltip("攻撃終了時間")] private int _hitEndTime = 80;
    [SerializeField, Tooltip("コントローラー")] private List<Enemy_blade> _hitBoxList;
    [SerializeField] private GameObject _currentHP_gauge ;   //残りHP   
    [SerializeField, Tooltip("ニメーター")] private Animator _animator = default; 
    [SerializeField, Tooltip("")] private Transform _child = null;
    [SerializeField, Tooltip("")] private Rigidbody _rigidBody = null;
    [SerializeField, Tooltip("")] private int _attackChargeStartTIme = 0;
    [SerializeField, Tooltip("")] private float _attackInitSpeed = 0f;
    [SerializeField, Tooltip("")] private float _attackAcc = 0f;
    [SerializeField, Tooltip("")] private List<GameObject> _attackTrailList = default;
    [SerializeField, Tooltip("")] private float _initPosY = 0f;
    // //LookAtを使いたいなら必要
    // [SerializeField] private Transform _self;
    // [SerializeField] private Transform _target;

    //[SerializeField] private Animator HumanoidAnimator;


    // Start is called before the first frame update
    private void Start()
    {
        Initialize(_initHp);
        // _rigidBody = GetComponent<Rigidbody>();          //汎用
        // _animator = GetComponent<Animator>();       //アニメーターの取得
        _destroy_counter = _destroy_time; //カウンターの初期化

        for(int i = 0; i < _hitBoxList.Count; i++)
        {
            _hitBoxList[i].SetEnemy(this);
            _hitBoxList[i].BoxColliderEnabled(false);
            // _hitBoxList[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _walk_1();
        _HP_gauge_UI();
        //Debug.Log(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }

    public void _walk_1()
    {
        if(_currentHP <= 0 )
        return;
        
        //攻撃中でなく、索敵範囲にプレイヤーがいるなら
        //if(_attack_counter == 0f && col.gameObject.tag == "Player" )
        if(_attack_counter == 0 )
        {
            //プレイヤーと自分の位置の差を、方向に変換
            Vector3 relativePos = PlayData.player.transform.position - this.transform.position;
            //上下の差は無いことにする
            relativePos.y = 0;
            //方向を、回転情報に変換
            Quaternion _rotasion = Quaternion.LookRotation(relativePos);
            //今向いている方向から、向きたい方向に、時間をかけて振り向く
            transform.rotation = Quaternion.Slerp(this.transform.rotation,_rotasion,_roll_speed);

            ////一瞬で振り向く
            //_self.LookAt(_target);

            //前に移動し続ける
            // _rigidBody.AddForce(transform.forward * _move_speed,ForceMode.VelocityChange);

            _rigidBody.velocity = transform.forward * _move_speed;

            _attack_counter = 0;
            _animator.SetBool("Attack",false);
            AttackEnd();
        }
        //攻撃中なら
        else
        {
            //攻撃時間カウンターを増やす
            _attack_counter += 1;
            //Debug.Log(_attack_counter);
            if(_hitStartTime <= _attack_counter && _attack_counter <= _hitEndTime && !_isAttackHit )
            {
                for(int i = 0; i < _hitBoxList.Count; i++)
                {
                    // _hitBoxList[i].gameObject.SetActive(true);
                    _hitBoxList[i].BoxColliderEnabled(true);
                }
            }
            else
            {
                for(int i = 0; i < _hitBoxList.Count; i++)
                {
                    // _hitBoxList[i].gameObject.SetActive(false);
                    _hitBoxList[i].BoxColliderEnabled(false);
                }
            }
            _rigidBody.angularVelocity = Vector3.zero;

            // 攻撃時突進
            if( _attackChargeStartTIme <= _attack_counter )
            {
                if(_attack_counter == _attackChargeStartTIme)
                    _attackSpeed = _attackInitSpeed;
                
                // 初速と同じ向きに少しでも突進するなら突進する力を加える
                if( 0 < _attackSpeed * _attackInitSpeed )
                    _rigidBody.velocity = transform.forward * _attackSpeed;
                else
                    _rigidBody.velocity = Vector3.zero;

                _attackSpeed += _attackAcc;
            }
            else
                _rigidBody.velocity = Vector3.zero;
        }

        //攻撃してから時間がたったら
        if(_attack_counter > _attack_end_time)
        {
            _attack_counter = 0;
            _isAttackHit = false;
            _rigidBody.angularVelocity = Vector3.zero;
            AttackEnd();
        }
    }

    public float GetInitPosY(){ return _initPosY; }

    //トリガーにあたった処理
    private void OnTriggerStay(Collider col)
    {
        //トリガーに当たっており、攻撃中でないなら
        if(col.gameObject.tag == "Player" && _attack_counter == 0)
        //if(_attack_counter == 0f)
        {
            //攻撃アニメーションをさせる
            _animator.SetBool("Attack",true);
            //カウンターが１～180の間はカウント
            _attack_counter += 1;
            for(int i = 0; i < _attackTrailList.Count; i++ )
                _attackTrailList[i].SetActive(true);
        }

        //攻撃してから時間がたったら
        if(_attack_counter > _attack_end_time)
        {
            AttackEnd();
        }
    }

    //攻撃が当たったプレイヤーをぶっとばす（武器のスプリクトから呼び出される関数）
    public void Player_blow_away(Rigidbody _hit_target)
    {
        _hit_target.AddForce(new Vector3(0, _attack_power / 5, 0) ,ForceMode.VelocityChange);
        _hit_target.AddForce(transform.forward * _attack_power / 3 ,ForceMode.Impulse);
    }

    public float GetAttackCounter(){ return _attack_counter; }
    public void AttackHit()
    { 
        _isAttackHit = true; 
        PlayData.player.AttackHit(_attack_power);
        Player_blow_away(PlayData.player.GetRigidBody());
    }


     //HPゲージと死亡の処理
    public void _HP_gauge_UI()
    {
        // if(Input.GetKey("w"))                                      //↑を押したときの処理
        // //if(Input.GetKey(KeyCode.Keypad5))
        //     _currentHP += 10f ;
        // if(Input.GetKey("s"))                                    //↓を押したときの処理
        // //if(Input.GetKey(KeyCode.Keypad2))
        //     _currentHP -= 10f ;

        //HPの現在値と最大値の割り合いを、ゲージの大きさに反映させる
        _currentHP_gauge.transform.localScale = new Vector3(_currentHP / _maxHP ,1 ,1);

        //HPが０以下の処理
        if(_currentHP <= 0)
        {
            //HPを0にする
            _currentHP = 0;
            //消失までカウントダウンする
            _destroy_counter -= 1f; 
            Debug.Log(_destroy_counter);

            this.transform.localScale = new Vector3(transform.localScale.x*0.9f , transform.localScale.y*0.9f , transform.localScale.z*0.9f ); 
        }

        //死んだ瞬間の処理
        if(_destroy_counter == _destroy_time - 1f )
        {
            // //プレイヤーと自分の位置の差を、方向に変換
            // Vector3 relativePos = PlayData.player.transform.position - this.transform.position;
            // relativePos.y = 0;
            // //方向を、回転情報に変換
            // Quaternion _rotasion = Quaternion.LookRotation(relativePos);
            // //今向いている方向から、向きたい方向に、時間をかけて振り向く
            // transform.rotation = Quaternion.Slerp(this.transform.rotation,_rotasion,1);
            //後ろにぶっ飛ぶ
            //_rigidBody.AddForce(transform.forward * -100,ForceMode.VelocityChange),ForceMode.Acceleration;
            _rigidBody.AddForce(new Vector3( 0 , 20f, -300f),ForceMode.VelocityChange);
        }

        //消失の処理
        if(_destroy_counter < 0)
        {
            Destroy(this.gameObject);
            // PlayData.player._enemy_destory_count();
        }
    }

    public void AttackEnd()
    {
        for(int i = 0; i < _attackTrailList.Count; i++ )
            _attackTrailList[i].SetActive(false);
        if(_child != null)
        {
            _child.localPosition = Vector3.zero;
            _child.localEulerAngles = Vector3.zero;
        }

        _attack_counter = 0;
        _isAttackHit = false; 
        _rigidBody.angularVelocity = Vector3.zero;
    }
}
