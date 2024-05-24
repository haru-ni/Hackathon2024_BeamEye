using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityObj : HitableObject
{
    [SerializeField, Tooltip("初期HP")] private int _initHp = 100;
    [SerializeField, Tooltip("初期HP")] private Rigidbody _rigidbody = null;

    private void Start()
    {
        if(_rigidbody != null)
            _rigidbody.isKinematic = true;
        Initialize(_initHp);
        AddOnDieCallback(()=>
        {
            if(100 <= _initHp || _rigidbody == null)
                Destroy(gameObject);
            else
            {
                _rigidbody.isKinematic = false;
                _rigidbody.AddForce(new Vector3(0, 6, 0) ,ForceMode.VelocityChange);
                _rigidbody.AddForce(PlayData.player.transform.forward * 10 ,ForceMode.Impulse);
            }
        });
    }
}
