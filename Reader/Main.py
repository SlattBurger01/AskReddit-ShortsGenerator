import pyttsx3
import sys

textToRead = "Personally, I’d rather not piss off the person who doesn’t care enough about the hygiene to smell like total ass in public when I will be sharing a 4x4 space with them for the next few hours. I feel like that screams “unpredictable personality”."
fileName_name = "Output"

#file_name = sys.argv[0]
if(True):
    textToRead = sys.argv[1]
    fileName_name = sys.argv[2]


print("Reding text")


def Read(text_):
    engine = pyttsx3.init()
    engine.setProperty('outputFormat', 'mp3')

    filePath = f"E:\HONZA\ContentGenerator\{fileName_name}.mp3"

    print(filePath)

    engine.setProperty('rate', 190)  # reading speed

    engine.save_to_file(text_, filePath)
    engine.runAndWait()


#Read("I hate ttsmp3 so fucking much")
Read(textToRead)
