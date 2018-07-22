﻿using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public abstract class Unit : MonoBehaviour
{
    public Sprite Portrait;
    public Sprite UnitPortrait
    {
        get { return Portrait; }
    }

    [SerializeField]
    [Range(0, 10)]
    public float movementSpeed;
    public UnitStats Stats = new UnitStats();

    public int ExperienceWorth = 1;

    [SerializeField]
    [Range(10f, 2000f)]
    public float maxHealth;
    private float _health;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health < 0)
            {
                _health = 0;
            }
        }
    }
    public bool IsDead
    {
        get
        {
            return Health <= 0;
        }
    }

    private void Awake()
    {
        Health = maxHealth;
    }

    public float TakeDamage(float damage, GameObject sender, Collider2D collider)
    {
        if (Stats.Status.Contains(Statuses.Invincible))
            return 0f;

        Health -= damage;
        if (damage > 0)
        {
            TextManager.CreateDamageText(damage.ToString(), transform, 0.2f);
        }
        else if (damage < 0)
        {
            TextManager.CreateHealText((-1 * damage).ToString(), transform, 0.2f);
        }
        TakeDamageExtender(damage, sender, collider);

        if (IsDead)
        {
            if (gameObject.tag == Tags.Player)
            {
                bool isReady = PlayerManager.playerReady[gameObject.GetComponent<Player>().playerID];
                if (isReady)
                {
                    PlayerManager.playerReady[gameObject.GetComponent<Player>().playerID] = false;
                    PlayerManager.playersReady -= 1;
                    Rigidbody2D rigid = gameObject.GetComponent<Rigidbody2D>();
                    rigid.velocity = Vector2.zero;
                    gameObject.transform.rotation = Quaternion.Euler(90, 0, 90);
                    rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                    if (PlayerManager.playersReady <= 0)
                    {
                        GameObject manager = GameObject.Find("ManagerManager");
                        CustomGameManager gm = manager.GetComponent<CustomGameManager>();
                        gm.GameOver();
                    }
                }
            }
            else
            {
                var killer = sender.GetComponent<Unit>();
                if (killer != null)
                    killer.Stats.GainExperience(ExperienceWorth);

                // TODO: Refactor this to event ffs
                var spawnyThing = GetComponent<SpawnThingOnDeath>();
                if (spawnyThing != null)
                    spawnyThing.SpawnThing();

                Destroy(gameObject);
                // Play generic death particle & sound?
                ParticleSpawner.instance.SpawnParticleEffect((Vector2)collider.transform.position, (gameObject.transform.position - collider.transform.position).normalized, ParticleSpawner.ParticleTypes.BloodParticles);
            }
        }

        return Health;
    }

    public abstract void TakeDamageExtender(float damage, GameObject sender, Collider2D collision);
}
