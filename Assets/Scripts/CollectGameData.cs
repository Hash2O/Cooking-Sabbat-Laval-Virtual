using Unity.XR.CoreUtils;
using UnityEngine;

public class CollectGameData : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CoinTriggerDoor coinInPocket;
    [SerializeField] private PumpkinCounter pumpkinCount;

    public void DataGathering()
    {
        Debug.Log("Position : " + playerTransform.position);
        Debug.Log("Nombre de pièces : " + coinInPocket.currentCoinsInTrigger);
        Debug.Log("Nombre de citrouilles : " + pumpkinCount.satisfiedClients);
    }
}
