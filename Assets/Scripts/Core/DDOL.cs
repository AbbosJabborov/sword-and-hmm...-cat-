using UnityEngine;

namespace Core
{
    public class DDOL : MonoBehaviour
    {
        public static DDOL Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}