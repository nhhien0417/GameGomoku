using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : Singleton<SettingsManager>
{
    public static bool IsSFXEnabled = true;
    public static bool IsBGMEnabled = true;
    public static bool IsVibrationEnabled = true;

    [SerializeField] Image _sfxIcon, _bgmIcon, _vibrationIcon;
    [SerializeField] Sprite _sfxOn, _sfxOff, _bgmOn, _bgmOff, _vibrationOn, _vibrationOff;

    public void Initialized()
    {
        LoadButtonsState();
    }

    public void SwitchSFX()
    {
        if (IsSFXEnabled)
        {
            IsSFXEnabled = false;
            _sfxIcon.sprite = _sfxOff;

            PlayerPrefs.SetInt("SFXIsEnable", 0);
        }
        else
        {
            IsSFXEnabled = true;
            _sfxIcon.sprite = _sfxOn;

            PlayerPrefs.SetInt("SFXIsEnable", 1);
        }
    }

    public void SwitchBGM()
    {
        if (IsBGMEnabled)
        {
            IsBGMEnabled = false;
            _bgmIcon.sprite = _bgmOff;

            PlayerPrefs.SetInt("BGMIsEnable", 0);
        }
        else
        {
            IsBGMEnabled = true;
            _bgmIcon.sprite = _bgmOn;

            PlayerPrefs.SetInt("BGMIsEnable", 1);
        }

        AudioManager.Instance.BackgroundMusic();
    }

    public void SwitchVibration()
    {
        if (IsVibrationEnabled)
        {
            IsVibrationEnabled = false;
            _vibrationIcon.sprite = _vibrationOff;

            PlayerPrefs.SetInt("VibrationIsEnable", 0);
        }
        else
        {
            IsVibrationEnabled = true;
            _vibrationIcon.sprite = _vibrationOn;

            PlayerPrefs.SetInt("VibrationIsEnable", 1);
        }
    }

    public void LoadButtonsState()
    {
        IsSFXEnabled = !Convert.ToBoolean(PlayerPrefs.GetInt("SFXIsEnable", 1));
        IsBGMEnabled = !Convert.ToBoolean(PlayerPrefs.GetInt("BGMIsEnable", 1));
        IsVibrationEnabled = !Convert.ToBoolean(PlayerPrefs.GetInt("VibrationIsEnable", 1));

        SwitchBGM();
        SwitchSFX();
        SwitchVibration();
    }
}
