using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class Sound
    {
        public enum Name
        {
            Music_GamePlay,
            Music_Menu,
            Sound_Click,
            Sound_GameOver,
            Sound_Jumping,
            Sound_PoinBonus,
            Sound_Clock_Stuck,
            Sound_Clock,
            Sound_Icon_Appear,
            Sound_PopupClose,
            Sound_PopupOpen,
            Sound_Reward,
            Sound_Revive,
            Sound_Progress,
            Sound_Gift,
            Sound_CoinSpawn,
            Sound_CoinRecive,
            Sound_Celebrate,
            Sound_Confetti,
        }

        public Name name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1;
        [HideInInspector]
        public AudioSource source;
        public bool loop = false;
    }
}