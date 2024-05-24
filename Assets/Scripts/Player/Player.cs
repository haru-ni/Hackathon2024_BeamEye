using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : HitableObject
{
    // ---------- 定数宣言 ----------
    private const float INIT_HP = 100f;
    private const float BEAM_POWER = 1.5f;
    // private const float BEAM_POWER = 0f;

    private const float DEFAULT_SPEED = 3.0f;
    private const float DASH_SPEED = 12.0f;
    private const float JUMP_SPEED = 6.0f;
    private const float SELF_HEAL = 0.1f;
    private const float DASH_COST = 0.1f;
    private const float JUMP_COST = 0.5f;
    private const float FIRST_DASH_COST = 5.0f;
    private const float FIRST_JUMP_COST = 5.0f;
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    [SerializeField, Tooltip("Rigidbody")] private Rigidbody _rigidbody;
    [SerializeField, Tooltip("mainCamera")] private Camera _mainCamera;
    [SerializeField, Tooltip("StepUpHighTrigger")] private ChildTrigger _stepUpHighTrigger = default;
    [SerializeField, Tooltip("StepUpLowTrigger")] private ChildTrigger _stepUpLowTrigger = default;
    [SerializeField] private GameObject _rest_tension_gauge ;   //残りテンション
    [SerializeField] private GameObject _grace_tension_gauge ;   //テンションが減った時、一瞬だけ見える部分

    [SerializeField, Tooltip("Beam")] private List<ChildTrigger> _beamAttackTriggerList = default;
    private bool _isSteping = false;   // 段差判定
    private bool _isGrounding = false;   // 着地判定
    private bool _isDash = false;   // ダッシュ判定
    private bool _isJump = false;   // ジャンプ判定
    private int _dashEndTimer = 0;   // ダッシュ終了タイマー
    private int _dashTimer = 0;   // ダッシュタイマー
    private float _graceHp = 100f;     //テンションが減った時、一瞬だけ見える部分の内部数値
    private float _graceHpCounter = 100f;    //攻撃を喰らった後、一瞬だけ見える部分が減り始めるまでの時間
    private Vector3 _lastDashVec = Vector3.zero;
    private int _stopAutoHealTimer = 0; 

    private GameObject _lastHitObject = null;
    private HitableObject _lastHitHitableObject = null;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    // ---------- Unity組込関数 ----------
    void Start()
    {
        Initialize(INIT_HP);
        int stepLayerMask = LayerMaskExtensions.Add(0, "Default");
        _stepUpHighTrigger.SetLayerMask(stepLayerMask);
        _stepUpLowTrigger.SetLayerMask(stepLayerMask);
        _lastDashVec = _mainCamera.transform.forward;

        int beamLayerMask = LayerMaskExtensions.Add(0, "Default");
        beamLayerMask = LayerMaskExtensions.Add(beamLayerMask, "Enemy");
        beamLayerMask = LayerMaskExtensions.Add(beamLayerMask, "Citizen");
        for(int i = 0; i < _beamAttackTriggerList.Count; i++)
        {
            ChildTrigger beamCollision = _beamAttackTriggerList[i];
            Debug.Log("ビーム初期化");
            beamCollision.SetLayerMask(beamLayerMask);
            beamCollision.SetOnParticleCollisionCallback(gameObject=>
            {
                // ビームが当たった対象のhitableObject取得
                HitableObject hitableObject = null;
                if(gameObject == _lastHitObject)
                {
                    hitableObject = _lastHitHitableObject;
                }
                else
                {
                    hitableObject = gameObject.gameObject.GetComponent<HitableObject>();
                    if(hitableObject != null)
                    {
                        _lastHitObject = gameObject;
                        _lastHitHitableObject = hitableObject;
                    }
                }
                if(hitableObject != null)
                    hitableObject.AttackHit(BEAM_POWER);
                Debug.Log("攻撃対象：" + gameObject.gameObject.name);

                // 市民を攻撃してしまった
                if(gameObject.layer == LayerMask.NameToLayer("Citizen"))
                {
                    Damage(20.0f);
                    UpdateStopAutoHealTimer(300);
                }
                // 敵を攻撃したら回復
                if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    _currentHP += _maxHP * 0.2f;
                }
            });
        }
        
    }

    void Update()
    {
        // UpdateAngles();
        UpdateChildAngles();
        CheckGrounding();
        SearchStep();
        DashProccess();
        MoveProccess();
        JumpProccess();
        UpdateTensionGuageUI();
    }

    void FixedUpdate()
    {
        if(_stopAutoHealTimer <= 0)
        {
            // if(_isGrounding)
                _currentHP += SELF_HEAL;
        }
        else
            _stopAutoHealTimer--;
    }
    // ---------- Public関数 ----------
    public Rigidbody GetRigidBody(){ return _rigidbody; }
    public void UpdateStopAutoHealTimer(int timer) { if(_stopAutoHealTimer <= timer ) _stopAutoHealTimer = timer; }
    // ---------- Private関数 ----------
    private void CheckGrounding()
    {
        Ray ray = new Ray(transform.localPosition, new Vector3(0, -1, 0));
        int layerMask = 1 << LayerMask.NameToLayer("Default");
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f, layerMask, QueryTriggerInteraction.Ignore))
        {
            _isGrounding = true;
        }
        else
            _isGrounding = false;
    }
    // ダッシュ処理
    private void DashProccess()
    {
        // 左クリックした時
        if (Input.GetMouseButtonDown(0))
        {
            _isDash = true;
            _rigidbody.useGravity = false;
            // 自傷
            Damage(FIRST_DASH_COST);

            // よじ登り状態でなければ上昇速度を変更
            if(!_isSteping)
            {
                Vector3 newVelocity = _rigidbody.velocity;
                newVelocity.y = JUMP_SPEED * 1.1f;
                if(!_isGrounding)
                    newVelocity.y *= 0.2f;
                    
                _rigidbody.velocity = newVelocity;
            }
            _dashEndTimer = 0;
            _dashTimer = 0;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 newVelocity = _rigidbody.velocity;
            newVelocity.y *= 0.9f;
            if(newVelocity.y <= 0.05f)
                newVelocity.y = 0;
            _rigidbody.velocity = newVelocity;
            // 自傷
            Damage(DASH_COST);

            _dashTimer++;
            UpdateStopAutoHealTimer(30);
        }
        else
        {
            _isDash = false;
            if( _dashEndTimer == 6 )
            {
                _rigidbody.velocity *= 0.9f;
            }
            else if( 7 <= _dashEndTimer &&_dashEndTimer <= 25 )
            {
                Vector3 velocity = _rigidbody.velocity;
                velocity.y *= 0.9f;
                _rigidbody.velocity = velocity;
            }
            else if(!_isSteping)
            {
                _rigidbody.useGravity = true;
            }
            _dashEndTimer++;
            if(1000 < _dashEndTimer || _isJump)
                _dashEndTimer = 1000;
        }
    }
    // 移動処理
    private void MoveProccess()
    {
        Vector3 horizontalVelocity = new Vector3( _rigidbody.velocity.x, 0, _rigidbody.velocity.z );

        Vector3 velocity = _rigidbody.velocity;
        Vector3 newVelocity = Vector3.zero;

        int moveFB = 0;
        int moveLR = 0;
        float speed = DEFAULT_SPEED; 
        if(_isDash)
            speed = DASH_SPEED;

        // Wキー（前方移動）
        if (Input.GetKey(KeyCode.W) || Input.GetKey("up"))
            moveFB += 1;
 
        // Sキー（後方移動）
        if (Input.GetKey(KeyCode.S) || Input.GetKey("down"))
            moveFB -= 1;
 
        // Dキー（右移動）
        if (Input.GetKey(KeyCode.D) || Input.GetKey("right"))
            moveLR += 1;
 
        // Aキー（左移動）
        if (Input.GetKey(KeyCode.A) || Input.GetKey("left"))
            moveLR -= 1;

        // 移動ボタンを押しているなら移動ベクトル上書き
        if(moveFB != 0 || moveLR != 0 || _isDash)
        {
            if(moveFB != 0 || moveLR != 0)
            {
                Vector3 velocityFB = _mainCamera.transform.forward * moveFB;
                Vector3 velocityLR = _mainCamera.transform.right * moveLR;

                newVelocity.x = velocityFB.x + velocityLR.x;
                newVelocity.z = velocityFB.z + velocityLR.z;
                if(_isDash)
                    _lastDashVec = newVelocity;
            }
            // ダッシュ中、方向キーを入力してないなら最後に指定した角度に走る
            else
            {
                newVelocity = _lastDashVec;
            }
            newVelocity = NormalizedEx(newVelocity) * speed;
            
            if( _isGrounding || _isDash)
            {
                newVelocity.y = velocity.y;
                _rigidbody.velocity = newVelocity;
            }
            // 自由落下中はAddforceで弱くかける。減速する方向に近いほど強くかかる
            else
            {  
                float maxSpd = DASH_SPEED * 0.7f;

                // 1:加速する,-1:減速する
                float col = (HorizontalNormalizedEx(newVelocity) + HorizontalNormalizedEx(_rigidbody.velocity)).magnitude - 1;
                // 現在速度が高いほど影響度が高い
                float col2 = HorizontalVec(_rigidbody.velocity).magnitude / maxSpd;
                col = 1 - col * col2;
                _rigidbody.AddForce( newVelocity * 1.2f * col, ForceMode.Force);
            }
        }
        // 移動せず接地しているなら水平移動を止める
        else if( _isGrounding )
        {
            velocity.x *= 0.2f;
            velocity.z *= 0.2f;

            if(velocity.x <= 0.01f)
                velocity.x = 0f;
            if(velocity.z <= 0.01f)
                velocity.z = 0f;
            _rigidbody.velocity = velocity;
            _lastDashVec = _mainCamera.transform.forward;
        }
    }

    private void JumpProccess()
    {
        Vector3 newVelocity = Vector3.zero;


        // 右クリック（上昇）
        if (Input.GetMouseButtonDown(1) && !_isDash)
        {
            Damage(FIRST_JUMP_COST); 
        }

        // 右クリック（上昇）
        if (Input.GetMouseButton(1) && !_isDash)
        {
            newVelocity = _rigidbody.velocity;
            newVelocity.y = 1.0f * JUMP_SPEED;
            _rigidbody.velocity = newVelocity;
            _isJump = true;
            Damage(JUMP_COST);
            UpdateStopAutoHealTimer(30);
        }
        else
            _isJump = false;
    }

    // 段差検知（高低2つのレイで判定する）
    private void SearchStep()
    {
        bool isLow = false;
        bool isHigh = false;
        if(0 < _stepUpLowTrigger.NumOfStayCollider())
            isLow = true;
        if(0 < _stepUpHighTrigger.NumOfStayCollider())
            isHigh = true;

        // 低いレイが障害物を検知して、高いほうが検知しないなら段差がある
        // 段差を上る
        float stepUpMinSpeed = 3f;
        if(isLow && !isHigh)
        {
            Vector3 newVelocity = _rigidbody.velocity;
            newVelocity.y = stepUpMinSpeed;
            _rigidbody.velocity = newVelocity;
            _rigidbody.useGravity = false;
            _isSteping = true;
        }
        else
        {
            if(!_isDash)
                _rigidbody.useGravity = true;
            if(_isSteping)
            {
                Vector3 newVelocity = _rigidbody.velocity;
                newVelocity.y /= 5;
                _rigidbody.velocity = newVelocity;
            }
            _isSteping = false;
        }
    }

    // 水平の前面
    private Vector3 HorizontalForward()
    {
        Vector3 ret = Vector3.zero;
        Vector3 forward = _mainCamera.transform.forward;
        // Vector3 forward = transform.forward;
        ret.x = forward.x;
        ret.z = forward.z;
        return NormalizedEx(ret);
    }
    // 水平のカメラ向き
    private void UpdateChildAngles()
    {
        Vector3 angle = new Vector3(0, _mainCamera.transform.localEulerAngles.y ,0);
        
        _stepUpLowTrigger.transform.localEulerAngles = angle;
        _stepUpHighTrigger.transform.localEulerAngles = angle;
    }
    // 水平の向き更新
    // private void UpdateAngles()
    // {
    //     Vector3 angle = transform.localEulerAngles;
    //     Vector3 cameraAngle = _mainCamera.transform.localEulerAngles;

    //     angle.y += _mainCamera.transform.localEulerAngles.y;
    //     transform.localEulerAngles = angle;
    //     cameraAngle.y = 0;
    //     _mainCamera.transform.localEulerAngles = cameraAngle;
    // }
    private Vector3 HorizontalVec(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }
    private Vector3 NormalizedEx(Vector3 vec)
    {
        float len = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
        if(len <= 0.01f)
            return Vector3.zero;
        return new Vector3(vec.x / len, vec.y / len, vec.z / len);
    }
    private Vector3 HorizontalNormalizedEx(Vector3 vec)
    {
        return NormalizedEx(HorizontalVec(vec));
    }

    private void UpdateTensionGuageUI()
    {
        //テンションの現在値と最大値の割り合いを、ゲージの大きさに反映させる
        _rest_tension_gauge.transform.localScale = new Vector3(_currentHP / _maxHP ,1 ,1);

        //テンションが最大値以上なら最大値にする
        if(_currentHP > _maxHP )
        {
            _currentHP = _maxHP;
        }

        //一瞬だけ見える部分の処理
        if(_currentHP < _graceHp)
        {
            _graceHp = _graceHp - 0.25f ;
            _grace_tension_gauge.transform.localScale = new Vector3(_graceHp / _maxHP ,1 ,1);
        }
        else
        {
            _graceHp = _currentHP;
        }
    }
}

