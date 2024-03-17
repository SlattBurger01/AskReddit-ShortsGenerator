from moviepy.editor import *
import Utils
import Paths
import sys
import SubtitlesGenerator
from moviepy.video.tools.subtitles import SubtitlesClip

# args = 0: name, 1: videoId, 2: enable subtitles, 3: whisper subs, 4: video text that is going to be used for subs

try: # called via c# file
    videoIdentifier = sys.argv[1]  # at position 0 is file name
    enableSubsS = sys.argv[2]
    whisperSubsS = sys.argv[3]
    videoText = sys.argv[4]
except: # called via visual studio code
    videoIdentifier = "test_128"
    enableSubsS = "True"
    whisperSubsS = "True"
    videoText = "I have passed two nude hikers in my 35 years of hiking. One male, one female, years and thousands of miles apart. Both said hello. I said hello. One mentioned the trail was washed out ahead but a second trail has been cut. I thanked them for the heads-up. Some people like the wind and sun on their skin. Both had on hiking boots. To each their own."

enableSubs : bool = enableSubsS == "True"
whisperSubs : bool = whisperSubsS == "True"

print(f"Calling python script id = ({videoIdentifier}), generating subs: {enableSubs}, with args: {sys.argv}")

image1 = ImageClip(f'{Paths.contentGeneratorFolder}\postH.png')
audio1 = AudioFileClip(f'{Paths.contentGeneratorFolder}\postA.mp3')

firstPause = .4  # in seconds, pause between title and first comment
pause = .5  # in seconds, pause between audios and videos
beforeEndPause = .4  # in seconds

def EditVideo(commentData : tuple[list, list]):
    print(f"Editing video (subs: {enableSubs})!")

    a1 : AudioClip = Utils.GetAudio(audio1, 0)
    i1 : ImageClip = Utils.GetImage(image1, 0, a1.duration)

    images : list[str] = commentData[0]
    audios : list[str] = commentData[1]

    finalAudios : list[AudioClip] = [a1]
    finalVideos : list[VideoClip] = [i1]

    audioDuration : float = a1.duration + firstPause

    recentAudio : AudioClip = None

    print(f"Audios length: {len(audios)}")

    for i in range(len(audios)):
        bAudio = a1
        if(i != 0):
            bAudio = recentAudio

        audio = AudioFileClip(audios[i])

        if(enableSubs == False):
            print("Get image")
            image = ImageClip(images[i])

        # pause is included in "NextImageStart" !!!
        a = Utils.GetAudio(audio, NextImageStart(bAudio))

        audioDuration += a.duration + pause

        finalAudios.append(a)

        if (enableSubs == False):
            img = Utils.GetImage(image, NextImageStart(bAudio), a.duration)
            finalVideos.append(img)

        recentAudio = a

    videoDuration = audioDuration + beforeEndPause

    print(f"final duration = {videoDuration}")

    finalVideos.insert(0, Utils.GetBackground(videoDuration))

    finalAudioClip : CompositeAudioClip = CompositeAudioClip(finalAudios).set_fps(44100)

    if (enableSubs == True):
        subtitles : SubtitlesClip = SubtitlesGenerator.GetSubtitles(finalAudioClip, a1.duration + pause, whisperSubs, videoText)
        finalVideos.append(subtitles.set_duration(videoDuration))

        print("Logging subtitles: ")
        for x in subtitles.subtitles:
            print(x)

    final : CompositeVideoClip = CompositeVideoClip(finalVideos)

    final.duration = videoDuration
    final.audio = finalAudioClip

    final.write_videofile(Utils.GetVideoPath(videoIdentifier))

def NextImageStart(audio1):
    return audio1.start + audio1.duration + pause


comments = Utils.GetComments()
EditVideo(comments)