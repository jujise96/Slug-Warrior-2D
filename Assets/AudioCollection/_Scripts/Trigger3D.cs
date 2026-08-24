using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The base class for any trigger for 3D
    /// </summary>
    public class Trigger3D : MonoBehaviour
    {
        [Header("Trigger")]
        [SerializeField] private LayerMask triggerMask;
        [SerializeField][Tooltip("On enter, it turns inactive")] protected bool disableOnEnter;
        [SerializeField][Tooltip("On exit, it turns inactive")] protected bool disableOnExit;

        protected bool inTrigger = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// This should be called on all Triggers that inherit this class as "base.<see cref="OnEnter(Collider)"/>" in the OnTargetEnter method.
        /// </summary>
        /// <param name="collider"></param>
        protected void OnEnter(Collider collider)
        {
            if (collider != null && (triggerMask.value & (1 << collider.gameObject.layer)) != 0)
            {
                inTrigger = true;
                if (disableOnEnter) gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// This should be called on all Triggers that inherit this class as "base.<see cref="OnExit(Collider)"/>" in the OnTargetExit method.
        /// </summary>
        /// <param name="collider"></param>
        protected void OnExit(Collider collider)
        {
            if (collider != null && (triggerMask.value & (1 << collider.gameObject.layer)) != 0)
            {
                inTrigger = false;
                if (disableOnExit) gameObject.SetActive(false);
            }
        }
    }
}
