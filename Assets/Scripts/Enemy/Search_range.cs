using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search_range : MonoBehaviour
{
    [SerializeField, Tooltip("コントローラー")] private Enemy _main_body;

    private Rigidbody _hit_target;

    // Start is called before the first frame update
    void Start()
    {
        _main_body = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player" )
        {
            //当たったもののリジッドボディを取得する。
            _hit_target = col.transform.gameObject.GetComponent<Rigidbody>();
            //本体の中の、プレイヤーに近づく処理を呼び出す。
        }
    }
}
