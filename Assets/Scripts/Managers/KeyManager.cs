using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    public static KeyManager KeyInstance;

    [System.Serializable]
    public class ManorKey
    {
        public string roomName;
        public GameObject keyPrefab;
        public bool obtained;
    }

    public List<ManorKey> manorKeys;
    public Transform spawnPoint;

    private void Awake()
    {
        if (KeyInstance != null && KeyInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        KeyInstance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void SpawnKey(int dayIndex)
    {
        if (dayIndex >= manorKeys.Count) return;

        var key = manorKeys[dayIndex];

        if (key.obtained) return;

        Instantiate(key.keyPrefab, spawnPoint.position, Quaternion.identity);
        key.obtained = true;

        Debug.Log($"Clé obtenue pour : {key.roomName}");
    }
}

