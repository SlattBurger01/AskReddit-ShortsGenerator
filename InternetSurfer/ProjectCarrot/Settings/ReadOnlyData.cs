namespace ProjectCarrot
{
    public static class Ttsmp3XPaths
    {
        public static readonly string input = "//*[@id=\"voicetext\"]";
        public static readonly string voiceSelectorButton = "//*[@id=\"sprachwahl\"]";

        public static readonly string mathewVoice = "//*[@id=\"sprachwahl\"]/option[51]";

        public static readonly string downloadButton = "//*[@id=\"downloadenbutton\"]";
    }

    public static class TtsfreeXPaths
    {
        public static readonly string usernameInput = "/html/body/section/div/div/div/div/div[1]/div/form/div[1]/div[1]/input";
        public static readonly string passwordInput = "/html/body/section/div/div/div/div/div[1]/div/form/div[1]/div[2]/input";

        public static readonly string agreeButton = "/html/body/section/div/div/div/div/div[1]/div[1]/form/div[1]/div[3]/div/label/div/ins";
        public static readonly string submitButton = "/html/body/section/div/div/div/div/div[1]/div[1]/form/div[3]/input";

        public static readonly string ericVoiceButton = "/html/body/section[2]/div[2]/form/div[2]/div[1]/div[1]/div[2]/div/div[4]";

        public static readonly string downloadButton = "/html/body/section[2]/div[2]/form/div[2]/div[2]/div[1]/div[4]/div[2]/center[1]/div/a";
        public static readonly string textInput = "/html/body/section[2]/div[2]/form/div[1]/div[1]/div/textarea";

        public static readonly string converButton = "/html/body/section[2]/div[2]/form/div[2]/div[2]/div[1]/a";
    }

    public static class AskRedditXPaths
    {
        private static bool loggedIn => Settings.loginToReddit;

        public static string posts => loggedIn ? posts_L : posts_N;

        /// <summary> Comment header (question) for screenshots (after post was opened) </summary>
        public static string post => loggedIn ? post_L : post_N; // whole post for screenshot
        public static string postHeader => loggedIn ? header_L : header_N;

        public static string settings => loggedIn ? settings_L : settings_N;
        public static string darkMode => loggedIn ? darkMode_L : darkMode_N;

        public static string comments_4 => loggedIn ? comments_4_L : comments_4_N;
        public static string comments_5 => loggedIn ? comments_5_L : comments_5_N;
        public static string comments_6 => loggedIn ? comments_6_L : comments_6_N;

        // --- logging in
        public static readonly string loginIFrame = "/html/body/div[1]/div/div[2]/div[3]/div/div/iframe";

        public static readonly string loginButton_old = "/html/body/div[1]/div/div[2]/div[1]/header/div/div[2]/div/div[1]/a";
        public static readonly string loginButton_new = "/html/body/shreddit-app/reddit-header-large/reddit-header-action-items/header/nav/div[3]/span[3]/faceplate-tracker/faceplate-tooltip/a";

        public static readonly string usernameInput = "/html/body/div/main/div[1]/div/div/form/fieldset[1]/input";
        public static readonly string passwordInput = "/html/body/div/main/div[1]/div/div/form/fieldset[2]/input";
        public static readonly string finalLoginButton = "/html/body/div/main/div[1]/div/div/form/fieldset[4]/button";

        // both
        public static readonly string openPostButton_local = "div[1]/div[1]";

        public static readonly string acceptButton = "//*[@id=\"SHORTCUT_FOCUSABLE_DIV\"]/div[3]/div[1]/section/div/section[2]/section[1]/form/button";
        public static readonly string post_promotedClass = "_2oEYZXchPfHwcf9mTMGMg8"; // if post is advertisement

        public static readonly string commentLocal = "div/div/div/div[2]";
        public static readonly string commentPosition = "div/div/div";

        public static readonly string commentUserName_1 = "div/div/div/div[2]/div[3]/div[1]/span/div[1]/div/div/a";
        public static readonly string commentUserName_2 = "div/div/div/div[2]/div[2]/div[1]/span/div[1]/div/div/a";

        public static readonly string badgeLocal_3 = "div/div/div/div[2]/div[3]/div[1]/span/span[1]/span";
        public static readonly string badgeLocal_4 = "div/div/div/div[2]/div[4]/div[1]/span/span[1]/span";

        public static readonly string openedPostCloseButton = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[5]/button";

        public static readonly string notLoadedComments_RetryButton_1 = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[4]/button";
        public static readonly string notLoadedComments_RetryButton_2 = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[5]/button";

        public static readonly string interestsTab = "/html/body/div[1]/div/div[2]/div[4]/div/div/div/header/div/div[2]/button";

        public static readonly string postDescriptionParent_4 = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[1]/div/div[4]/div";
        public static readonly string postDescriptionParent_5 = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[1]/div/div[5]/div";

        public static readonly string[] postDescriptionParentArray = new string[] { postDescriptionParent_4, postDescriptionParent_5 };

        public static readonly string postHeaderTextOnly = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[1]/div/div[3]";

        public static readonly string postUserHeader = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[1]/div/div[2]";

        public static readonly string nsfwTagPost = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[1]/div/div[4]/div[2]";

        // --- NOT LOGGED IN ---
        private static readonly string posts_N = "/html/body/div[1]/div/div[2]/div[2]/div/div/div/div[2]/div[4]/div[1]/div[4]/div";
        private static readonly string post_N = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[2]/div[1]"; // for screenshots (after post was opened)

        private static readonly string header_N = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[2]/div[1]/div/div[3]/div[1]/div/h1"; // after post is epened

        private static readonly string settings_N = "/html/body/div[1]/div/div[2]/div[1]/header/div/div[2]/div/div[2]/div[2]/button";
        private static readonly string darkMode_N = "/html/body/div[30]/div/button[1]";

        // comments
        private static readonly string comments_4_N = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[2]/div[4]/div/div/div/div";
        private static readonly string comments_5_N = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[2]/div[5]/div/div/div/div";
        private static readonly string comments_6_N = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[2]/div[6]/div/div/div/div";

        // --- LOGGED IN ---
        private static readonly string posts_L = "/html/body/div[1]/div/div[2]/div[2]/div/div/div/div[2]/div[4]/div[1]/div[5]/div";
        private static readonly string post_L = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[1]"; // for screenshots (after post was opened)

        private static readonly string header_L = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[1]/div/div[3]/div[1]/div/h1"; // after post is epened

        private static readonly string settings_L = "/html/body/div[1]/div/div[2]/div[1]/header/div/div[2]/div[2]/div/div[2]/button";
        private static readonly string darkMode_L = "/html/body/div[30]/div/div/div[4]/button";

        // comments
        private static readonly string comments_4_L = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[4]/div/div/div/div";
        private static readonly string comments_5_L = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[5]/div/div/div/div";
        private static readonly string comments_6_L = "/html/body/div[1]/div/div[2]/div[3]/div/div/div/div[2]/div[1]/div[3]/div[6]/div/div/div/div";

        public static readonly string[] commentsArray = new string[] { comments_4_L, comments_5_L, comments_6_L };
    }

    public static class YouTubeXPaths
    {
        public static readonly string uploadButton = "/html/body/ytd-app/div[1]/div/ytd-masthead/div[4]/div[3]/div[2]/ytd-topbar-menu-button-renderer[1]/div/a/yt-icon-button/button/yt-icon";

        public static readonly string uploadVideo = "/html/body/ytd-app/ytd-popup-container/tp-yt-iron-dropdown/div/ytd-multi-page-menu-renderer/div[3]/div[1]/yt-multi-page-menu-section-renderer/div[2]/ytd-compact-link-renderer[1]/a/tp-yt-paper-item/div[2]/yt-formatted-string[1]";

        public static readonly string videoInput = "//*[@id=\"content\"]/input";
        public static readonly string notMadeForKidsButton = "//*[@id=\"audience\"]/ytkc-made-for-kids-select/div[4]/tp-yt-paper-radio-group/tp-yt-paper-radio-button[2]";

        public static readonly string videoNameInput = "//*[@id=\"textbox\"]";
        public static readonly string visibilityButton = "//*[@id=\"step-badge-3\"]";

        public static readonly string datePickerButton = "//*[@id=\"datepicker-trigger\"]/ytcp-dropdown-trigger";

        public static readonly string schedulePage = "//*[@id=\"schedule-radio-button\"]";

        public static readonly string dateInput = "//*[@id=\"input-2\"]/input";
        public static readonly string timeInput = "//*[@id=\"input-1\"]/input";

        public static readonly string scheduleButton = "//*[@id=\"done-button\"]/div";
    }

    public static class InstagramPaths
    {
        public static readonly string uploadButton = "/html/body/div[2]/div/div/div[1]/div/div/div/div[1]/div[1]/div[1]/div/div/div/div/div[2]/div[7]/div/div/a";

        public static readonly string fileInput = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div[1]/form/input";

        public static readonly string videoRatioButton = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div[1]/div/div/div/div[1]/div/div[2]/div/button";
        public static readonly string originaRationButton = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div[1]/div/div/div/div[1]/div/div[1]/div/button[1]";

        public static readonly string tryAgainButton = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div[1]/div/div/div[2]/div/button";
    }

    public static class TiktokPaths
    {
        public static readonly string videoInput = "/html/body/div[1]/div/div/div/div/div/div/div/input";

        public static readonly string hashtagRecomendation = "/html/body/div[1]/div/div/div/div[2]/div[2]/div[2]/div[1]/div/div[1]/div[2]/div/div[1]/div[2]";

        public static readonly string uploadButton = "/html/body/div[1]/div/div/div/div[2]/div[2]/div[2]/div[8]/div[2]/button";

        public static readonly string videoDescriptionInput = "/html/body/div[1]/div/div/div/div/div[2]/div[2]/div[1]/div/div[1]/div[2]/div/div[1]/div/div/div";
        public static readonly string videoDescriptionText = "/html/body/div[1]/div/div/div/div[2]/div[2]/div[2]/div[1]/div/div[1]/div[2]/div/div/div/div/div/div/div/div/div/span/span";
    }

    public static class FileNames
    {
        public static readonly string postName = "postH";
        public static readonly string postAudio = "postA";

        public static readonly string commentName = "commentH";
        public static readonly string commentAudio = "commentA";
    }

    public static class LocalPaths
    {
        private static readonly string projectPath = "E:\\OtherProjects\\ProjectCarrot"; // new machine

        private static readonly string generatedVideosPath = "E:\\ContentGenerator"; // new machine

        public static readonly string addBlockPath = "C:\\Users\\kocna\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Extensions\\gighmmpiobklfepjocnamgkkbiglidom\\5.6.0_0.crx"; // new machine

        public static readonly string adAwayAddBlock = "C:\\Users\\kocna\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1\\Extensions\\dgjbaljgolmlcmmklmmeafecikidmjpi\\1.9.2_0.crx";

        public static readonly string pythonPath = "C:\\Users\\kocna\\AppData\\Local\\Programs\\Python\\Python311\\python.exe"; // make sure this path is to actuall python, not just its shortcut


        // this should not be necessary to change
        public static readonly string videoEditorPath = $"{projectPath}\\VideoEditor\\Main.py";
        public static readonly string customReaderPath = $"{projectPath}\\Reader\\Main.py";

        /// <summary> includes "/" </summary>
        public static readonly string filesPath = $"{generatedVideosPath}\\";

        public static readonly string completedVideosFolder = $"{generatedVideosPath}\\CompletedVideos";
        public static readonly string uploadedVideosFolder = $"{generatedVideosPath}\\UploadedVideos";
    }

    public static class LogingData
    {
        public static readonly string userNameReddit = "Slatt_270";
        public static readonly string userNameTtsFree = "Slatt";
        public static readonly string gmailVoiceMaker = "slatt.burger@gmail.com";

        public static readonly string password = "QfdlIOp5468sQ&ss";
    }

    public static class ChromePersons
    {
        public static readonly string person1 = "Person 1";
        public static readonly string person2 = "Person 2";
        public static readonly string person3 = "Person 3";
    }

    public static class RedditUrls
    {
        public static readonly string askReddit = "https://www.reddit.com/r/AskReddit/top";
        public static readonly string askMen = "https://www.reddit.com/r/AskMen/top";
        public static readonly string askWomen = "https://www.reddit.com/r/AskWomen/top";

        public static readonly string trueOffMyChest = "https://www.reddit.com/r/TrueOffMyChest/top";
    }

    public static class TtsUrls
    {
        public static readonly string ttsMp3 = "https://ttsmp3.com";
        public static readonly string narakeet = "https://www.narakeet.com/app/text-to-audio";
        public static readonly string ttsfree = "https://ttsfree.com";
        public static readonly string ttsfree_login = "https://ttsfree.com/login";
        public static readonly string voiceMaker = "https://voicemaker.in";
    }
}