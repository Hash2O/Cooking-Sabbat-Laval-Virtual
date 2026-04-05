using UnityEngine;

public class AudioRelay : MonoBehaviour
{
    // Fonction publique pour pouvoir être appelée par le Socket
    public void PlaySoundFromManager(int soundIndex)
    {
        // On vérifie que le Singleton existe bien pour éviter les erreurs
        if (AudioManager.audioInstance != null)
        {
            AudioManager.audioInstance.PlayNotificationSound(soundIndex);
        }
        else
        {
            Debug.LogWarning("AudioManager introuvable dans cette scène !");
        }
    }
}