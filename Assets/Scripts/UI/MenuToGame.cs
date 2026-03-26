using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class MenuToGame : MonoBehaviour
    {
        public void PlayGame(int sceneID)
        {
            SceneManager.LoadScene(sceneID);
        }
    }
}
