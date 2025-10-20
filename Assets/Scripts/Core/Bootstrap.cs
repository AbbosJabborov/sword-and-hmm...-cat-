using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class Bootstrap: MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return null;
            SceneManager.LoadScene("Menu");
        }
    }    
}