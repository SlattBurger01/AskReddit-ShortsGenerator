# Moved into SubtitlesGenerator.py

# import whisper
# from moviepy import *
# from moviepy.video.tools.subtitles import *
# from moviepy.editor import *
# import SubtitlesGeneratorUtils as sUtils
# import Paths

# audioPath = f'{Paths.tempFolder}/audio.mp3'

# def GetSubsArray(audio : AudioFileClip, start : float) -> list:    
#     """(start, end), text"""

#     audio = audio.subclip(start, audio.duration)

#     audio.write_audiofile(audioPath)

#     model = whisper.load_model('base')

#     result = model.transcribe(audioPath, task = 'translate')
    
#     subs = []

#     for i in result['segments']:
#         data = sUtils.GetSubtitleData(float(i['start']) + start, float(i['end']) + start, i['text'])
#         subs.append(data)

#     os.remove(audioPath)

#     return subs
