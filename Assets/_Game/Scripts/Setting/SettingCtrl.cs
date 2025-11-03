using Audio;
using Data;
using UnityEngine;
using UnityEngine.UI;
namespace Setting
{
    public class SettingCtrl : Singleton<SettingCtrl>
    {
        [SerializeField] private Image imgSound;
        [SerializeField] private Image imgMusic;
        [SerializeField] private Image imgVibration;
        [SerializeField] private ButtonType[] sprtSound;
        [SerializeField] private ButtonType[] sprtMusic;
        [SerializeField] private ButtonType[] sprtVibration;
        
        private bool _isUIInitialized = false;
        public bool SetSound()
        {
            DBController.Instance.SOUND =! DBController.Instance.SOUND;
            AudioController.Instance.SetVolumeSound(DBController.Instance.SOUND);
            if(imgSound != null && sprtSound.Length > 0)
                UpdateSettingImage(imgSound, sprtSound, DBController.Instance.SOUND);
            return DBController.Instance.SOUND;
        }
        public bool SetVibration()
        {
            DBController.Instance.VIBRATE =! DBController.Instance.VIBRATE;
            if (DBController.Instance.VIBRATE)
            {
                Handheld.Vibrate();
            }
            if(imgVibration != null && sprtVibration.Length > 0)
                UpdateSettingImage(imgVibration, sprtVibration, DBController.Instance.VIBRATE);
            return  DBController.Instance.VIBRATE;
        }
        public bool SetMusic()
        {
            DBController.Instance.MUSIC =! DBController.Instance.MUSIC;
            AudioController.Instance.SetVolumeMusic( DBController.Instance.MUSIC);
            if(imgMusic != null && sprtMusic.Length > 0)
                UpdateSettingImage(imgMusic, sprtMusic, DBController.Instance.MUSIC);
            
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
        public void InitSetting()
        {
            if (!_isUIInitialized) return;
            
            UpdateSettingImage(imgSound, sprtSound, DBController.Instance.SOUND);
            UpdateSettingImage(imgMusic, sprtMusic, DBController.Instance.MUSIC);
            UpdateSettingImage(imgVibration, sprtVibration, DBController.Instance.VIBRATE);
            // UpdateSettingImage(imgVibration, sprtVibration, DBController.Instance.VIBRATION);
        }
        public void InjectUI(Image soundImg, Image musicImg, Image vibrationImg,
            ButtonType[] soundSprites, ButtonType[] musicSprites, ButtonType[] vibrationSprites)
        {
            imgSound = soundImg;
            imgMusic = musicImg;
            imgVibration = vibrationImg;

            sprtSound = soundSprites;
            sprtMusic = musicSprites;
            sprtVibration = vibrationSprites;

            _isUIInitialized = (imgSound && imgMusic && imgVibration);
            if (_isUIInitialized)
                InitSetting();
        }
    }
}