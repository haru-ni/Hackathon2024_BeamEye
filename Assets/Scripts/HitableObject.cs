using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

// 攻撃が当たるオブジェクト
public class HitableObject : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    protected float _maxHP = 100f;
    protected float _currentHP = 100f;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    protected UnityEvent _onDamage = new UnityEvent();
    protected UnityEvent _onDie = new UnityEvent();
    // ---------- Unity組込関数 ----------
    // ---------- Public関数 ----------
    public float GetCurrentHP(){ return _currentHP; }
    public virtual void AttackHit( float damage )
    { 
        Damage(damage);
    }

    public virtual void Damage( float damage )
    {
        _currentHP -= damage; 
        _onDamage!.Invoke();

        if(_currentHP <= 0 )
        {
            _onDie!.Invoke();
            _currentHP = 0;
        }
    }

    // ---------- Private関数 ----------
    protected virtual void Initialize(float initHp){ 
        _currentHP = initHp; 
        _maxHP = initHp;
    }
    public virtual void AddOnDamageCallback(Action onDamageCallback ){ _onDamage.AddListener(()=>onDamageCallback()); }
    public virtual void AddOnDieCallback(Action onDieCallback ){ _onDie.AddListener(()=>onDieCallback()); }
}
