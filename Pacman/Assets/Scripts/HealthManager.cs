using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartsParent;
    public int maxHearts = 20;
    public int currentHealth = 3;

    private GameObject[] heartObjects;

    void Start()
    {
        InitializeHearts();
        UpdateHearts();
    }

    void InitializeHearts()
    {
        heartObjects = new GameObject[maxHearts];

        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsParent);
            heartObjects[i] = heart;
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < maxHearts; i++)
        {
            heartObjects[i].SetActive(i < currentHealth);
        }
    }

    public void AddHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth + newHealth, 0, maxHearts);
        UpdateHearts();
    }

    public void DecreaseHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth - newHealth, 0, maxHearts);
        UpdateHearts();
    }

}
