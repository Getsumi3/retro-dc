using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{

    public Animator animator;
    public int levelToLoad;

    IEnumerator LoadLevelAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

    }

    public void FadeToLevel()
    {
        animator.SetTrigger("FadeOut");
    }


    public void OnFadeComplete()
    {
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    //trigger for portal
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            print("portal blocked by: " + other.gameObject.name);
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Player")
        {
            FadeToLevel();
            //GameManager.levelIndex++;
            GameManager.EnemyCount = 0;
            PlayerPrefs.SetInt("level id", GameManager.levelIndex);
        }
    }

}