using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChildTrigger : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    // [SerializeField, Tooltip("パーティクル")] private ParticleSystem _particleSystem = null;
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    private Action<Collision> _onCollisionEnterCallback = null;
    private Action<Collision> _onCollisionStayCallback = null;
    private Action<Collision> _onCollisionExitCallback = null;
    private Action<Collider> _onTriggerEnterCallback = null;
    private Action<Collider> _onTriggerStayCallback = null;
    private Action<Collider> _onTriggerExitCallback = null;
    private Action<GameObject> _onParticleCollisionCallback = null;

    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    private List<Collider> StaycollidersList = default;
    private int _layerMask = -1;
    // ---------- Unity組込関数 ----------
    private void Start()
    {
        StaycollidersList = new List<Collider>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(_layerMask == -1 || LayerMaskExtensions.Contains(_layerMask, collision.gameObject.layer))
        {
            if(_onCollisionEnterCallback != null)
                _onCollisionEnterCallback(collision);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(_layerMask == -1 || LayerMaskExtensions.Contains(_layerMask, collision.gameObject.layer))
        {
            if(_onCollisionStayCallback != null)
                _onCollisionStayCallback(collision);
        }
    }
    private void OnCollisioExit(Collision collision)
    {
        if(_onCollisionExitCallback != null)
            _onCollisionExitCallback(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_layerMask == -1 || LayerMaskExtensions.Contains(_layerMask, other.gameObject.layer))
        {
            if(_onTriggerEnterCallback != null)
                _onTriggerEnterCallback(other);
            StaycollidersList.Add(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(_layerMask == -1 || LayerMaskExtensions.Contains(_layerMask, other.gameObject.layer))
        {
            if(_onTriggerStayCallback != null)
                _onTriggerStayCallback(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(_onTriggerExitCallback != null)
            _onTriggerExitCallback(other);

        StaycollidersList.Remove(other);
    }
    private void OnParticleCollision(GameObject gameObject)
    {
        if(_onParticleCollisionCallback != null)
            _onParticleCollisionCallback(gameObject);
    }
    // ---------- Public関数 ----------
    public void SetLayerMask(int layerMask){ layerMask = _layerMask; }
    public void SetOnCollisionEnterCallback(Action<Collision> onCollisionEnterCallback)
    {
        _onCollisionEnterCallback = onCollisionEnterCallback;
    }
    public void SetOnCollisionStayCallback(Action<Collision> onCollisionStayCallback)
    {
        _onCollisionStayCallback = onCollisionStayCallback;
    }
    public void SetOnCollisionExitCallback(Action<Collision> onCollisionExitCallback)
    {
        _onCollisionExitCallback = onCollisionExitCallback;
    }
    public void SetOnTriggerEnterCallback(Action<Collider> onTriggerEnterCallback)
    {
        _onTriggerEnterCallback = onTriggerEnterCallback;
    }
    public void SetOnTriggerStayCallback(Action<Collider> onTriggerStayCallback)
    {
        _onTriggerStayCallback = onTriggerStayCallback;
    }
    public void SetOnExitEnterCallback(Action<Collider> onTriggerExitCallback)
    {
        _onTriggerExitCallback = onTriggerExitCallback;
    }
    public void SetOnParticleCollisionCallback(Action<GameObject> onParticleCollisionCallback)
    {
        _onParticleCollisionCallback = onParticleCollisionCallback;
    }
    public int NumOfStayCollider(){ return StaycollidersList.Count; }
    // ---------- Private関数 ----------
}
