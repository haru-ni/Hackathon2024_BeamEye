using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_blade : MonoBehaviour
{

    [SerializeField, Tooltip("トリガー")] private BoxCollider _collider;
    private Enemy _main_body;
    public void SetEnemy( Enemy enemy){ _main_body = enemy; }
    public void BoxColliderEnabled( bool isEnabled ){ _collider.enabled = isEnabled; }

    private void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Player" )
        {
            //本体の中の、プレイヤーをぶっ飛ばすスプリクトを呼び出す。
            _main_body.AttackHit();
            // gameObject.SetActive(false);
            BoxColliderEnabled(false);
        }
    }
}
