from moviepy.editor import *
from moviepy.video.tools.subtitles import SubtitlesClip
import Utils

def GetSubtitlesTest():
    audio = AudioFileClip("E:\ContentGenerator\commentA-0_0_p1.mp3")

    text = "Had alcohol poisoning, passed out in snowbank in -30 weather, thankfully someone found me soon after, my something"

    text.replace(".", ",")

    textArr = text.split(",")

    chunkDuration = 0.1

    chunks = Utils.GetAudioChunks(audio, chunkDuration)

    print(len(chunks))

    b = True # max was > than 0
    i = 0

    timeStamps = [0] # first is always on 0

    for x in chunks:
        if(x.max() <= 0):
            if(b):
                timeStamps.append(i * chunkDuration)
                b = False
        else:
            b = True


        i += 1
        print(x.max())

    print("Logging time stamps")

    for x in timeStamps:
        print(x)


def GetSubsArray(audio : AudioFileClip, text : str, start : float):
    
    text = text.replace("?", ".")
    text = text.replace("!", ".")
    
    textArr = text.split(".")

    curArrPos = 0

    chunkDuration = 0.1

    chunks = Utils.GetAudioChunks(audio, chunkDuration)

    print(f"text array lenght: {len(textArr)}")

    b = False # max was > than 0    

    subs = []

    prev = start

    i = 0

    # This sets the previous subtitle (after the narrator stops speaking)
    for x in chunks:

        if (i * chunkDuration > start):
            
            if(x.max() <= 0):

                if(b):
                    value = i * chunkDuration

                    subs.append(((prev, value), textArr[curArrPos]))
                    prev = value

                    curArrPos += 1
                    b = False

                    print(value)
            else:
                b = True


        i += 1

    subs.append(((prev, i * chunkDuration + start), textArr[curArrPos])) # add the last one

    return subs


def GetSubtitles(audio : AudioFileClip, text : str, start : float):
    subs = GetSubsArray(audio, text, start) # ((start, end), text)

    subtitles = SubtitlesClip(subs, GetSubtitlesGenerator())
    result = subtitles.set_position(('center')).set_duration(10)

    return result

subtitlesWidth = 800

def GetSubtitlesGenerator() -> TextClip:
    return lambda txt: TextClip(txt, font='Cooper-Black', fontsize=100, stroke_width = 4, stroke_color = "black", color='white', kerning=-2, interline=-1, size = (subtitlesWidth, 1080), method='caption')