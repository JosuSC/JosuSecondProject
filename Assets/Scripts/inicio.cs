using UnityEngine;
using UnityEngine.SceneManagement;


public class inicio : MonoBehaviour
{
    public string nombredeescena;

    public void cargarescena()
    {
        SceneManager.LoadScene(1);
    }

}
