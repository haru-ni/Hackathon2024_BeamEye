using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : HitableObject
{
    [SerializeField, Tooltip("初期HP")] private int _initHp = 100;
    [SerializeField, Tooltip("初期HP")] private Rigidbody _rigidbody = null;

    private float _move_speed = 5f;     //移動速度

    private float _rool_speed = 1f;  //旋回性能

    private float _ramdam = 3;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(_initHp);
        AddOnDieCallback(()=>
        {
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _run();
    }

    private void _run()
    {
        // //プレイヤーと自分の位置の差を、方向に変換
        // Vector3 relativePos = _player.transform.position - this.transform.position;
        // //上下の差は無いことにする
        // relativePos.y = 0;
        // //方向を、回転情報に変換
        // Quaternion _rotasion = Quaternion.LookRotation(relativePos);

        // //今向いている方向から、向きたい方向に、時間をかけて振り向く
        // transform.rotation = Quaternion.Slerp(this.transform.rotation,_rotasion,_rool_speed);

        // //逃げる方向を向く
        // transform.Rotate(new Vector3(0,180,0)); 

        transform.rotation = Quaternion.Euler(0,transform.localEulerAngles.y,0);

        //2文で、向いている方向（ローカルの正面方向）へvelocity。
        Vector3 _vec33 = new Vector3(0, 0, 10);
        _rigidbody.velocity = transform.TransformDirection(_vec33);


        if(_ramdam <= 0)
        {
            _ramdam = 3f;
        }
        else
        {
            _ramdam -= 1f;
        }
    }

    //private void OnTriggerStay(Collision col)
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag != null)
        {
            if(_ramdam <= 2)
            {
                //transform.Rotate(new Vector3(0,90,0)); 
                transform.rotation = Quaternion.Euler(0,transform.localEulerAngles.y + 120,0);
            }
            else
            {
                //transform.Rotate(new Vector3(0,-90,0)); 
                transform.rotation = Quaternion.Euler(0,transform.localEulerAngles.y - 120,0);
            }
        }
        
    }
}
