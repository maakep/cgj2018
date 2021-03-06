﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class Powerup : MonoBehaviour
{

    public PowerupObject powerup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            powerup.HandlePowerup(powerup, player);

            ParticleSpawner.instance.SpawnParticleEffect(gameObject.transform.position, ParticleTypes.YellowPixelExplosion);
            Destroy(gameObject);
        }
    }
}

[Serializable]
public class PowerupObject
{
    [Header("Attributes")]
    [Range(0, 50)]
    public int Agility;
    [Range(0, 50)]
    public int Intelligence;
    [Range(0, 50)]
    public int Strength;

    [Header("Status")]
    public bool ModifyStatus = false;
    public Statuses Status;

    [Header("Weapons")]
    public int WeaponIncrement;

    [Header("Health")]
    [Range(0, 1000)]
    public float Health;
    [Range(0, 1000)]
    public float MaxHealth;

    [Header("Experience")]
    [Range(0, 500)]
    public int Experience;

    [Header("Duration")]
    [Tooltip("Leave unchecked if this is a permanent powerup")]
    public bool ShouldBeTemporary;
    [Range(0, 30)]
    public float Seconds;

    private int _maxProjectilesDamageAmplifier = 10;
    public void HandlePowerup(PowerupObject powerup, Player player)
    {
        if (powerup.ModifyStatus)
        {
            player.Stats.Status.Add(powerup.Status);
        }

        if (powerup.WeaponIncrement > 0)
        {
            if (player.weapon.projectiles + powerup.WeaponIncrement <= player.weapon.maxProjectiles)
            {
                player.weapon.projectiles += powerup.WeaponIncrement;
            }
            else if (player.weapon.projectiles == player.weapon.maxProjectiles)
            {
                powerup.Strength += _maxProjectilesDamageAmplifier;
            }
            else
            {
                powerup.WeaponIncrement = player.weapon.maxProjectiles - player.weapon.projectiles;
                player.weapon.projectiles = player.weapon.maxProjectiles;
            }

        }

        player.Stats.GainExperience(powerup.Experience);

        player.Stats.Agility += powerup.Agility;
        player.Stats.Intelligence += powerup.Intelligence;
        player.Stats.Strength += powerup.Strength;

        player.Health += powerup.Health;
        player.maxHealth += powerup.MaxHealth;

        if (ShouldBeTemporary)
        {
            ParticleSpawner.instance.SpawnParticleEffect(player.transform.position.Offset(y: 0.5f), ParticleTypes.BluePixelTrail_Fewer, lifetime: powerup.Seconds, parent: player.transform);
            player.StartCoroutine(RevertAfterSeconds(powerup.Seconds, powerup, player));
        }
    }

    IEnumerator RevertAfterSeconds(float seconds, PowerupObject powerup, Player player)
    {
        yield return new WaitForSeconds(seconds);
        if (player != null)
        {
            if (powerup.ModifyStatus)
                player.Stats.Status.Remove(powerup.Status);

            if (powerup.WeaponIncrement > 0)
            {
                if (player.weapon.projectiles - powerup.WeaponIncrement >= 1)
                {
                    player.weapon.projectiles -= powerup.WeaponIncrement;
                }
            }

            player.Stats.Agility -= powerup.Agility;
            player.Stats.Intelligence -= powerup.Intelligence;
            player.Stats.Strength -= powerup.Strength;

            player.Health -= powerup.Health;
            player.maxHealth -= powerup.MaxHealth;
        }
    }
}
