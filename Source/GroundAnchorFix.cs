using UnityEngine;
using KSP;
using KSP.IO;

[KSPModule("GroundAnchorFix")]
public class GroundAnchorFix : PartModule
{
    [KSPField(isPersistant = false)]
    public float parafusoDepth = 0.12f; // Depth of the screws after animation (visual compensation)

    [KSPField(isPersistant = false)]
    public bool enableDebugLogs = false;

    private bool fixApplied = false;
    private ModuleGroundPart groundPart;

    public override void OnStart(StartState state)
    {
        base.OnStart(state);

        if (state == StartState.Flying)
        {
            groundPart = part.FindModuleImplementing<ModuleGroundPart>();
            if (groundPart == null)
            {
                Debug.Log("[GroundAnchorFix] Could not find ModuleGroundPart on this part.");
                if (enableDebugLogs)
                {
                    ScreenMessages.PostScreenMessage("[GroundAnchorFix] GroundPart module not found!", 5f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            else if (groundPart.IsAnchored)
            {
                ApplyFix();
            }
        }
    }

    public void Update()
    {
        if (!fixApplied && HighLogic.LoadedSceneIsFlight && groundPart != null && groundPart.IsAnchored)
        {
            ApplyFix();
        }
    }

    private void ApplyFix()
    {
        FixAnchorToGround();
        fixApplied = true;

        Debug.Log("[GroundAnchorFix] Anchor fix applied.");
        if (enableDebugLogs)
        {
            ScreenMessages.PostScreenMessage("[GroundAnchorFix] Anchor fix applied!", 5f, ScreenMessageStyle.UPPER_CENTER);
        }
    }

    private void FixAnchorToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(part.transform.position, Vector3.down, out hit, 5f))
        {
            Vector3 newPosition = part.transform.position;
            newPosition.y = hit.point.y - parafusoDepth;

            part.transform.position = newPosition;

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

            Debug.Log("[GroundAnchorFix] Anchor fixed at position: " + part.transform.position);
            Debug.Log("[GroundAnchorFix] Rigidbody isKinematic set to true.");

            if (enableDebugLogs)
            {
                ScreenMessages.PostScreenMessage("[GroundAnchorFix] Anchor fixed at: " + part.transform.position, 5f, ScreenMessageStyle.UPPER_CENTER);
            }
        }
        else
        {
            Debug.Log("[GroundAnchorFix] Raycast did not hit terrain.");
            if (enableDebugLogs)
            {
                ScreenMessages.PostScreenMessage("[GroundAnchorFix] Raycast failed to hit terrain!", 5f, ScreenMessageStyle.UPPER_CENTER);
            }
        }
    }
}
