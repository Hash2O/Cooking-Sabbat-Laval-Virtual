using UnityEngine;

public class BottlePour : MonoBehaviour
{
    [SerializeField] private ParticleSystem liquidParticles;
    [SerializeField] private Vector3 referenceAxis = Vector3.up; // Axe vertical de référence (par défaut Vector3.up)

    private void Update()
    {
        // On mesure l'angle entre l'axe "haut" de la fiole et la verticale du monde
        float angle = Vector3.Angle(transform.up, referenceAxis);
        Debug.Log("Angle : " + angle);

        // Si la fiole est entre 90 et 180 degrés, on "verse"
        if (angle >= 90f && angle <= 180f)
        {
            if (!liquidParticles.isPlaying)
                liquidParticles.Play();
        }
        else
        {
            if (liquidParticles.isPlaying)
                liquidParticles.Stop();
        }
    }
}

