using UnityEngine;
using NaughtyAttributes;
[RequireComponent(typeof(Collider))]
internal class DynamicMusicPresset : MonoBehaviour
{
    [BoxGroup("Settings")] public string Name;
    [BoxGroup("Settings")] public float Volume = 1;
    [BoxGroup("Settings"), ReorderableList] public AudioClip[] audioClips;
}
