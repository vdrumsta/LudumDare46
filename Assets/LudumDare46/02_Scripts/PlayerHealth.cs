using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // When collide with this layer, player loses health
    public LayerMask damageObjectLayerMask;
    public List<GameObject> healthHearts;
    public int currentHearts;
    private int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = healthHearts.Count;
        currentHearts = healthHearts.Count;
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
        if (currentHearts <= 0) return;

        healthHearts[currentHearts - 1].SetActive(false);
        currentHearts--;

        if (currentHearts <= 0)
        {
            // Die();
        }
    }
}
