using UnityEngine;

namespace Assets.Scripts.Database
{
    public static class LocalSave
    {
        public static void TrySaveHighscore(int highscore)
        {
            if (highscore > GetHighscore())
            {
                PlayerPrefs.SetInt("Highscore", highscore);
            }
        }

        public static int GetHighscore()
        {
            return PlayerPrefs.GetInt("Highscore");
        }
    }
}