using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite heartEmpty;
    [SerializeField] private Sprite heartHalf;
    [SerializeField] private Sprite heartFull;

    private Image[] hearts;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        hearts = new Image[Mathf.CeilToInt(player.maxHealth / 2)];
        for (int i = 0; i < hearts.Length; i++)
        {
            GameObject heart = Instantiate(heartPrefab, new Vector2(transform.position.x + 50*i, transform.position.y), Quaternion.identity);
            heart.transform.SetParent(transform);
            hearts[i] = heart.GetComponent<Image>();
        }

        UpdateHealth();
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            switch (Math.Clamp(player.currentHealth - i * 2, 0, 2))
            {
                case 0:
                    hearts[i].sprite = heartEmpty;
                    break;
                case 1:
                    hearts[i].sprite = heartHalf;
                    break;
                case 2:
                    hearts[i].sprite = heartFull;
                    break;
            }
        }
    }
}
