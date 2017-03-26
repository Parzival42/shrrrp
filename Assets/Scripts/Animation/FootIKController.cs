using UnityEngine;

public class FootIKController : MonoBehaviour
{
    [SerializeField]
    private float leftFootWeight = 1f;

    [SerializeField]
    private float rightFootWeight = 1f;

    [SerializeField]
    private float footOffset = 0f;

    [Header("Raycast settings")]
    [SerializeField]
    private float raycastOffset = 0.1f;

    [SerializeField]
    private int raycastLayer = 8;

    [SerializeField]
    private float raycastDistance = 1f;

    private Animator animator;

    private Transform leftFoot;
    private Transform rightFoot;

    private Vector3 leftFootPosition;
    private Vector3 rightFootPosition;

    private Quaternion leftRotation;
    private Quaternion rightRotation;

    private void Start ()
    {
        animator = GetComponent<Animator>();
        leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

        leftRotation = leftFoot.rotation;
        rightRotation = rightFoot.rotation;
    }
	
	private void Update ()
    {
        RaycastGround(leftFoot, ref leftFootPosition, ref leftRotation);
        RaycastGround(rightFoot, ref rightFootPosition, ref rightRotation);
    }

    private void RaycastGround(Transform foot, ref Vector3 footPosition, ref Quaternion footRotation)
    {
        RaycastHit rayHit;
        Vector3 footPositionWorld = foot.TransformPoint(Vector3.zero) + new Vector3(0f, raycastOffset, 0f);

        Debug.DrawRay(footPositionWorld, -Vector3.up * raycastDistance, Color.red);
        if (Physics.Raycast(footPositionWorld, -Vector3.up, out rayHit, raycastDistance, 1 << raycastLayer))
        {
            footPosition = rayHit.point;
            footRotation = Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // TODO: Somehow get the animation infos for the current feet position for the weights.
        leftFootWeight = animator.GetFloat("LeftFoot");
        rightFootWeight = animator.GetFloat("RightFoot");

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);

        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition + new Vector3(0f, footOffset, 0f));
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition + new Vector3(0f, footOffset, 0f));

        animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftRotation);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, rightRotation);
    }

    private void OnDrawGizmos()
    {
        if (leftFoot != null && rightFoot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(leftFoot.position, Vector3.one * 0.05f);
            Gizmos.DrawCube(rightFoot.position, Vector3.one * 0.05f);

            Gizmos.DrawSphere(leftFootPosition, 0.02f);
            Gizmos.DrawSphere(rightFootPosition, 0.02f);
        }
    }
}