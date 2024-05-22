using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChildTrigger : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- プロパティ ----------
    private Action<Collider> OnTriggerEnterCallback = null;
    private Action<Collider> OnTriggerExitCallback = null;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    private List<Collider> StaycollidersList = default;
    private int _layerMask = -1;
    // ---------- Unity組込関数 ----------
    private void Start()
    {
        StaycollidersList = new List<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(_layerMask == -1 || LayerMaskExtensions.Contains(_layerMask, other.gameObject.layer))
        {
            if(OnTriggerEnterCallback != null)
                OnTriggerEnterCallback(other);
            StaycollidersList.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(OnTriggerExitCallback != null)
            OnTriggerExitCallback(other);

        StaycollidersList.Remove(other);
    }
    // ---------- Public関数 ----------
    public void SetLayerMask(int layerMask){ layerMask = _layerMask; }
    public void SetOnTriggerEnterCallback(Action<Collider> onTriggerEnterCallback)
    {
        OnTriggerEnterCallback = onTriggerEnterCallback;
    }
    public int NumOfStayCollider(){ return StaycollidersList.Count; }
    // ---------- Private関数 ----------
}
