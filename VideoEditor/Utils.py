import glob
import random
import Paths
import Settings
from moviepy.editor import *

def GetComments() -> tuple[list, list]:
    """ Returns tuple of sorted (commentImagePaths, commentAudioPaths) lists """
    comments = glob.glob(f"{Paths.contentGeneratorFolder}\*.png")
    audios = glob.glob(f"{Paths.contentGeneratorFolder}\*.mp3")

    sortedCommentsH = GetSortedComments_(comments)
    sortedCommentsA = GetSortedComments_(audios)

    print(f"sorted: {sortedCommentsH} || {sortedCommentsA}")

    return (sortedCommentsH, sortedCommentsA)

def GetSortedComments_(comments) -> list:
    """ Returns list of sorted comments, "comments" is list of comment paths """

    sortedComments = []

    cPosition = 0

    for x in comments:

        for comment in comments:
            if(x == comment):
                continue

            if(comment.__contains__("comment")):                
                v = comment[29] # this will fuck up everything if any path is changed xDDD
                print(f"Comment {v}")

                if(v == f"{cPosition}"):
                    sortedComments.append(comment)
                    cPosition += 1

    return sortedComments

def CropSilentEnd(audio : AudioClip) -> AudioClip:
    """ Returns audio with cropped silent end """

    chunkDuration = 0.01  # seconds
    endPause = 0.05

    #chunks = []

    #for chunk in audio.iter_chunks(chunk_duration=chunk_duration):
    #    chunks.append(chunk)

    chunks = GetAudioChunks(audio, chunkDuration)

    i = 0
    lastT = 0

    for chunk in chunks:
        # this seems to fix the fragmets at the end without any other side effect :)
        if (i == len(chunks) - 10):
            break

        #print(f"chunk: {i} - {chunk.max()} {len(chunks)}")

        energy = chunk.max()

        if energy > 0:
            lastT = i

        i += 1

    end = lastT * chunkDuration + endPause

    print(f"end: {end}")
    print(f"dur: {audio.duration}")

    if(end < audio.duration):
        return audio.subclip(0, end)
    else:
        return audio

def GetAudioChunks(audio : CompositeAudioClip, duration) -> list:
    """ Returns list of audio chunks """
    chunks = []

    for chunk in audio.iter_chunks(chunk_duration=duration):
        chunks.append(chunk)

    return chunks

def GetBackground(videoLenght) -> VideoClip:
    """ Returns background with custom start (depends on settings) with set values """
    id = random.randint(0, len(Paths.backgrounds) - 1)
    bg = Paths.backgrounds[id]

    clip = VideoFileClip(bg)

    #print(f"bg = {bg}")

    startPos = 0

    if (Paths.customStartPosition[id]):
        maxStartPos = clip.duration - videoLenght
        startPos = -random.uniform(0, maxStartPos - 1)
        #print(f"sPos = {startPos}")

    return clip.set_start(startPos)

def GetAudio(refAudio : AudioClip, start) -> AudioClip:
    """ Returns audio with cropped silent end and set values """
    return CropSilentEnd(refAudio.set_start(start))

def GetImage(refImage, start, duration) -> ImageClip:
    """ Returns images with set values """
    return refImage.set_start(start).set_position(Settings.imagePosition).set_duration(duration).resize(2).set_opacity(.95)

def GetVideoPath(videoIdentifier : str) -> str:
    """ Returns path of rendered video """
    return f"{Paths.contentGeneratorFolder}\CompletedVideos\Video-{videoIdentifier}.mp4"
