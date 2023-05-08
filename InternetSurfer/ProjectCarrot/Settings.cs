namespace ProjectCarrot
{
    public enum TextToSpeechType { ttsmp3, ttsfree, pyttsx3 }

    public static class Settings
    {
        // for "Create" and "Upload" option
        public static readonly SessionSettings defaultSettings = new SessionSettings("def", 2, RedditUrls.AskReddit, ChromePersons.person3, new UploadPlatforms(1, 1, 0));

        // for "Create and Upload" option
        public static readonly SessionSettings[] sessionsSettings = new SessionSettings[]
        {
            new SessionSettings("p1", 4, RedditUrls.AskWomen, ChromePersons.person1, new UploadPlatforms(1, 1, 0)),
            new SessionSettings("p2", 4, RedditUrls.AskReddit, ChromePersons.person2, new UploadPlatforms(1, 1, 0)),
            new SessionSettings("p3", 4, RedditUrls.AskMen, ChromePersons.person3, new UploadPlatforms(1, 1, 0))
        };

        public static readonly int sessionSettingsVideoCount = GetSessionSettingsVideoCount(sessionsSettings);

        public static readonly TextToSpeechType speechType = TextToSpeechType.ttsfree;

        public static readonly bool loginToReddit = true;
        public static readonly bool loginToTtsFree = true;

        public static readonly int maxWordCountPerComment = int.MaxValue;
        public static readonly int maxCharsCountPerComment = 2000;

        //public static readonly int maxCommentCountPerVideo = 2; // not implemented yet

        public static readonly int schedulePause = 4; // in 1/4 hours

        // debug optios
        public static readonly bool renderVideo = true;
        public static readonly bool readText = true;

        public static int GetSessionSettingsVideoCount(SessionSettings[] settings)
        {
            int val = 0;

            for (int i = 0; i < settings.Length; i++)
            {
                val += settings[i].videoCount;
            }

            return val;
        }
    }

    public struct SessionSettings
    {
        public string redditUrl;
        public string uploadPerson;
        public UploadPlatforms uploadPlatforms;
        public int videoCount;
        public string sessionName;

        public SessionSettings(string name, int c, string rURL, string person, UploadPlatforms platforms)
        {
            redditUrl = rURL;
            uploadPerson = person;
            uploadPlatforms = platforms;
            videoCount = c;
            sessionName = name;
        }
    }

    public struct UploadPlatforms
    {
        public bool includeYoutube;
        public bool includeTikTok;
        public bool includeInstagram;

        public UploadPlatforms(bool y, bool t, bool i)
        {
            includeYoutube = y;
            includeTikTok = t;
            includeInstagram = i;
        }

        public UploadPlatforms(int y, int t, int i)
        {
            includeYoutube = y == 1;
            includeTikTok = t == 1;
            includeInstagram = i == 1;
        }
    }
}