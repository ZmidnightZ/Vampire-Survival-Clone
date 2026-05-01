using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SaveSystem.LoadAll();
        UIController.instance.UpdateCoins();
    }

    public int currentCoins;

    public CoinPickup coin;

    public void AddCoins(int coinsToAdd)
    {
        currentCoins += coinsToAdd;

        UIController.instance.UpdateCoins();

        SFXManager.instance.PlaySFXPitched(2);
        SaveSystem.SaveAll();
    }

    public void DropCoin(Vector3 position, int value)
    {
        CoinPickup newCoin = Instantiate(coin, position + new Vector3(.2f, .1f, 0f), Quaternion.identity);
        newCoin.coinAmount = value;
        newCoin.gameObject.SetActive(true);
    }

    public void SpendCoins(int coinsToSpend)
    {
        currentCoins -= coinsToSpend;

        if (currentCoins < 0)
        {
            currentCoins = 0;
        }

        UIController.instance.UpdateCoins();
        SaveSystem.SaveAll();
    }
}
