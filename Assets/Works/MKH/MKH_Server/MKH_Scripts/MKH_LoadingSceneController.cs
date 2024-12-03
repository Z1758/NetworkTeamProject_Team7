using Photon.Pun;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MKH_LoadingSceneController : MonoBehaviour
{
    private static MKH_LoadingSceneController instance;
    public static MKH_LoadingSceneController Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<MKH_LoadingSceneController>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = Create();
                }
            }
            return instance;
        }
    }

    public static MKH_LoadingSceneController Create()
    {
        return Instantiate(Resources.Load<MKH_LoadingSceneController>("GameObject/MatchMaking/LoadingUI"));
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Slider loadingSlider;

    private string loadSceneName;

    private void Start()
    {
        StartCoroutine(LoadSceneProcess1());
    }

    public void LoadScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    #region 사용 X
    /*
    private IEnumerator LoadSceneProcess()
    {
        loadingSlider.value = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;
            if (loadingSlider.value <0.9f)
            {
                loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, 0.9f, Time.deltaTime);
            }
            else if(op.progress >= 0.9f)
            {
                loadingSlider.value = Mathf.MoveTowards(loadingSlider.value, 1f, Time.deltaTime);

                if (loadingSlider.value >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }

    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

    }
 
    
    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }

        if (!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }
    */
    #endregion

    private IEnumerator LoadSceneProcess1()
    {
        while (true)
        {
            yield return null;
            if(PhotonNetwork.LevelLoadingProgress == 0 ||
                PhotonNetwork.LevelLoadingProgress == 1)
            {
                //로딩 UI 안띄움
                //투명하게
                canvasGroup.alpha = 0f;
            }
            else
            {
                // 로딩 UI 띄움
                // 안투명하게
                canvasGroup.alpha = 1f;
                loadingSlider.value = PhotonNetwork.LevelLoadingProgress;
                
            }
        }

    }
}
