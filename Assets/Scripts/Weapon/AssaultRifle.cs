﻿using UnityEngine;
using System.Collections.Generic;

public class AssaultRifle : Weapon
{

    //Speedを大きくしすぎるとみえなくなるから注意
    [SerializeField]
    float Speed = 100;//速度

    BulletCreater bullet_creater_ = null;
    Vector3 origin_pos_ = Vector3.zero;

    [SerializeField]
    GameObject spark_prefab_ = null;


    SoundManager sound_manager_ = null;

    float POWER = 0.0f;

    void Start()
    {
        bullet_creater_ = FindObjectOfType<BulletCreater>();
        var parameter = FindObjectOfType<AssaultRifleParameter>();
        if (parameter == null) return;
        POWER = parameter.GetAttackPower(0);
        sound_manager_ = FindObjectOfType<SoundManager>();
    }

    public override void OnAttack()
    {
        if (shot_count_ <= 0.0f)
        {
            spark_prefab_.SetActive(true);

            shot_count_ = 0.1f;

            if (origin_pos_ == Vector3.zero)
            {
                origin_pos_ = WeaponObject.transform.localPosition;
            }

            var diff = 0.01f;
            var random = new Vector3(Random.Range(-diff, diff), Random.Range(-diff, diff), 0.0f);
            WeaponObject.transform.localPosition = origin_pos_ + random; 
        }
        shot_count_ -= Time.deltaTime;
    }

    public override void OnNotAttack()
    {
        spark_prefab_.SetActive(false);
        shot_count_ = 0.1f;
    }

    public override bool CanShot()
    {
        return shot_count_ <= 0.0f;
    }

    public override IEnumerable<GameObject> CreateBullet()
    {
        List<GameObject> bullets = new List<GameObject>();
        var obj = Instantiate(FindObjectOfType<BulletCreater>().getAssaulutBullet);
        obj.transform.position = gameObject.transform.position;
        obj.transform.Translate(gameObject.transform.forward);
        obj.transform.rotation = gameObject.transform.rotation;
        obj.transform.Rotate(0, -90, 0);
        Vector3 force;

        var reticle_position = Reticle.transform.position;

        var direction = (reticle_position - transform.position).normalized;

        const float diff = 0.03f;

        var random = new Vector3(Random.Range(-diff, diff), Random.Range(-diff, diff), Random.Range(-diff, diff));

        force = (direction + random) * 300;
        obj.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        obj.layer = layer_;

        Destroy(obj, 3.0f);

        obj.GetComponent<BulletPower>().SetPower(POWER);

        bullets.Add(obj);

        sound_manager_.PlaySE(7);

        return bullets;
    }


    float shot_count_ = 0.0f;
}
