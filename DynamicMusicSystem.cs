using UnityEngine;
using Random = UnityEngine.Random;
using NaughtyAttributes;
using System;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DynamicMusic
{
    [RequireComponent(typeof(Collider)), SelectionBase]
    internal class DynamicMusicSystem : MonoBehaviour
    {
        #region Settings Script
        [SerializeField, BoxGroup("First clip"), Tooltip("Enable to play initial background music")] bool PlayFirstClip;
        [SerializeField, BoxGroup("First clip"), ShowIf("PlayFirstClip"), Tooltip("Array of audio clips to play as initial background music")] AudioClip[] FirstClip;
    [Space]
        [SerializeField, BoxGroup("General"), Tooltip("Main AudioSource component for background music")] AudioSource AudioSourceBackgroundMusic;
        [SerializeField, BoxGroup("General"), Tooltip("AudioMixerGroup for controlling music volume through mixer")] AudioMixerGroup AudioMixer;
    [Space]
        [SerializeField, BoxGroup("Settings"), Range(0.1f, 10f), Tooltip("Speed of music volume transition")] float SpeedChangeMusic = 5;
    [Space]
        [SerializeField, BoxGroup("Settings"), Tooltip("Enable tag-based music trigger detection")] bool CheckTag;
        [SerializeField, BoxGroup("Settings"), ShowIf("CheckTag"), Tag, Tooltip("Tag to check for music trigger")] string tagCheck = "Player";
        [SerializeField, BoxGroup("Settings"), Tooltip("Enable layer-based music trigger detection")] bool CheckLayer;
        [SerializeField, BoxGroup("Settings"), ShowIf("CheckLayer"), Layer, Tooltip("Layer to check for music trigger")] string layerCheck = "MusicLayer";
    [Space]
        [BoxGroup("Events"), Tooltip("Event triggered when a new music clip starts playing")] public UnityEvent OnPlayNewClip;
    [Space]
        [SerializeField, BoxGroup("Settings"), ShowIf("PlayFirstClip"), HideIf("IfExitStopMusic"), Tooltip("Return to playing first clip after exiting trigger zone")]
        bool PlayingFirstClipAfterExit;
    [Space]
        [SerializeField, ReadOnly, BoxGroup("Debug"), Tooltip("Current volume level")] float _volumeNow = 1;
        [SerializeField, ReadOnly, BoxGroup("Debug"), Tooltip("Indicates if player has exited trigger zone")] bool _playerExit;
        [SerializeField, ReadOnly, ReorderableList, BoxGroup("Debug"), Tooltip("List of currently playing audio sources")] List<AudioSource> playingSources;

        #endregion
        void Start()
        {
            if (PlayFirstClip)
                CreateAndPlaySource(FirstClip[Random.Range(0, FirstClip.Length)]);
            if (AudioSourceBackgroundMusic)
                CustomSource(AudioSourceBackgroundMusic);

            StartCoroutine(SimpleUpdate());
        }
        void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(SimpleUpdate());
        }

        void OnDisable() =>
            StopAllCoroutines();

        #region General

        /// <summary>
        /// Play music if player enter in collider
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            if (CheckTag)
            {
                if (!other.CompareTag(tagCheck)) return;
            }
            if (CheckLayer)
            {
                if (other.gameObject.layer != LayerMask.NameToLayer(layerCheck)) return;
            }

            other.TryGetComponent(out DynamicMusicPresset musicSettings);

            if (!musicSettings) return;

            CreateAndPlaySource(musicSettings.audioClips[Random.Range(0, musicSettings.audioClips.Length)]);

            _volumeNow = musicSettings.Volume;
            _playerExit = false;
        }
        /// <summary>
        /// Stop music if player exit from collider
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            if (CheckTag)
            {
                if (!other.CompareTag(tagCheck)) return;
            }
            if (CheckLayer)
            {
                if (other.gameObject.layer != LayerMask.NameToLayer(layerCheck)) return;
            }

            other.TryGetComponent(out DynamicMusicPresset musicSettings);

            if (!musicSettings) return;

            if (AudioSourceBackgroundMusic)
                CustomSource(AudioSourceBackgroundMusic);
            else if (PlayingFirstClipAfterExit)
                CreateAndPlaySource(FirstClip[Random.Range(0, FirstClip.Length)]);
        }

        /// <summary>
        /// Creating AudioSource in child list with 0 volume for further checking in SimpleUpdate()
        /// </summary>
        void CreateAndPlaySource(AudioClip audioClip)
        {
            if (playingSources.Count > 0 && playingSources[0].clip == audioClip)
            {
                Debug.Log($"Clip {audioClip} is already playing.");
                return;
            }
            Debug.Log($"Playing Clip : {audioClip} Length : {audioClip.length}");
            AudioSource Source = new GameObject(name = $"Music Player - {audioClip.name}", typeof(AudioSource)).GetComponent<AudioSource>();

            Source.gameObject.transform.SetParent(gameObject.transform);
            if (AudioMixer)
                Source.outputAudioMixerGroup = AudioMixer;
            Source.clip = audioClip;
            Source.loop = true;
            Source.volume = 0;
            Source.Play();

            OnPlayNewClip?.Invoke();

            playingSources.Add(Source);
        }
        void CustomSource(AudioSource source)
        {
            if (AudioMixer)
                source.outputAudioMixerGroup = AudioMixer;
            source.volume = 0;

            playingSources.Add(source);
        }
        internal void SetNewClip(AudioClip audioClip)
        {
            if (audioClip == null) return;
            CreateAndPlaySource(audioClip);
        }
        #endregion

        #region Simple Update
        /// <summary>
        /// Updating all existing AudioSource, removing sound from others except the last one added to the list.
        /// </summary>
        IEnumerator SimpleUpdate()
        {
            while (true)
            {
                for (int i = 0; i < playingSources.Count; i++)
                {
                    if (playingSources[i] != playingSources[^1] || _playerExit)
                    {
                        if (playingSources[i].volume > 0)
                            playingSources[i].volume -= Time.deltaTime * SpeedChangeMusic;

                        else
                        {
                            if (playingSources[i] != AudioSourceBackgroundMusic)
                                Destroy(playingSources[i].gameObject);

                            playingSources.Remove(playingSources[i]);
                        }
                    }
                    else
                    {
                        if (playingSources[i].volume != _volumeNow && !_playerExit)
                            playingSources[i].volume += Time.deltaTime * SpeedChangeMusic;
                    }
                }
                yield return new WaitForSeconds(.1f);
            }
        }
        #endregion
    }
}