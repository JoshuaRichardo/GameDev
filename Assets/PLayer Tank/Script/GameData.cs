using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/Tank Data Terpusat")]
public class GameData : ScriptableObject
{
    [Header("Health Data")]
    public int currentHealth = 100;
    public int maxHealth = 100;

    [Header("Map Progress Data")]
    public int currentMapIndex = 0; // <--- PASTIKAN HURUF 'c' DI AWAL ADALAH HURUF KECIL

    public void ResetData()
    {
        currentHealth = maxHealth;
        currentMapIndex = 0;
    }
}