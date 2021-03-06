﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerModer : NetworkBehaviour
{
    [SerializeField]
    GameObject effect_ = null;

    [SerializeField]
    GameObject melee_manager_ = null;

    [SerializeField]
    GameObject right_weapon_ = null;

    [SerializeField]
    GameObject leftt_weapon_ = null;

    Vector3 hit_position_ = Vector3.zero;
    PlayerMode player_mode_ = PlayerMode.NORMAL;
    PlayerMeleeAttacker player_melee_attacker_ = null;
    Rigidbody rigidbody_ = null;
    float MELEE_DISTANCE = 7.0f;
    int ENEMY_HASH = 0;

    float melee_time_ = 0.0f;

    public bool isNormalMode
    {
        get
        {
            return player_mode_ == PlayerMode.NORMAL;
        }
    }

    public bool isMeleeMode
    {
        get
        {
            return player_mode_ == PlayerMode.MELEE;
        }
    }

    public override void OnStartLocalPlayer()
    {
        ENEMY_HASH = "Enemy".GetHashCode();
        player_melee_attacker_ = GetComponent<PlayerMeleeAttacker>();
        rigidbody_ = GetComponent<Rigidbody>();
        melee_manager_.SetActive(false);
        base.OnStartLocalPlayer();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (!isMeleeMode) return;

        melee_time_ += Time.deltaTime;

        var position = new Vector3(hit_position_.x, transform.position.y, hit_position_.z);
        transform.position = position;

        if (!(player_melee_attacker_.isFiveAttacked || melee_time_ >= 3.0f)) return;

        melee_time_ = 0.0f;
        rigidbody_.AddForce(-transform.forward * 50.0f, ForceMode.Impulse);
        player_mode_ = PlayerMode.NORMAL;
        melee_manager_.SetActive(false);
        right_weapon_.SetActive(true);
        leftt_weapon_.SetActive(true);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.GetHashCode() != ENEMY_HASH) return;

        var enemy_stater = collider.gameObject.GetComponent<EnemyStater>();
        if (!enemy_stater.isNormal) return;

        var enemy_hp_manager = collider.gameObject.GetComponent<HPManager>();
        if (!enemy_hp_manager.isActive) return;

        if (player_mode_ != PlayerMode.NORMAL) return;

        enemy_stater.SendHitPlayer(transform);
        var direction = new Vector3(transform.forward.x, 0, transform.forward.z);
        hit_position_ = collider.gameObject.transform.position + direction * MELEE_DISTANCE;
        player_mode_ = PlayerMode.MELEE;
        melee_manager_.SetActive(true);
        right_weapon_.SetActive(false);
        leftt_weapon_.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            var effect = Instantiate(effect_);
            var position = transform.position;
            position.y += 2.0f;
            effect.transform.position = position;
            Destroy(effect, 4.0f);
        }

        rigidbody_.velocity = Vector3.zero;
        rigidbody_.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.GetHashCode() != ENEMY_HASH) return;

        var enemy_stater = collision.gameObject.GetComponent<EnemyStater>();
        if (!enemy_stater.isNormal) return;

        var enemy_hp_manager = collision.gameObject.GetComponent<HPManager>();
        if (!enemy_hp_manager.isActive) return;

        if (player_mode_ != PlayerMode.NORMAL) return;

        enemy_stater.SendHitPlayer(transform);
        var direction = new Vector3(transform.forward.x, 0, transform.forward.z);
        hit_position_ = collision.gameObject.transform.position + direction * MELEE_DISTANCE;
        player_mode_ = PlayerMode.MELEE;
        melee_manager_.SetActive(true);
        right_weapon_.SetActive(false);
        leftt_weapon_.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            var effect = Instantiate(effect_);
            var position = transform.position;
            position.y += 2.0f;
            effect.transform.position = position;
            Destroy(effect, 4.0f);
        }

        rigidbody_.velocity = Vector3.zero;
        rigidbody_.angularVelocity = Vector3.zero;
    }
}
