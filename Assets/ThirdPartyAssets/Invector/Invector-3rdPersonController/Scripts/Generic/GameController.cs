using UnityEngine;
using System.Collections;

namespace Invector
{
    public class GameController : MonoBehaviour
    {
        public Transform spawnPoint;
        public GameObject playerPrefab;
        public bool destroyPlayerDead;
        private GameObject currentPlayer;

        private static GameController _instance;        
        public static GameController instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameController>();
                    //Tell unity not to destroy this object when loading a new scene
                    //DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        void Start()
        {
            if (CharacterController.ThirdPersonController.instance == null && playerPrefab != null && spawnPoint != null)
                Spawn(spawnPoint);
            else if (CharacterController.ThirdPersonController.instance != null) currentPlayer = CharacterController.ThirdPersonController.instance.gameObject;
        }

        public void Spawn(Transform _spawnPoint)
        {
            if (playerPrefab != null)
            {
                if (currentPlayer != null && destroyPlayerDead) Destroy(currentPlayer);
                else
                {
                    var comps = currentPlayer.GetComponents<MonoBehaviour>();
                    foreach (Component comp in comps) Destroy(comp);
                    var coll = currentPlayer.GetComponent<Collider>();
                    if (coll != null) Destroy(coll);
                    var rigdbody = currentPlayer.GetComponent<Rigidbody>();
                    if (rigdbody != null) Destroy(rigdbody);
                    var animator = currentPlayer.GetComponent<Animator>();
                    if (animator != null) Destroy(animator);
                }
                    
                currentPlayer = Instantiate(playerPrefab, _spawnPoint.position, _spawnPoint.rotation) as GameObject;
            }
        }

        public void Spawn()
        {
            if (playerPrefab != null && spawnPoint != null)
            {
                if (currentPlayer != null && destroyPlayerDead) Destroy(currentPlayer);
                else
                {
                    var comps = currentPlayer.GetComponents<MonoBehaviour>();
                    foreach (Component comp in comps) Destroy(comp);
                    var coll = currentPlayer.GetComponent<Collider>();
                    if(coll!=null) Destroy(coll);
                    var rigdbody = currentPlayer.GetComponent<Rigidbody>();
                    if (rigdbody != null) Destroy(rigdbody);
                    var animator = currentPlayer.GetComponent<Animator>();
                    if (animator != null) Destroy(animator);
                }

                currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
            }
        }
    }
}