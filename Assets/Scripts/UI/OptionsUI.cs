using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    public class OptionsUI : MonoBehaviour
    {
        [Header("Mixer")]
        public AudioMixer audioMixer;

        [Header("Toggles")]
        public Toggle musicToggle;
        public Toggle sfxToggle;

        [Header("Sliders")]
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider sfxSlider;

        private const string MasterVolKey = "MasterVolume";
        private const string MusicVolKey = "MusicVolume";
        private const string SfxVolKey = "SFXVolume";
        private const string MusicEnabledKey = "MusicEnabled";
        private const string SfxEnabledKey = "SFXEnabled";

        private void Start()
        {
            var master = PlayerPrefs.GetFloat(MasterVolKey, 0.8f);
            var music = PlayerPrefs.GetFloat(MusicVolKey, 0.8f);
            var sfx = PlayerPrefs.GetFloat(SfxVolKey, 0.8f);
            var musicOn = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
            var sfxOn = PlayerPrefs.GetInt(SfxEnabledKey, 1) == 1;

            masterSlider.value = master;
            musicSlider.value = music;
            sfxSlider.value = sfx;
            musicToggle.isOn = musicOn;
            sfxToggle.isOn = sfxOn;

            ApplyMaster(master);
            ApplyMusic(music, musicOn);
            ApplySfx(sfx, sfxOn);
        }

        private float SliderToDb(float v)
        {
            if (v <= 0.001f) v = 0.001f;
            return Mathf.Log10(v) * 20f;
        }

        private void ApplyMaster(float v)
        {
            audioMixer.SetFloat("MasterVolume", SliderToDb(v));
        }

        private void ApplyMusic(float v, bool on)
        {
            var value = on ? v : 0.001f;
            audioMixer.SetFloat("MusicVolume", SliderToDb(value));
            musicSlider.interactable = on;
        }

        private void ApplySfx(float v, bool on)
        {
            var value = on ? v : 0.001f;
            audioMixer.SetFloat("SFXVolume", SliderToDb(value));
            sfxSlider.interactable = on;
        }

        public void OnMasterSliderChanged(float v)
        {
            PlayerPrefs.SetFloat(MasterVolKey, v);
            ApplyMaster(v);
        }

        public void OnMusicSliderChanged(float v)
        {
            PlayerPrefs.SetFloat(MusicVolKey, v);
            ApplyMusic(v, musicToggle.isOn);
        }

        public void OnSfxSliderChanged(float v)
        {
            PlayerPrefs.SetFloat(SfxVolKey, v);
            ApplySfx(v, sfxToggle.isOn);
        }

        public void OnMusicToggleChanged(bool on)
        {
            PlayerPrefs.SetInt(MusicEnabledKey, on ? 1 : 0);
            ApplyMusic(musicSlider.value, on);
        }

        public void OnSfxToggleChanged(bool on)
        {
            PlayerPrefs.SetInt(SfxEnabledKey, on ? 1 : 0);
            ApplySfx(sfxSlider.value, on);
        }
    }
}
