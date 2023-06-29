from moviepy.editor import *
import Paths
import Utils
import SubtitlesGenerator
from moviepy.video.tools.subtitles import SubtitlesClip
    
clip = VideoFileClip(Paths.bg1).subclip(0, 5)
    
generator = SubtitlesGenerator.GetSubtitlesGenerator()
subs = [((.3, .6), 'I'), # (start, end) in seconds
        ((.6, .9), 'I really'),
        ((.9, 1.2), 'I really like'),
        ((1.2, 1.5), 'I  really like this')]

subtitles = SubtitlesClip(subs, generator)
result = subtitles.set_position(('center')).set_duration(10)
    
video = CompositeVideoClip([clip, result]) 

#print(TextClip.list("font"))

video.write_videofile(Utils.GetVideoPath(10))