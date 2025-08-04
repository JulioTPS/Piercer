using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isActive = false;
    private float collisionMinPitch = 0.4f;
    private float collisionMaxPitch = 0.5f;
    private float grindingMinPitch = 0.5f;
    private float grindingMaxPitch = 0.51f;
    private float currentGrindingVolume = 0f;

    private AudioSource grindingAudioSource = null;

    private void Update() { }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];
        float pitch = UnityEngine.Random.Range(collisionMinPitch, collisionMaxPitch);
        float volume = Mathf.InverseLerp(
            3f,
            600f,
            contactPoint.impulse.magnitude / Time.fixedDeltaTime
        );
        // Debug.Log(
        //     $"Collision impulse: {contactPoint.impulse.magnitude / Time.fixedDeltaTime}, Volume: {volume}"
        // );
        SoundFXManager.Instance.PlaySFX("PieceHit", contactPoint.point, volume, pitch);
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];

        float slidingSpeed = Vector3
            .ProjectOnPlane(collision.relativeVelocity, contactPoint.normal)
            .magnitude;
        float targetVolume = 0f;
        float targetPitch = Random.Range(grindingMinPitch, grindingMaxPitch);

        if (slidingSpeed > 0.01f)
        {
            targetVolume = Mathf.InverseLerp(0.01f, 10f, slidingSpeed);
            // Debug.Log($"Sliding speed: {slidingSpeed:F2} m/s, Volume: {volume:F2}");
        }

        currentGrindingVolume = Mathf.Lerp(
            currentGrindingVolume,
            targetVolume,
            Time.fixedDeltaTime * 10f
        );

        if (grindingAudioSource == null)
        {
            grindingAudioSource = SoundFXManager.Instance.PlayContinuousSound(
                "Grinding",
                currentGrindingVolume,
                targetPitch,
                contactPoint.point,
                grindingAudioSource
            );
        }
        else
        {
            SoundFXManager.Instance.PlayContinuousSound(
                "Grinding",
                currentGrindingVolume,
                targetPitch,
                contactPoint.point,
                grindingAudioSource
            );
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        SoundFXManager.Instance.StopContinuousSound(grindingAudioSource);
        grindingAudioSource = null;
    }

    void OnDestroy()
    {
        if (grindingAudioSource != null)
        {
            SoundFXManager.Instance.StopContinuousSound(grindingAudioSource);
            grindingAudioSource = null;
        }
    }
}
