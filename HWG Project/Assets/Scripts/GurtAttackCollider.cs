using UnityEngine;
using UnityEngine.SceneManagement;

public class GurtAttackCollider : MonoBehaviour
{
    public string loseScene = "Lose Scene";
    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            SceneManager.LoadScene(loseScene);
        }
    }
}
