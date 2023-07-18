from moviepy.editor import *
from moviepy.video.tools.subtitles import SubtitlesClip

subtitlesWidth = 750
fontS = 80
strokeW = 4

def GetSubtitlesGenerator() -> TextClip:
    return lambda txt: TextClip(txt, font='Cooper-Black', fontsize = fontS, stroke_width = strokeW, stroke_color = "black", color='white', kerning=-2, interline=-1, size = (subtitlesWidth, 2000), method='caption')

def GetSubtitleData(start : float, end : float, text : str):
    return ((start, end), text)
