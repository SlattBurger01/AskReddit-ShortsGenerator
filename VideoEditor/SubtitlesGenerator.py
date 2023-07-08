from moviepy.editor import *
from moviepy.video.tools.subtitles import SubtitlesClip
import Utils

def GetSubsArray(audio : AudioFileClip, text : str, start : float):
    
    text = text.replace("?", ".")
    text = text.replace("!", ".")

    textArr = text.split(".")

    curArrPos = 0

    chunkDuration = 0.05

    chunks = Utils.GetAudioChunks(audio, chunkDuration)

    print(f"text array lenght: {len(textArr)} ({text})")
    for x in textArr:
        print(x)


    b = False # max was > than 0    

    subs = []

    prev = start

    i = 0

    count : int = 0

    # This sets the previous subtitle (after the narrator stops speaking)
    for x in chunks:

        if (i * chunkDuration > start):
            
            print(f"{x.max()} ({i * chunkDuration}) ({i}/{len(chunks)})")

            if (x.max() <= 0.0001):
                count += 1
            else:
                count = 0

            if (count >= 3):

                if(b):
                    value = i * chunkDuration

                    while(textArr[curArrPos] == ""):
                        curArrPos += 1

                    print(f"Updating subs ({curArrPos}/{len(textArr)}) ({textArr[curArrPos]})")
                    subs.append(((prev, value), textArr[curArrPos]))
                    prev = value

                    curArrPos += 1
                    b = False

                    print(f"Updating subs on time: {value}")
            else:
                b = True

        i += 1

    print(1)

    subs.append(((prev, i * chunkDuration + start), textArr[curArrPos])) # add the last one

    print(2)

    return subs


def GetSubtitles(audio : AudioFileClip, text : str, start : float):
    subs = GetSubsArray(audio, text, start) # ((start, end), text)

    print("Got subs array sucessfuly")

    subtitles = SubtitlesClip(subs, GetSubtitlesGenerator())
    result = subtitles.set_position(('center')).set_duration(10)

    return result

subtitlesWidth = 750
fontS = 80
strokeW = 4

def GetSubtitlesGenerator() -> TextClip:
    return lambda txt: TextClip(txt, font='Cooper-Black', fontsize = fontS, stroke_width = strokeW, stroke_color = "black", color='white', kerning=-2, interline=-1, size = (subtitlesWidth, 2000), method='caption')

def StringIsEmpty(text : str) -> bool:
    for x in text:
        if(x != " " and x != "."):
            return False
    return True