using UnityEngine;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;

        private void Start()
        {
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}