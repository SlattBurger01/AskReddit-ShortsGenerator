from moviepy.editor import *
import sys
import Utils

try:
    videoIdentifier = sys.argv[1]  # at position 0 is file name
except:
    videoIdentifier = "test_128"

print(f"Calling python script id = ({videoIdentifier})")

image1 = ImageClip(f'E:\HONZA\ContentGenerator\postH.png')
audio1 = AudioFileClip(f'E:\HONZA\ContentGenerator\postA.mp3')

imagePosition = ("center", 500)

firstPause = .4  # in seconds, pause between title and first comment
pause = .5  # in seconds, pause between audios and videos
beforeEndPause = .4  # in seconds


def EditVideo(commentData):
    print("Editing video !")

    a1 = GetAudio(audio1, 0)
    i1 = GetImage(image1, 0, a1.duration)

    images = commentData[0]
    audios = commentData[1]

    finalAudios = [a1]
    finalVideos = [i1]

    audioDuration = a1.duration + firstPause

    recentAudio = None

    for i in range(len(images)):
        bAudio = a1
        if(i != 0):
            bAudio = recentAudio

        audio = AudioFileClip(audios[i])
        image = ImageClip(images[i])

        # pause is included in "NextImageStart" !!!
        a = GetAudio(audio, NextImageStart(bAudio))
        img = GetImage(image, NextImageStart(bAudio), a.duration)

        audioDuration += a.duration + pause

        finalAudios.append(a)
        finalVideos.append(img)

        recentAudio = a

    videoDuration = audioDuration + beforeEndPause

    finalVideos.insert(0, Utils.GetBackground(videoDuration))

    finalAudio = CompositeAudioClip(finalAudios)
    final = CompositeVideoClip(finalVideos)

    print(f"final duration = {videoDuration}")

    final.duration = videoDuration
    final.audio = finalAudio

    final.write_videofile(GetVideoPath(videoIdentifier))


def GetImage(refImage, start, duration):
    return refImage.set_start(start).set_position(imagePosition).set_duration(duration).resize(2).set_opacity(.95)


def GetAudio(refAudio, start):
    return Utils.CropSilentEnd(refAudio.set_start(start))


def NextImageStart(audio1):
    return audio1.start + audio1.duration + pause


def GetVideoPath(videoIdentifier):
    return f"E:\HONZA\ContentGenerator\CompletedVideos\Video-{videoIdentifier}.mp4"


data = Utils.GetComments()
EditVideo(data)
