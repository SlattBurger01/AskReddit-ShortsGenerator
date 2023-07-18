from moviepy.editor import *
from moviepy.video.tools.subtitles import SubtitlesClip
import Utils
import SubtitlesGeneratorUtils as sUtils
import whisper
import Paths

def GetSubsArray_c(audio : AudioFileClip, start : float, text : str) -> list[(float, float), str]:
    """(start, end), text"""

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

    subs : list = []

    # previous subtitle end
    prev = start

    # count of silent chunks in a row
    count : int = 0

    i = 0

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

                    data = sUtils.GetSubtitleData(prev, value, textArr[curArrPos])
                    #subs.append(((prev, value), textArr[curArrPos])) # add the last one
                    subs.append(data)

                    prev = value

                    curArrPos += 1
                    b = False

                    print(f"Updating subs on time: {value}")
            else:
                b = True

        i += 1

    data = sUtils.GetSubtitleData(prev, i * chunkDuration + start, textArr[curArrPos])
    #subs.append(((prev, i * chunkDuration + start), textArr[curArrPos])) # add the last one
    subs.append(data)

    return subs

audioPath = f'{Paths.tempFolder}/audio.mp3'

def GetSubsArray_whisper(audio : AudioFileClip, start : float) -> list:    
    """(start, end), text"""

    audio = audio.subclip(start, audio.duration)

    audio.write_audiofile(audioPath)

    model = whisper.load_model('base')

    result = model.transcribe(audioPath, task = 'translate')
    
    subs = []

    for i in result['segments']:
        data = sUtils.GetSubtitleData(float(i['start']) + start, float(i['end']) + start, i['text'])
        subs.append(data)

    os.remove(audioPath)

    return subs


def GetSubtitles(audio : AudioFileClip, start : float, whisper : bool, text : str):
    """If 'whisper' is enabled: 'text' is unnecessary"""

    if (whisper):
        subs = GetSubsArray_whisper(audio, start)
    else:
        subs = GetSubsArray_c(audio, start, text) # ((start, end), text)

    subtitles = SubtitlesClip(subs, sUtils.GetSubtitlesGenerator())
    result = subtitles.set_position(('center')).set_duration(10)

    return result

#def StringIsEmpty(text : str) -> bool:
#    for x in text:
#        if(x != " " and x != "."):
#            return False
#    return True