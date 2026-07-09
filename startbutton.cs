using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void StartRun()
    {
        GameManager.Instance.StartRun();
    }

    public void RestartRun()
    {
        GameManager.Instance.RestartRun();
    }
}
