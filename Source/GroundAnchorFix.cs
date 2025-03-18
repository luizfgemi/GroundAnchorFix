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
                    ScreenMessages.PostScreenMessage("[GroundAnchorFix] GroundPart module not found!", 5f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            GameEvents.onPartCouple.Add(OnPartCouple);
            GameEvents.onPartDie.Add(OnPartDie);

            if (groundPart.deploymentState == ModuleGroundPart.DeploymentState.Deployed)
            {
                FixAnchorToGround();
            }
        }
    }

    private void OnPartCouple(GameEvents.FromToAction<Part, Part> data)
    {
        if (data.from == part || data.to == part)
        {
            FixAnchorToGround();
        }
    }

    private void OnPartDie(Part p)
    {
        if (p == part)
        {
            GameEvents.onPartCouple.Remove(OnPartCouple);
            GameEvents.onPartDie.Remove(OnPartDie);
        }
    }

    private void FixAnchorToGround()
    {
        if (groundPart.deploymentState != ModuleGroundPart.DeploymentState.Deployed)
            return;

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
            if (enableDebugLogs)
            {
                ScreenMessages.PostScreenMessage("[GroundAnchorFix] Anchor fixed at: " + part.transform.position, 5f, ScreenMessageStyle.UPPER_CENTER);
            }
        }
    }
}
