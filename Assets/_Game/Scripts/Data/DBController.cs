using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
namespace Data
{
    public class DBController : Singleton<DBController>
    {
        #region VARIABLE
        private bool _music;
        public bool MUSIC
        {
            get => _music;
            set
            {
                _music = value;
                Save(DBKey.MUSIC, _music);
            }
        }

        private bool _sound;
        public bool SOUND
        {
            get => _sound;
            set
            {
                _sound = value;
                Save(DBKey.SOUND, _sound);
            }
        }

        private bool _vibrate;
        public bool VIBRATE
        {
            get => _vibrate;
            set
            {
                _vibrate = value;
                Save(DBKey.VIBRATE, _vibrate);
            }
        }
        private bool _tutorialCompleted;
        public bool TUTORIAL_COMPLETED
        {
            get => _tutorialCompleted;
            set
            {
                _tutorialCompleted = value;
                Save(DBKey.TUTORIAL_COMPLETED, _tutorialCompleted);
            }
        }
        private int _score;
        public int SCORE
        {
            get => _score;
            set
            {
                _score = value;
                Save(DBKey.SCORE, _score);
            }
        }
        private int _bestScore;
        public int BEST_SCORE
        {
            get => _bestScore;
            set
            {
                _bestScore = value;
                Save(DBKey.BEST_SCORE, _bestScore);
            }
        }

        private int _level;
        public int LEVEL
        {
            get => _level;
            set
            {
                _level = value;
                Save(DBKey.LEVEL, _level);
            }
        }
        private int _exp;
        public int EXP
        {
            get => _exp;
            set
            {
                _exp = value;
                Save(DBKey.EXP, _exp);
            }
        }

        private int _coin;
        public int COIN
        {
            get => _coin;
            set
            {
                _coin = value;
                Save(DBKey.COIN, _coin);
            }
        }
        private int _coinPerLevel;
        public int COIN_PER_LEVEL
        {
            get => _coinPerLevel;
            set
            {
                _coinPerLevel = value;
                Save(DBKey.COIN_PER_LEVEL, _coinPerLevel);
            }
        }

        private int _dogSkin;
        public int DOG_SKIN
        {
            get => _dogSkin;
            set
            {
                _dogSkin = value;
                Save(DBKey.DOG_SKIN, _dogSkin);
            }
        }
        private int _catSkin;

        public int CAT_SKIN
        {
            get => _catSkin;
            set
            {
                _catSkin = value;
                Save(DBKey.CAT_SKIN, _catSkin);
            }
        }
        private int _lastPlayTime;

        public int LAST_PLAY_TIME
        {
            get => _lastPlayTime;
            set
            {
                _lastPlayTime = value;
                Save(DBKey.LAST_PLAY_TIME, _lastPlayTime);
            }
        }
        private int _bestPlayTime;

        public int BEST_PLAY_TIME
        {
            get => _bestPlayTime;
            set
            {
                _bestPlayTime = value;
                Save(DBKey.BEST_PLAY_TIME, _bestPlayTime);
            }
        }
        private int _guideBooster;

        public int GUIDE_BOOSTER
        {
            get => _guideBooster;
            set
            {
                _guideBooster = value;
                Save(DBKey.GUIDE_BOOSTER, _guideBooster);
            }
        }
        private string _ownedThemes; 
        public string OWNED_THEMES
        {
            get => _ownedThemes;
            set { _ownedThemes = value; Save(DBKey.OWNED_THEMES, _ownedThemes); }
        }
        private string _selectedTheme;
        public string SELECTED_THEME
        {
            get => _selectedTheme;
            set { _selectedTheme = value; Save(DBKey.SELECTED_THEME, _selectedTheme); }
        }
        private HashSet<string> ownedThemeSet = new();
        #endregion
        
