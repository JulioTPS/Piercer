using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isActive = false;
    private const float MIN_PITCH = 0.9f;
    private const float MAX_PITCH = 1.1f;
    private const float GRIND_PITCH = 1f;
    private const float SPATIAL_BLEND = 1f;
    private float currentGrindingVolume = 0f;

    private Rigidbody rb;
    private const float PRESSING_THRESHOLD = 3.1f;
    private const float SNAP_MARGIN = 20f;
    private Coroutine fitPieceCoroutine = null;
    private bool isFittingPiece = false;
    private float fittingTime = 0.3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing on the Piece object.");
        }
    }

    private AudioSource grindingAudioSource = null;

    private void Update() { }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive)
            return;
        ContactPoint contactPoint = collision.contacts[0];
        float volume = Mathf.InverseLerp(
            3f,
            600f,
            Mathf.Sqrt(contactPoint.impulse.magnitude / Time.fixedDeltaTime)
        );
        float pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        // Debug.Log(
        //     $"Collision impulse: {contactPoint.impulse.magnitude / Time.fixedDeltaTime}, Volume: {volume}"
        // );
        SoundFXManager.Instance.PlaySFX("Impact", contactPoint.point, volume, pitch);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isActive)
            return;

        // foreach (ContactPoint contact in collision.contacts)
        // {
        //     float pushDirection = Vector3.Dot(rb.linearVelocity.normalized, contact.normal);
        //     bool isPushingOposite = pushDirection > 0f;
        //     if (
        //         !isPushingOposite
        //         && !isFittingPiece
        //         && contact.impulse.magnitude > PRESSING_THRESHOLD
        //     )
        //     {
        //         Debug.Log("Start fitting piece");
        //         if (fitPieceCoroutine != null)
        //         {
        //             Debug.Log("stop coroutine and then start");
        //             StopCoroutine(fitPieceCoroutine);
        //             fitPieceCoroutine = null;
        //         }
        //         isFittingPiece = true;
        //         fitPieceCoroutine = StartCoroutine(FitPiecePlace());
        //         break;
        //     }
        //     else if (fitPieceCoroutine != null && isPushingOposite)
        //     {
        //         Debug.Log("stop coroutine");
        //         StopCoroutine(fitPieceCoroutine);
        //         fitPieceCoroutine = null;
        //         isFittingPiece = false;
        //         break;
        //     }
        // }

        ContactPoint contactPoint = collision.contacts[0];

        Vector3 velocityAtContact = rb.GetPointVelocity(contactPoint.point);

        float slidingSpeed = Vector3
            .ProjectOnPlane(velocityAtContact, contactPoint.normal)
            .magnitude;
        float targetVolume = 0f;

        if (slidingSpeed > 0.01f)
        {
            float normalizedSpeed = Mathf.InverseLerp(0.01f, 10f, slidingSpeed);
            targetVolume = Mathf.Sqrt(normalizedSpeed);
        }

        currentGrindingVolume = Mathf.Lerp(
            currentGrindingVolume,
            targetVolume,
            Time.fixedDeltaTime * 8f
        );

        if (grindingAudioSource == null)
        {
            grindingAudioSource = SoundFXManager.Instance.PlayContinuousSound(
                "Grinding",
                currentGrindingVolume,
                GRIND_PITCH,
                contactPoint.point,
                grindingAudioSource,
                SPATIAL_BLEND
            );
        }
        else
        {
            SoundFXManager.Instance.UpdateContinuousSound(
                grindingAudioSource,
                currentGrindingVolume,
                GRIND_PITCH,
                contactPoint.point
            );
        }
    }

    private void OnCollisionExit()
    {
        if (!isActive)
            return;

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

    // private IEnumerator FitPiecePlace()
    // {
    //     float eulerZ = transform.eulerAngles.z;
    //     float snappedZ = Mathf.Round(eulerZ / 90f) * 90f;

    //     if (Mathf.Abs(Mathf.DeltaAngle(eulerZ, snappedZ)) <= SNAP_MARGIN)
    //     {
    //         float startingRotation = transform.eulerAngles.z;
    //         Vector3 startingPosition = transform.position;
    //         Vector3 targetPosition = new Vector3(
    //             Mathf.Round(transform.position.x),
    //             Mathf.Round(transform.position.y),
    //             transform.position.z
    //         );

    //         for (float t = 0; t < fittingTime; t += Time.deltaTime)
    //         {
    //             transform.SetPositionAndRotation(
    //                 new Vector3(
    //                     Mathf.SmoothStep(startingPosition.x, targetPosition.x, t / fittingTime),
    //                     Mathf.SmoothStep(startingPosition.y, targetPosition.y, t / fittingTime),
    //                     startingPosition.z
    //                 ),
    //                 Quaternion.Euler(
    //                     0,
    //                     0,
    //                     Mathf.SmoothStep(startingRotation, snappedZ, t / fittingTime)
    //                 )
    //             );
    //             yield return null;
    //         }
    //         isFittingPiece = false;
    //     }
    // }
}
