using System;
using DG.Tweening;
using Farm.Helpers;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Farm.Managers
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [Header("Settings")] 
        [SerializeField] private float dbScaleFactor = 40f;
        
        [Header("SFX")]
        [SerializeField] private AudioMixer sfxMixer;
        [SerializeField] private AudioSource[] sfxSources;
        [SerializeField] private AudioClip[] sfxClips;

        [Header("Music")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip titleClip;
        [SerializeField] private AudioClip[] gameMusicClips;

        private const float MinDb = -80f;
        private AudioMixer _musicMixer;

        protected override void OnInitOnce()
        {
            base.OnInitOnce();
            _musicMixer = musicSource.outputAudioMixerGroup.audioMixer;
        }

        protected override void OnAfterInitialized()
        {
            base.OnAfterInitialized();
            SetVolumeMusic(0.8f);
            SetVolumeSfx(1f);
        }

        public void SetVolumeMusic(float volumeLinear)
        {
            //parse 0 to 1 to DB -80 to 0
            var dB = ParseToDb(volumeLinear);
            _musicMixer.SetFloat(GameConstants.MusicVolumeParam, dB);
        }
        
        public void SetVolumeSfx(float volumeLinear)
        {
            //parse 0 to 1 to DB -80 to 0
            var dB = ParseToDb(volumeLinear);
            sfxMixer.SetFloat(GameConstants.SfxVolumeParam, dB);
        }

        private float ParseToDb(float volumeLinear)
        {
            float dB;

            if (volumeLinear <= 0f)
            {
                dB = MinDb; // treat 0 as mute; avoid -Infinity
            }
            else
            {
                // Convert linear amplitude (0...1) to decibels
                dB = Mathf.Log10(Mathf.Clamp(volumeLinear, 0.0001f, 1f)) * dbScaleFactor;
                dB = Mathf.Max(dB, MinDb); // clamp to mixer’s floor
            }
            return dB;
        }

        public void PlayTitleMusic()
        {
            musicSource.Stop();
            musicSource.clip = titleClip;
            SetVolumeMusic(0.8f);
            musicSource.Play();
        }

        public void PlayGameMusic()
        {
            DOTween.Kill(musicSource);
            musicSource.Stop();
            musicSource.clip = gameMusicClips[Random.Range(0, gameMusicClips.Length)];
            musicSource.DOFade(1f, 1f);
            musicSource.Play();
        }

        public void StopMusic(Action onComplete)
        {
            DOTween.Kill(musicSource);
            musicSource.DOFade(0f, 1f).OnComplete( () => { musicSource.Stop(); onComplete?.Invoke(); });
        }

        public void PlaySfx(int index)
        {
            var freeSfx = GetFreeSfxSource();
            if(freeSfx == null) Debug.LogWarning("No free SFX source found!");
            freeSfx.pitch = Random.Range(0.8f, 1.2f);
            freeSfx.clip = sfxClips[index];
            freeSfx.Play();
        }

        private AudioSource GetFreeSfxSource()
        {
            foreach (var source in sfxSources)
            {
                if (!source.isPlaying) return source;
            }

            foreach (var source in sfxSources)
            {
                source.Stop();
                return source;
            }
            
            return null;
        }
    }
}