        protected override void CustomAwake()
        {
            Initializing();
        }
        void Initializing()
        {
            CheckDependency(DBKey.MUSIC, key => MUSIC = true);
            CheckDependency(DBKey.SOUND, key => SOUND = true);
            CheckDependency(DBKey.VIBRATE, key => VIBRATE = true);
            CheckDependency(DBKey.TUTORIAL_COMPLETED, key => TUTORIAL_COMPLETED = false);
            CheckDependency(DBKey.SCORE, key => SCORE = 0);
            CheckDependency(DBKey.BEST_SCORE, key => BEST_SCORE = 0);
            CheckDependency(DBKey.LEVEL, key => LEVEL = 1);
            CheckDependency(DBKey.EXP, key => EXP = 0);
            CheckDependency(DBKey.COIN, key => COIN = 300);
            CheckDependency(DBKey.COIN_PER_LEVEL, key => COIN_PER_LEVEL = 10);
            CheckDependency(DBKey.DOG_SKIN, key => DOG_SKIN = 0);
            CheckDependency(DBKey.CAT_SKIN, key => CAT_SKIN = 0);
            CheckDependency(DBKey.LAST_PLAY_TIME, key => LAST_PLAY_TIME = 0);
            CheckDependency(DBKey.BEST_PLAY_TIME, key => BEST_PLAY_TIME = 0);
            CheckDependency(DBKey.GUIDE_BOOSTER, key => GUIDE_BOOSTER = 0);
            CheckDependency(DBKey.OWNED_THEMES, key => OWNED_THEMES = "default");
            CheckDependency(DBKey.SELECTED_THEME, key => SELECTED_THEME = "default");
            Load();
        }
        #region MainFucntions
        void CheckDependency(string key, UnityAction<string> onComplete)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                onComplete?.Invoke(key);
            }
        }
        public void Save<T>(string key, T values)
        {
            if (typeof(T) == typeof(int))
                PlayerPrefs.SetInt(key, (int)(object)values);
            else if (typeof(T) == typeof(bool))
                PlayerPrefs.SetInt(key, (bool)(object)values ? 1 : 0);
            else if (typeof(T) == typeof(string))
                PlayerPrefs.SetString(key, values as string);
            else if (typeof(T) == typeof(float))
                PlayerPrefs.SetFloat(key, (float)(object)values);
            else
            {
                try
                {
                    string json = JsonUtility.ToJson(values);
                    PlayerPrefs.SetString(key, json);
                }
                catch (UnityException e)
                {
                    throw new UnityException(e.Message);
                }
            }

            PlayerPrefs.Save(); 
        }
        void Load()
        {
            _sound = LoadDataByKey<bool>(DBKey.SOUND);
            _music = LoadDataByKey<bool>(DBKey.MUSIC);
            _vibrate = LoadDataByKey<bool>(DBKey.VIBRATE);
            _tutorialCompleted = LoadDataByKey<bool>(DBKey.TUTORIAL_COMPLETED);
            _score = LoadDataByKey<int>(DBKey.SCORE);
            _bestScore = LoadDataByKey<int>(DBKey.BEST_SCORE);
            _level = LoadDataByKey<int>(DBKey.LEVEL);
            _exp = LoadDataByKey<int>(DBKey.EXP);
            _coin = LoadDataByKey<int>(DBKey.COIN);
            _coinPerLevel = LoadDataByKey<int>(DBKey.COIN_PER_LEVEL);
            _dogSkin = LoadDataByKey<int>(DBKey.DOG_SKIN);
            _catSkin = LoadDataByKey<int>(DBKey.CAT_SKIN);
            _lastPlayTime = LoadDataByKey<int>(DBKey.LAST_PLAY_TIME);
            _bestPlayTime = LoadDataByKey<int>(DBKey.BEST_PLAY_TIME);
            _guideBooster = LoadDataByKey<int>(DBKey.GUIDE_BOOSTER);
            _ownedThemes = LoadDataByKey<string>(DBKey.OWNED_THEMES);
            _selectedTheme = LoadDataByKey<string>(DBKey.SELECTED_THEME);
        }
        
        public T LoadDataByKey<T>(string key)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)PlayerPrefs.GetInt(key);
            else if (typeof(T) == typeof(bool))
                return (T)(object)(PlayerPrefs.GetInt(key) == 1);
            else if (typeof(T) == typeof(string))
                return (T)(object)PlayerPrefs.GetString(key);
            else if (typeof(T) == typeof(float))
                return (T)(object)PlayerPrefs.GetFloat(key);
            else
            {
                string json = PlayerPrefs.GetString(key);
                return JsonUtility.FromJson<T>(json);
            }
        }
        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
        #endregion
        
        #region SHOP THEME FUNCTIONS

        public void InitDefaultTheme(List<BGModel> themes)
        {
            // Đọc lại dữ liệu OWNED_THEMES từ PlayerPrefs
            ownedThemeSet = new HashSet<string>(
                string.IsNullOrEmpty(OWNED_THEMES)
                    ? new List<string>()
                    : OWNED_THEMES.Split(',').Where(id => !string.IsNullOrEmpty(id))
            );

            // Nếu chưa có theme nào, tạo mặc định
            if (ownedThemeSet.Count == 0)
            {
                string defaultId = themes != null && themes.Count > 0 ? themes[0].id : "default";
                AddOwnedTheme(defaultId);
                SetSelectedTheme(defaultId);
                Save(DBKey.OWNED_THEMES, OWNED_THEMES);
                Save(DBKey.SELECTED_THEME, SELECTED_THEME);
                Debug.Log($"[DBController] Init default theme: {defaultId}");
            }

            // Nếu đã có dữ liệu nhưng SELECTED_THEME trống => gán lại
            if (string.IsNullOrEmpty(SELECTED_THEME))
            {
                SELECTED_THEME = ownedThemeSet.FirstOrDefault() ?? "default";
                Save(DBKey.SELECTED_THEME, SELECTED_THEME);
            }
        }

        public bool IsThemeOwned(string id)
        {
            return ownedThemeSet.Contains(id);
        }

        public void AddOwnedTheme(string id)
        {
            if (ownedThemeSet.Add(id))
            {
                OWNED_THEMES = string.Join(",", ownedThemeSet);
                Save(DBKey.OWNED_THEMES, OWNED_THEMES);
            }
        }

        public void SetSelectedTheme(string id)
        {
            // Chỉ cho chọn khi theme đã mua
            if (!IsThemeOwned(id))
            {
                Debug.LogWarning($"[DBController] Không thể chọn theme chưa mua: {id}");
                return;
            }

            SELECTED_THEME = id;
            Save(DBKey.SELECTED_THEME, SELECTED_THEME);
        }
        #endregion
    }   
}

