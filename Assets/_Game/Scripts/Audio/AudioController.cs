using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.HapticFeedback;
using Data;

namespace Audio
{
    public class AudioController : Singleton<AudioController>
    {
        [Header("Background & SFX")]
        public Sound[] arrBackgroundMusic;
        public Sound[] arrSoundEffect;

        private AudioSource backgroundSource;
        private Coroutine fadeCoroutine;
        
        private List<AudioSource> pooledSources = new List<AudioSource>();
        [SerializeField] private int pooledSourceLimit = 10;
        private void OnEnable()
        {
            EventDispatcher.Register(EventId.OnPlayerDead, OnGameOver);
            EventDispatcher.Register(EventId.OnPlayerJump, OnPlayerJump);
            EventDispatcher.Register(EventId.OnGamePlayScreen, OnGamePlayScreen);
            EventDispatcher.Register(EventId.OnMainScreen, OnMainScreen);
            EventDispatcher.Register(EventId.OnSoundClick, OnSoundClick);
        }

        private void OnDisable()
        {
            EventDispatcher.RemoveCallback(EventId.OnPlayerDead, OnGameOver);
            EventDispatcher.RemoveCallback(EventId.OnPlayerJump, OnPlayerJump);
            EventDispatcher.RemoveCallback(EventId.OnGamePlayScreen, OnGamePlayScreen);
            EventDispatcher.RemoveCallback(EventId.OnMainScreen, OnMainScreen);
            EventDispatcher.RemoveCallback(EventId.OnSoundClick, OnSoundClick);
        }
        
        #region Init
        private void Start()
        {
            CreateAudioSource(arrSoundEffect);
            backgroundSource = gameObject.AddComponent<AudioSource>();
            backgroundSource.loop = true;

            CheckSound(DBController.Instance.MUSIC, DBController.Instance.SOUND);
            Preload();

            Debug.Log($"[AudioController] Start (Music={DBController.Instance.MUSIC}, Sound={DBController.Instance.SOUND})");
        }

        private void CreateAudioSource(Sound[] sounds)
        {
            foreach (var sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.loop = sound.loop;
            }
        }
        #endregion

        #region Event
        private void OnGameOver(object data = null) => PlayEffect(Sound.Name.Sound_GameOver);
        private void OnPlayerJump(object data = null) => PlayEffect(Sound.Name.Sound_Jumping);
        private void OnSoundClick(object data = null) => PlayEffect(Sound.Name.Sound_Click);
        private void OnGamePlayScreen(object data = null)
        {
            if (DBController.Instance.MUSIC)
            {
                PlayMusic(Sound.Name.Music_GamePlay);
            }
        }

        private void OnMainScreen(object data = null)
        {
            if (DBController.Instance.MUSIC)
            {
                PlayMusic(Sound.Name.Music_Menu);
            }
        }

        #endregion

        #region Music Control
        public void PlayMusic(Sound.Name musicName)
        {
            var musicSound = Array.Find(arrBackgroundMusic, s => s.name == musicName);
            if (musicSound == null || musicSound.clip == null)
            {
                Debug.LogWarning($"[AudioController] Music '{musicName}' not found!");
                return;
            }
            
            if (backgroundSource.isPlaying && backgroundSource.clip == musicSound.clip)
                return;
            
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            
            fadeCoroutine = StartCoroutine(FadeAndSwitchMusic(musicSound));
        }

        private IEnumerator FadeAndSwitchMusic(Sound newMusic, float fadeTime = 0.5f)
        {
            float startVolume = backgroundSource.volume;
            
            for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
            {
                backgroundSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
                yield return null;
            }

            backgroundSource.Stop();
            backgroundSource.clip = newMusic.clip;
            backgroundSource.volume = newMusic.volume;
            backgroundSource.Play();
            
            for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
            {
                backgroundSource.volume = Mathf.Lerp(0, newMusic.volume, t / fadeTime);
                yield return null;
            }

            backgroundSource.volume = newMusic.volume;
            fadeCoroutine = null;

            Debug.Log($"🎵 Now playing: {newMusic.name}");
        }

        public void StopMusic()
        {
            if (backgroundSource != null && backgroundSource.isPlaying)
                backgroundSource.Stop();
        }
        #endregion

        #region Effects
        public void PlayEffect(Sound.Name name)
        {
            var effect = Array.Find(arrSoundEffect, e => e.name == name);
            if (effect == null) return;
            effect.source.Play();
        }

        public void StopEffect(Sound.Name name)
        {
            var effect = Array.Find(arrSoundEffect, e => e.name == name);
            if (effect == null) return;
            effect.source.Stop();
        }
        #endregion

        #region Settings
        public void SetVolumeMusic(bool status)
        {
            backgroundSource.volume = status ? 1 : 0;
        }

        public void SetVolumeSound(bool status)
        {
            foreach (var sound in arrSoundEffect)
            {
                sound.source.volume = status ? sound.volume : 0;
            }
        }

        public void CheckSound(bool music, bool sound)
        {
            SetVolumeMusic(music);
            SetVolumeSound(sound);
        }

        public void Preload()
        {
            Debug.Log("[AudioController] Preloading sounds...");
            foreach (var s in arrSoundEffect)
            {
                if (s.clip == null || s.source == null) continue;
                s.source.volume = 0;
                s.source.Play();
                s.source.Stop();
                s.source.volume = s.volume;
            }
        }
        public void DefaultVibration()
        {
            if(DBController.Instance.VIBRATE)
                HapticFeedback.LightFeedback();
        }
        private void MediumVibration()
        {
            if(DBController.Instance.VIBRATE)
                HapticFeedback.MediumFeedback();
        }
        private void HeavyVibration()
        {
            if(DBController.Instance.VIBRATE)
                HapticFeedback.HeavyFeedback();
        }
        public void RunVibration(float intensity = 1f, int milliseconds = 50)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
            Debug.Log($"[Vibration] RunVibration: intensity={intensity}, ms={milliseconds}");
        }
        #endregion
        
        #region Fade for Alerts
        public void FadeBackgroundForAlert(float targetVolume = 0.3f, float duration = 0.5f)
        {
            if (backgroundSource == null) return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeAudioVolume(backgroundSource, targetVolume, duration));
        }

        public void RestoreBackgroundVolume(float duration = 0.5f)
        {
            if (backgroundSource == null) return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeAudioVolume(backgroundSource, 1f, duration));
        }

        private IEnumerator FadeAudioVolume(AudioSource source, float targetVolume, float duration)
        {
            float startVolume = source.volume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                yield return null;
            }

            source.volume = targetVolume;
        }
        #endregion
        
        
        #region Audio Pool
        private AudioSource GetPooledSource()
        {
            var src = pooledSources.Find(s => !s.isPlaying);
            if (src == null && pooledSources.Count < pooledSourceLimit)
            {
                src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 0f; 
                pooledSources.Add(src);
            }
            return src;
        }
        
        public void PlayEffectPooled(Sound.Name name, float volumeScale = 1f, float pitchMin = 0.95f, float pitchMax = 1.05f)
        {
            if (!DBController.Instance.SOUND) return;

            var sfx = Array.Find(arrSoundEffect, s => s.name == name);
            if (sfx == null || sfx.clip == null)
            {
                Debug.LogWarning($"[AudioController] SFX '{name}' not found or clip missing!");
                return;
            }

            var src = GetPooledSource();
            if (src == null) return; 

            src.volume = sfx.volume * volumeScale;
            src.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
            src.PlayOneShot(sfx.clip);
        }
        #endregion
    }
}
