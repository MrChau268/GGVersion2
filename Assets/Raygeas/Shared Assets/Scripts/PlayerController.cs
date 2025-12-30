using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Raygeas
{
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        public class GroundLayer
        {
            public string layerName;
            public Texture2D[] groundTextures;
            public AudioClip[] footstepSounds;
        }

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5.0f;
        [SerializeField] private float runMultiplier = 3.0f;
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Camera Look")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensivity = 1.0f;
        [SerializeField] private float mouseVerticalClamp = 90.0f;

        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

        [Header("Footsteps")]
        [SerializeField] private AudioSource footstepSource;
        [SerializeField][Range(0f, 1f)] private float walkFootstepVolume = 0.4f;
        [SerializeField][Range(0f, 1f)] private float runFootstepVolume = 0.8f;
        [SerializeField] private float groundCheckDistance = 1.5f;
        [SerializeField][Range(1f, 4f)] private float footstepRate = 1f;
        [SerializeField][Range(1f, 4f)] private float runningFootstepRate = 1.5f;
        public List<GroundLayer> groundLayers = new List<GroundLayer>();

        private float _horizontalMovement;
        private float _verticalMovement;
        private float _currentSpeed;
        private Vector3 _moveDirection;
        private Vector3 _velocity;
        private CharacterController _characterController;
        private bool _isRunning;
        private float _verticalRotation;
        private Animator _animator;
        [Header("Jump Lightning Effect")]
        [SerializeField] private GameObject lightningEffectPrefab;
        [SerializeField] private Vector3 lightningOffset = new Vector3(0, 2f, 0);
        private Terrain[] _terrains;
        private Dictionary<Terrain, TerrainData> _terrainDataMap = new();
        private Dictionary<Terrain, TerrainLayer[]> _terrainLayersMap = new();
        private AudioClip _previousClip;
        private Texture2D _currentTexture;
        private RaycastHit _groundHit;
        private float _nextFootstep;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            Cursor.visible = false;
            GetTerrainData();
        }

        private void Update()
        {
            Movement();
            GroundChecker();
            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            SpeedCheck();
        }

        #region === MOVEMENT SYSTEM ===
        private void Movement()
        {
            HandleGroundCheck();
            HandleJump();
            HandleInput();
            HandleRotation();
            ApplyMovement();
            ApplyGravity();
            HandleActions();
            // HandleCameraFollow();
        }

        private void HandleGroundCheck()
        {
            if (_characterController.isGrounded && _velocity.y < 0)
                _velocity.y = -2f;
        }

        private void HandleJump()
        {
            if (Input.GetKeyDown(jumpKey) && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                _animator.SetTrigger("Jump");

                // 🌩 Spawn lightning effect
                if (lightningEffectPrefab != null)
                {
                    Vector3 spawnPosition = transform.position + lightningOffset;
                    GameObject lightningInstance = Instantiate(
                        lightningEffectPrefab,
                        spawnPosition,
                        Quaternion.identity,
                        transform // 👈 Attach to player
                    );

                    Destroy(lightningInstance, 1.5f); // Destroy after jump duration
                }
            }
        }

        private void HandleInput()
        {
            _horizontalMovement = Input.GetAxis("Horizontal"); // A/D
            _verticalMovement = Input.GetAxis("Vertical");     // W/S
            _isRunning = Input.GetKey(runKey);
            _currentSpeed = walkSpeed * (_isRunning ? runMultiplier : 1f);
        }

        private void ApplyMovement()
        {
            // Move only when pressing W
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 move = transform.forward * _currentSpeed * Time.deltaTime;
                _characterController.Move(move);
            }
        }

        private void ApplyGravity()
        {
            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void HandleRotation()
        {
            // A/D rotate left/right
            if (_horizontalMovement != 0)
            {
                transform.Rotate(Vector3.up, _horizontalMovement * 120f * Time.deltaTime);
            }

            // S rotates 180 degrees (no movement)
            if (Input.GetKeyDown(KeyCode.S))
            {
                transform.Rotate(Vector3.up, 180f);
            }
        }

        private void HandleActions()
        {
            if (Input.GetMouseButtonDown(0))
                _animator.SetTrigger("Throw");
        }

        // private void HandleCameraFollow()
        // {
        //     if (!playerCamera) return;

        //     // Camera stays in front of the player, facing him
        //     Vector3 offset = transform.forward * 3f + Vector3.up * 2f;
        //     playerCamera.transform.position = transform.position + offset;
        //     playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
        // }
        #endregion

        #region === ANIMATION ===
        private void UpdateAnimation()
        {
            float movementMagnitude = new Vector2(_horizontalMovement, _verticalMovement).magnitude;
            float targetSpeed = 0f;

            // Only W triggers movement animation
            if (Input.GetKey(KeyCode.W))
                targetSpeed = _isRunning ? 1f : 0.5f;

            float smoothedSpeed = Mathf.Lerp(_animator.GetFloat("Speed"), targetSpeed, Time.deltaTime * 8f);
            _animator.SetFloat("Speed", smoothedSpeed);
        }
        #endregion

        #region === FOOTSTEPS ===
        private void SpeedCheck()
        {
            if (_characterController.isGrounded && Input.GetKey(KeyCode.W))
            {
                float currentRate = _isRunning ? runningFootstepRate : footstepRate;

                if (_nextFootstep >= 100f)
                {
                    PlayFootstep();
                    _nextFootstep = 0;
                }
                _nextFootstep += currentRate * walkSpeed;
            }
        }

        private void PlayFootstep()
        {
            for (int i = 0; i < groundLayers.Count; i++)
            {
                for (int k = 0; k < groundLayers[i].groundTextures.Length; k++)
                {
                    if (_currentTexture == groundLayers[i].groundTextures[k])
                    {
                        footstepSource.PlayOneShot(RandomClip(groundLayers[i].footstepSounds));
                    }
                }
            }
        }

        private AudioClip RandomClip(AudioClip[] clips)
        {
            int attempts = 2;
            footstepSource.pitch = Random.Range(0.9f, 1.1f);
            footstepSource.volume = _isRunning ? runFootstepVolume : walkFootstepVolume;

            AudioClip selectedClip = clips[Random.Range(0, clips.Length)];
            while (selectedClip == _previousClip && attempts > 0)
            {
                selectedClip = clips[Random.Range(0, clips.Length)];
                attempts--;
            }
            _previousClip = selectedClip;
            return selectedClip;
        }
        #endregion

        #region === GROUND TEXTURE DETECTION ===
        private void GetTerrainData()
        {
            _terrains = Terrain.activeTerrains;
            foreach (Terrain terrain in _terrains)
            {
                _terrainDataMap[terrain] = terrain.terrainData;
                _terrainLayersMap[terrain] = terrain.terrainData.terrainLayers;
            }
        }

        private void GroundChecker()
        {
            Ray checkerRay = new Ray(transform.position + (Vector3.up * 0.1f), Vector3.down);
            if (Physics.Raycast(checkerRay, out _groundHit, groundCheckDistance))
            {
                foreach (Terrain terrain in _terrains)
                {
                    if (_groundHit.collider.gameObject == terrain.gameObject)
                    {
                        _currentTexture = _terrainLayersMap[terrain][GetTerrainTexture(terrain, transform.position)].diffuseTexture;
                        break;
                    }
                }
                if (_groundHit.collider.GetComponent<Renderer>())
                {
                    _currentTexture = GetRendererTexture();
                }
            }
        }

        private float[] GetTerrainTexturesArray(Terrain terrain, Vector3 controllerPosition)
        {
            TerrainData terrainData = _terrainDataMap[terrain];
            Vector3 terrainPosition = terrain.transform.position;

            int posX = (int)(((controllerPosition.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int posZ = (int)(((controllerPosition.z - terrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);

            float[,,] layerData = terrainData.GetAlphamaps(posX, posZ, 1, 1);
            float[] texturesArray = new float[layerData.GetUpperBound(2) + 1];
            for (int n = 0; n < texturesArray.Length; ++n)
                texturesArray[n] = layerData[0, 0, n];
            return texturesArray;
        }

        private int GetTerrainTexture(Terrain terrain, Vector3 controllerPosition)
        {
            float[] array = GetTerrainTexturesArray(terrain, controllerPosition);
            float maxArray = 0;
            int maxArrayIndex = 0;

            for (int n = 0; n < array.Length; ++n)
            {
                if (array[n] > maxArray)
                {
                    maxArrayIndex = n;
                    maxArray = array[n];
                }
            }
            return maxArrayIndex;
        }

        private Texture2D GetRendererTexture()
        {
            return (Texture2D)_groundHit.collider.gameObject.GetComponent<Renderer>().material.mainTexture;
        }
        #endregion
    }
}