public class DBKey
{
    public readonly static string COIN = "COIN";
    public readonly static string COIN_PER_LEVEL = "COIN_PER_LEVEL";
    public readonly static string SCORE = "SCORE";
    public static readonly string BEST_SCORE = "BEST_SCORE";
    public readonly static string LEVEL = "LEVEL";
    public readonly static string EXP = "EXP";
    public readonly static string SOUND = "SOUND";
    public readonly static string MUSIC = "MUSIC";
    public readonly static string VIBRATE = "VIBRATE";
    public readonly static string TUTORIAL_COMPLETED = "TUTORIAL_COMPLETED";
    public static readonly string USER_PROFILE = "USER_PROFILE";
    public static readonly string CHARACTER_DATA = "CHARACTER_DATA";
    public static readonly string DOG_SKIN = "DOG_SKIN";
    public static readonly string CAT_SKIN = "CAT_SKIN";
    public static readonly string LAST_PLAY_TIME = "LAST_PLAY_TIME";
    public static readonly string BEST_PLAY_TIME = "BEST_PLAY_TIME";
    public static readonly string GUIDE_BOOSTER = "GUIDE_BOOSTER";
    public static readonly string OWNED_THEMES = "OWNED_THEMES";
    public static readonly string SELECTED_THEME = "SELECTED_THEME";
}
