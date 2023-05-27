using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] private Image deathImage;
    [SerializeField] private AnimationCurve _fadeAnimation;
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private Image _image;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private int _exitScene;
    private int _pointsCounter;

    void Start()
    {
        _characterMovement.deathEvent += Death;
        _characterMovement.onStepBack += SubPoints;
        _characterMovement.onStepForward += AddPoints;
        _restartButton.onClick.AddListener(Restart);
        _exitButton.onClick.AddListener(Exit);
    }

    private void Death()
    {
        StartCoroutine(ShowDeathScreen());
    }

    private IEnumerator ShowDeathScreen()
    {
        deathImage.gameObject.SetActive(true);
        var animationTime = 0f;
        
        do
        {
            animationTime += Time.deltaTime;

            _image.material.color = new Color(_image.material.color.r, _image.material.color.g, _image.material.color.b,
                _fadeAnimation.Evaluate(animationTime));
            yield return null;
        } while (animationTime <= _fadeAnimation.keys[^1].time);
        
        _restartButton.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Exit()
    {
        SceneManager.LoadScene(_exitScene);
    }

    private void AddPoints()
    {
        _pointsCounter++;
        _text.text = $"{_pointsCounter}";
    }

    private void SubPoints()
    {
        _pointsCounter--;
        if (_pointsCounter < 0)
        {
            _pointsCounter = 0;
        }

        _text.text = $"{_pointsCounter}";
    }
}
