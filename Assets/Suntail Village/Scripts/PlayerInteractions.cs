using UnityEngine;
using UnityEngine.UI;

//Interacting with objects and doors
namespace Suntail
{
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("Interaction variables")]
        [Tooltip("Layer mask for interactive objects")]
        [SerializeField] private LayerMask interactionLayer;
        [Tooltip("Maximum distance from player to object of interaction")]
        [SerializeField] private float interactionDistance = 3f;
        [Tooltip("Tag for door object")]
        [SerializeField] private string doorTag = "Door";
        [Tooltip("Tag for pickable object")]
        [SerializeField] private string itemTag = "Item";
        [Tooltip("The player's main camera")]
        [SerializeField] private Camera mainCamera;
        [Tooltip("Parent object where the object to be lifted becomes")]
        [SerializeField] private Transform pickupParent;

        [Header("Keybinds")]
        [Tooltip("Interaction key")]
        [SerializeField] private KeyCode interactionKey = KeyCode.H;

        [Header("Object Following")]
        [Tooltip("Minimum speed of the lifted object")]
        [SerializeField] private float minSpeed = 0;
        [Tooltip("Maximum speed of the lifted object")]
        [SerializeField] private float maxSpeed = 3000f;

        [Header("UI")]
        [Tooltip("Background object for text")]
        [SerializeField] private Image uiPanel;
        [Tooltip("Text holder")]
        [SerializeField] private Text panelText;
        [Tooltip("Text when an object can be lifted")]
        [SerializeField] private string itemPickUpText;
        [Tooltip("Text when an object can be drop")]
        [SerializeField] private string itemDropText;
        [Tooltip("Text when the door can be opened")]
        [SerializeField] private string doorOpenText;
        [Tooltip("Text when the door can be closed")]
        [SerializeField] private string doorCloseText;

        //Private variables.
        private PhysicsObject _physicsObject;
        private PhysicsObject _currentlyPickedUpObject;
        private PhysicsObject _lookObject;
        private Quaternion _lookRotation;
        private Vector3 _raycastPosition;
        private Rigidbody _pickupRigidBody;
        private Door _lookDoor;
        private float _currentSpeed = 0f;
        private float _currentDistance = 0f;
        private CharacterController _characterController;


        private void Start()
        {
            // mainCamera = Camera.main;
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Interactions();
            LegCheck();
        }

        //Determine which object we are now looking at, depending on the tag and component
        private void Interactions()
        {
            if (mainCamera == null)
            {
                Debug.LogError("PlayerInteractions: No camera assigned! Interaction raycasts won't work.");
                return;
            }

            // Cast ray from camera center forward
            Vector3 rayOrigin = mainCamera.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, mainCamera.transform.forward, out hit, interactionDistance, interactionLayer))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Reset UI first
                uiPanel.gameObject.SetActive(false);
                _lookDoor = null;
                _lookObject = null;

                // === HANDLE DOOR ===
                if (hitObject.CompareTag(doorTag))
                {
                    _lookDoor = hit.collider.GetComponentInParent<Door>() ?? hit.collider.GetComponentInChildren<Door>();

                    if (_lookDoor != null)
                    {
                        ShowDoorUI();

                        // Only run if user presses E while looking at door
                        if (Input.GetKeyDown(interactionKey))
                        {
                            Debug.Log($"[PlayerInteractions] Interacting with door: {_lookDoor.name}");
                            _lookDoor.PlayDoorAnimation();
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Door tag found, but no Door component on {hitObject.name}");
                    }
                }

                // === HANDLE ITEM ===
                else if (hitObject.CompareTag(itemTag))
                {
                    _lookObject = hitObject.GetComponentInChildren<PhysicsObject>();
                    if (_lookObject != null)
                    {
                        ShowItemUI();
                    }
                }
            }
            else
            {
                // Nothing hit
                _lookDoor = null;
                _lookObject = null;
                uiPanel.gameObject.SetActive(false);
            }

            // === PICKUP / DROP ===
            if (Input.GetKeyDown(interactionKey))
            {
                if (_lookDoor == null) // don't pick up if looking at door
                {
                    if (_currentlyPickedUpObject == null && _lookObject != null)
                    {
                        PickUpObject();
                    }
                    else if (_currentlyPickedUpObject != null)
                    {
                        BreakConnection();
                    }
                }
            }
        }



        //Disconnects from the object when the player attempts to step on the object, prevents flight on the object
        private void LegCheck()
        {
            Vector3 spherePosition = _characterController.center + transform.position;
            RaycastHit legCheck;
            if (Physics.SphereCast(spherePosition, 0.3f, Vector3.down, out legCheck, 2.0f))
            {
                if (legCheck.collider.CompareTag(itemTag))
                {
                    BreakConnection();
                }
            }
        }

        //Velocity movement toward pickup parent
        private void FixedUpdate()
        {
            if (_currentlyPickedUpObject != null)
            {
                _currentDistance = Vector3.Distance(pickupParent.position, _pickupRigidBody.position);
                _currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, _currentDistance / interactionDistance);
                _currentSpeed *= Time.fixedDeltaTime;
                Vector3 direction = pickupParent.position - _pickupRigidBody.position;
                _pickupRigidBody.linearVelocity = direction.normalized * _currentSpeed;
            }
        }

        //Picking up an looking object
        public void PickUpObject()
        {
            _physicsObject = _lookObject.GetComponentInChildren<PhysicsObject>();
            _currentlyPickedUpObject = _lookObject;
            _lookRotation = _currentlyPickedUpObject.transform.rotation;
            _pickupRigidBody = _currentlyPickedUpObject.GetComponent<Rigidbody>();
            _pickupRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            _pickupRigidBody.transform.rotation = _lookRotation;
            _physicsObject.playerInteraction = this;
            StartCoroutine(_physicsObject.PickUp());
        }

        //Release the object
        public void BreakConnection()
        {
            if (_currentlyPickedUpObject)
            {
                _pickupRigidBody.constraints = RigidbodyConstraints.None;
                _currentlyPickedUpObject = null;
                _physicsObject.pickedUp = false;
                _currentDistance = 0;
            }
        }

        //Show interface elements when hovering over an object
        private void ShowDoorUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (_lookDoor.doorOpen)
            {
                panelText.text = doorCloseText;
            }
            else
            {
                panelText.text = doorOpenText;
            }
        }

        private void ShowItemUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (_currentlyPickedUpObject == null)
            {
                panelText.text = itemPickUpText;
            }
            else if (_currentlyPickedUpObject != null)
            {
                panelText.text = itemDropText;
            }

        }

    }
}