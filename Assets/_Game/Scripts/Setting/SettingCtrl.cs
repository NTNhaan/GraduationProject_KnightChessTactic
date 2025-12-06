using Audio;
using Data;
using UnityEngine;
using UnityEngine.UI;
namespace Setting
{
    public class SettingCtrl : Singleton<SettingCtrl>
    {
        private bool _isUIInitialized = false;
        public bool SetSound()
        {
            DBController.Instance.SOUND =! DBController.Instance.SOUND;
            AudioController.Instance.SetVolumeSound(DBController.Instance.SOUND);
            return DBController.Instance.SOUND;
        }
        public bool SetVibration()
        {
            DBController.Instance.VIBRATE =! DBController.Instance.VIBRATE;
            if (DBController.Instance.VIBRATE)
            {
                Handheld.Vibrate();
            }
            return  DBController.Instance.VIBRATE;
        }
        public bool SetMusic()
        {
            DBController.Instance.MUSIC =! DBController.Instance.MUSIC;
            AudioController.Instance.SetVolumeMusic( DBController.Instance.MUSIC);
            
            if (DBController.Instance.MUSIC)
            {
                if (ScreenController.Instance.CurScreen == ScreenGame.MainScreen)
                    AudioController.Instance.PlayMusic(Sound.Name.Music_Menu);
                else if (ScreenController.Instance.CurScreen == ScreenGame.GamePlayScreen)
                    AudioController.Instance.PlayMusic(Sound.Name.Music_GamePlay);
            }
            else
            {
                AudioController.Instance.StopMusic();
            }
            return DBController.Instance.MUSIC;
        }
        public void UpdateSettingImage(Image imgTarget, ButtonType[] dataList, bool isOn)
        {
            var type = isOn ? typeSetting.isOn : typeSetting.isOff;
            Debug.Log($"[UpdateSetting] type: {type}");
            foreach (var data in dataList)
            {
                if (data.type == type)
                {
                    imgTarget.sprite = data.sprite;
                    break;
                }
            }
        }
    }
}