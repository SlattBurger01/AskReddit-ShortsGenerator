import glob
import random
import Paths
from moviepy.editor import VideoFileClip


def GetComments():
    # E:\HONZA\ContentGenerator\commentH-1
    comments = glob.glob("E:\HONZA\ContentGenerator\*.png")
    audios = glob.glob("E:\HONZA\ContentGenerator\*.mp3")

    sortedCommentsH = GetSortedComments_(comments)
    sortedCommentsA = GetSortedComments_(audios)

    print(f"{sortedCommentsH} || {sortedCommentsA}")

    return (sortedCommentsH, sortedCommentsA)


def GetSortedComments_(comments):
    sortedComments = []

    cPosition = 0

    for x in comments:
        for comment in comments:
            if(x == comment):
                continue

            if(comment.__contains__("comment")):
                v = comment[35]
                if(v == f"{cPosition}"):
                    sortedComments.append(comment)
                    cPosition += 1

    return sortedComments


def CropSilentEnd(audio):
    chunk_duration = 0.01  # seconds
    endPause = 0.05

    chunks = []

    for chunk in audio.iter_chunks(chunk_duration=chunk_duration):
        chunks.append(chunk)

    i = 0
    lastT = 0

    for chunk in chunks:

        # this seems to fix the fragmets at the end without any other side effect :)
        if (i == len(chunks) - 10):
            break

        print(f"chunk: {i} - {chunk.max()} {len(chunks)}")

        energy = chunk.max()

        if energy > 0:
            lastT = i

        i += 1

    end = lastT * chunk_duration + endPause

    print(f"end: {end}")
    print(f"dur: {audio.duration}")

    if(end < audio.duration):
        return audio.subclip(0, end)
    else:
        return audio


def GetBackground(videoLenght):
    id = random.randint(0, len(Paths.backgrounds) - 1)
    bg = Paths.backgrounds[id]

    clip = VideoFileClip(bg)

    print(f"bg = {bg}")

    startPos = 0

    if (Paths.customStartPosition[id]):
        maxStartPos = clip.duration - videoLenght
        startPos = -random.uniform(0, maxStartPos - 1)
        print(f"sPos = {startPos}")

    return clip.set_start(startPos)
