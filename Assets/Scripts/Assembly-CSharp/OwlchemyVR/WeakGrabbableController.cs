using System;
using UnityEngine;

namespace OwlchemyVR
{
    [RequireComponent(typeof(GrabbableItem))]
    public class WeakGrabbableController : MonoBehaviour
    {
        private const float SNAP_SPEED_SCALE = 20f;
        private const float MAX_SNAP_SPEED = 15f;
        private const float MAX_OFFSET = 0.15f;
        private const float ANGULAR_SNAP_SPEED_SCALE = 0.5f;
        private const float MAX_ANGULAR_SNAP_SPEED = 30f;

        [SerializeField]
        private float strengthMultiplier = 1f;

        [SerializeField]
        private float toleranceMultiplier = 1f;

        [SerializeField]
        private bool affectsRotation;

        [SerializeField]
        private HapticTransformInfo hapticInfo;

        private GrabbableItem grabbableItem;
        private Vector3 initialLocalGrabPos;
        private Quaternion relativeRotAtGrab;
        private Transform handTransform;

        public float StrengthMultiplier
        {
            get
            {
                return strengthMultiplier;
            }
        }

        public float ToleranceMultiplier
        {
            get
            {
                return toleranceMultiplier;
            }
        }

        public bool AffectsRotation
        {
            get
            {
                return affectsRotation;
            }
        }

        protected virtual void Awake()
        {
            grabbableItem = GetComponent<GrabbableItem>();
            if (grabbableItem == null)
            {
                Debug.LogError("WeakGrabbableController couldn't find its required GrabbableItem", base.gameObject);
            }
            hapticInfo.ManualAwake();
        }

        protected virtual void Start()
        {
            if (grabbableItem.Rigidbody != null)
            {
                grabbableItem.Rigidbody.maxAngularVelocity = 30f;
            }
        }

        protected virtual void OnEnable()
        {
            GrabbableItem obj = grabbableItem;
            obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
            GrabbableItem obj2 = grabbableItem;
            obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
        }

        protected virtual void OnDisable()
        {
            GrabbableItem obj = grabbableItem;
            obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
            GrabbableItem obj2 = grabbableItem;
            obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
        }

        private void Grabbed(GrabbableItem item)
        {
            handTransform = grabbableItem.CurrInteractableHand.transform;
            initialLocalGrabPos = base.transform.InverseTransformPoint(handTransform.position);
            relativeRotAtGrab = Quaternion.Inverse(handTransform.rotation) * base.transform.rotation;
        }

        private void GrabbedUpdate(GrabbableItem item)
        {
            if (handTransform == null)
            {
                return;
            }
            Vector3 vector = base.transform.InverseTransformPoint(handTransform.position);
            Vector3 vector2 = base.transform.TransformVector(vector - initialLocalGrabPos);
            if (vector2.magnitude > 0.15f * toleranceMultiplier)
            {
                grabbableItem.CurrInteractableHand.TryRelease(false);
                return;
            }
            if (!grabbableItem.Rigidbody.isKinematic)
            {
                Vector3 velocity = vector2 * 20f * strengthMultiplier;
                float magnitude = velocity.magnitude;
                if (magnitude > 15f * strengthMultiplier)
                {
                    velocity *= 15f * strengthMultiplier / magnitude;
                }
                grabbableItem.Rigidbody.velocity = velocity;
            }
            if (affectsRotation)
            {
                Quaternion quaternion = handTransform.rotation * relativeRotAtGrab;
                float angle;
                Vector3 axis;
                quaternion.ToAngleAxis(out angle, out axis);
                while (angle > 180f)
                {
                    angle -= 360f;
                }
                while (angle < -180f)
                {
                    angle += 360f;
                }
                if (!grabbableItem.Rigidbody.isKinematic)
                {
                    Vector3 velocity2 = vector2 * 20f * strengthMultiplier;
                    float magnitude2 = velocity2.magnitude;
                    if (magnitude2 > 15f * strengthMultiplier)
                    {
                        velocity2 *= 15f * strengthMultiplier / magnitude2;
                    }
                    grabbableItem.Rigidbody.velocity = velocity2;
                    if (Mathf.Abs(angle) > 0.05f)
                    {
                        Vector3 angularVelocity = angle * axis * 0.5f * strengthMultiplier;
                        float magnitude3 = angularVelocity.magnitude;
                        if (magnitude3 > 30f * strengthMultiplier)
                        {
                            angularVelocity *= 30f * strengthMultiplier / magnitude3;
                        }
                        grabbableItem.Rigidbody.angularVelocity = angularVelocity;
                    }
                }
            }
            if (hapticInfo != null)
            {
                hapticInfo.ManualUpdate();
            }
        }

        public void EditorSetStrengthMultiplier(float s)
        {
            strengthMultiplier = s;
        }

        public void EditorSetToleranceMultiplier(float s)
        {
            toleranceMultiplier = s;
        }

        public void EditorSetAffectsRotation(bool a)
        {
            affectsRotation = a;
        }
    }
}
