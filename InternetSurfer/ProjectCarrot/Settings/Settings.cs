using System.Security.Policy;

namespace ProjectCarrot
{
    public enum TextToSpeechType 
    { 
        ttsmp3,
        ttsfree,
        voiceMaker,
        narakeet,
        pyttsx3
    }

    public enum VideoType
    {
        postAndComments, // ask reddit
        postAndDescription // true of my chest reddit
    }

    public static class Settings
    {
        public static readonly Size browserSize = new Size(660, 1080);

        // for "Create" and "Upload" option
        public static readonly SessionSettings defaultSettings = new SessionSettings("def", Video.PostAndComments(1, RedditUrls.askReddit), ChromePersons.person2, new UploadPlatforms(0, 1, 0));

        // for "Create and Upload" option
        public static readonly SessionSettings[] sessionsSettings01 = new SessionSettings[]
        {
            new SessionSettings("p1", Video.PostAndComments(4, RedditUrls.askReddit), ChromePersons.person1, new UploadPlatforms(1, 1, 0)), // slatt
            new SessionSettings("p2", Video.PostAndDescription(4, RedditUrls.askWomen), ChromePersons.person2, new UploadPlatforms(1, 1, 0)), // meme mania ( meme.mania270@gmail.com )
        };

        public static readonly SessionSettings[] sessionsSettings02 = new SessionSettings[]
        {
            new SessionSettings("p1", Video.PostAndDescription(2, RedditUrls.trueOffMyChest), ChromePersons.person1, new UploadPlatforms(1, 1, 0)), // slatt
            //new SessionSettings("p2", Video.PostAndDescription(4, RedditUrls.askWomen), ChromePersons.person2, new UploadPlatforms(1, 1, 0)), // meme mania ( meme.mania270@gmail.com )
        };

        public static readonly int sessionSettingsVideoCount01 = GetSessionSettingsVideoCount(sessionsSettings01);

        public static readonly TextToSpeechType speechType = TextToSpeechType.ttsfree;

        public static readonly bool loginToReddit = true;
        public static readonly bool loginToTtsFree = true;

        public static readonly int maxWordCountPerComment = int.MaxValue;
        public static readonly int minWordCountPerComment = 1;

        public static readonly int maxCharsCountPerComment = 900;

        public static readonly int minCommentWordCount = 25; // if no comment like this was found: this option will be ignored

        public static readonly int maxCommentCountPerVideo = 5;
        public static readonly int maxWordCountPerVideo = int.MaxValue;

        public static readonly int schedulePause = 6; // in 1/4 hours

        public static readonly bool deleteFilesAfterUsed = false; // audio files and images will be deleted after video is generated
        public static readonly bool deleteVideosAfterUploaded = false;

        public static readonly int maxCommentHeight = 750; // for screenshots only
        public static readonly int minCharsForSingleComment = 360;

        public static readonly bool enableSubs = true; // even if subs are enabled, only images may be used (if there is comment that is not long enough)

        // debug optios
        public static readonly bool renderVideo = true;
        public static readonly bool readText = true;

        public static int GetSessionSettingsVideoCount(SessionSettings[] settings)
        {
            int val = 0;

            for (int i = 0; i < settings.Length; i++)
            {
                val += settings[i].video.count;
            }

            return val;
        }
    }

    public struct SessionSettings
    {
        public Video video;
        public string uploadPerson;
        public UploadPlatforms uploadPlatforms;
        public string sessionName;

        public SessionSettings(string name, Video v, string person, UploadPlatforms platforms)
        {
            video = v;
            uploadPerson = person;
            uploadPlatforms = platforms;
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

    public struct Video
    {
        public int count;
        public string rUrl;
        public VideoType type;

        public Video(int c, string url, VideoType t)
        {
            count = c;
            rUrl = url;
            type = t;
        }

        public static Video PostAndComments(int c, string url) => new Video(c, url, VideoType.postAndComments);
        public static Video PostAndDescription(int c, string url) => new Video(c, url, VideoType.postAndDescription);
    }
}