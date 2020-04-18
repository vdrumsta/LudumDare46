﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // When collide with this layer, player loses health
    public LayerMask damageObjectLayerMask;
    public List<GameObject> healthHearts;
    [HideInInspector] public int currentHearts;

    public bool immune;
    public List<Material> playerMaterials;
    public float flashInterval = 0.1f;

    private int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = healthHearts.Count;
        currentHearts = healthHearts.Count;
        GhostPlayerMaterials(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if other object layer is part of damage object layer mask
        if (damageObjectLayerMask == (damageObjectLayerMask | (1 << other.gameObject.layer)))
        {
            DecreaseHealth();
        }
    }

    private void IncreaseHealth()
    {
        if (currentHearts >= maxHealth) return;

        healthHearts[currentHearts].SetActive(true);
        currentHearts++;
    }

    private void DecreaseHealth()
    {
        if (currentHearts <= 0 || immune) return;

        healthHearts[currentHearts - 1].SetActive(false);
        currentHearts--;

        immune = true;
        StartCoroutine(RemoveImmunityAfterTime(1));

        if (currentHearts <= 0)
        {
            // Die();
        }
    }

    IEnumerator RemoveImmunityAfterTime(float seconds)
    {
        float timeCounter = 0;

        while (timeCounter < seconds)
        {
            GhostPlayerMaterials(true);
            yield return new WaitForSeconds(flashInterval);
            GhostPlayerMaterials(false);
            yield return new WaitForSeconds(flashInterval);

            timeCounter += flashInterval * 2;
        }

        immune = false;
    }

    private void GhostPlayerMaterials(bool transparent)
    {
        foreach(var material in playerMaterials)
        {
            var currentColor = material.color;

            if (transparent)
            {
                currentColor.a = 0.5f;
            }
            else
            {
                currentColor.a = 1f;
            }
            material.color = currentColor;
        }
    }
}
