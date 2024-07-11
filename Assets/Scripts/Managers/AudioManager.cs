using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource _clickSound, _tapSound, _swipeSound, _gameOverSound, _backgroundMusic;
    [SerializeField] AudioClip _winSoundClip, _loseSoundClip;

    public void Initialized()
    {
        if (SettingsManager.IsBGMEnabled)
        {
            _backgroundMusic.Play();
        }
    }

    public void ClickSFX()
    {
        if (SettingsManager.IsSFXEnabled)
        {
            _clickSound.Play();
        }
    }

    public void TapSFX()
    {
        if (SettingsManager.IsSFXEnabled)
        {
            _tapSound.Play();
        }
    }

    public void SwipeSFX()
    {
        if (SettingsManager.IsSFXEnabled)
        {
            _swipeSound.Play();
        }
    }

    public void GameOverSFX()
    {
        if (SettingsManager.IsSFXEnabled)
        {
            if (GameManager.Instance.Result == "Draw")
            {
                _gameOverSound.clip = _loseSoundClip;
            }
            else
            {
                if (GameManager.Instance.CurrentPlayer == "Human")
                {
                    _gameOverSound.clip = _winSoundClip;
                }
                else if (GameManager.Instance.CurrentPlayer == "AI")
                {
                    _gameOverSound.clip = _loseSoundClip;
                }
            }

            _gameOverSound.Play();
        }
    }

    public void BackgroundMusic()
    {
        if (SettingsManager.IsBGMEnabled)
        {
            _backgroundMusic.Play();
        }
        else
        {
            _backgroundMusic.Pause();
        }
    }

    public void Vibration()
    {
        if (SettingsManager.IsVibrationEnabled)
        {
            Handheld.Vibrate();
        }
    }
}
