from moviepy.editor import *
import SubtitlesGenerator
from moviepy.video.tools.subtitles import SubtitlesClip

videoPath = 'E:/ContentGenerator/video.mp4'

clip = VideoFileClip(videoPath)
subtitles = SubtitlesGenerator.GetSubtitles(clip.audio, 3, True, "")

for x in subtitles:
    print(x)

# Test passed !