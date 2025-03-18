using UnityEngine;
using KSP;
using KSP.IO;

[KSPModule("GroundAnchorFix")]
public class GroundAnchorFix : PartModule
{
    private const float anchorOffset = 0.15f;

    [KSPField(isPersistant = false)]
    public bool enableDebugLogs = false;

    public override void OnStart(StartState state)
    {
        base.OnStart(state);

        if (state == StartState.Flying)
        {
            if (enableDebugLogs)
                Debug.Log("[GroundAnchorFix] OnStart detected - fixing anchor to ground.");

            FixAnchorToGround();
        }
    }

    private void FixAnchorToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(part.transform.position, Vector3.down, out hit, 5f))
        {
            part.transform.position = hit.point - Vector3.up * anchorOffset;

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            Collider col = part.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            if (enableDebugLogs)
            {
                Debug.Log("[GroundAnchorFix] Anchor fixed at position: " + part.transform.position);
                Debug.Log("[GroundAnchorFix] Rigidbody isKinematic set to true.");
            }
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("[GroundAnchorFix] Raycast did not hit terrain.");
        }
    }
}
