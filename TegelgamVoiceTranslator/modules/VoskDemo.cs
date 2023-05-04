using System;
using System.IO;
using Vosk;

namespace TegelgamVoiceTranslator.modules
{
    public class VoskDemo
    {
        private static string FinalizeStringresult(string inputResult)
        {   
            int firstIndex = inputResult.IndexOf('"') + 10;
            int lastIndex = inputResult.LastIndexOf('"');
            int length = lastIndex - firstIndex;
            return inputResult.Substring(firstIndex, length);
        }

        public static string DemoBytes(Model model, string voicePath)
        {
            // Demo byte buffer
            VoskRecognizer rec = new VoskRecognizer(model, 128000.0f);
            rec.SetMaxAlternatives(0);
            rec.SetWords(true);
            using (Stream source = File.OpenRead(voicePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        // Console.WriteLine(rec.Result());
                    }
                    else
                    {
                        // Console.WriteLine(rec.PartialResult());
                    }
                }
            }
            return FinalizeStringresult(rec.FinalResult());
        }

        public static string DemoFloats(Model model, string voicePath)
        {
            // Demo float array
            VoskRecognizer rec = new VoskRecognizer(model, 128000.0f);
            using (Stream source = File.OpenRead(voicePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    float[] fbuffer = new float[bytesRead / 2];
                    for (int i = 0, n = 0; i < fbuffer.Length; i++, n += 2)
                    {
                        fbuffer[i] = BitConverter.ToInt16(buffer, n);
                    }
                    if (rec.AcceptWaveform(fbuffer, fbuffer.Length))
                    {
                        // Console.WriteLine(rec.Result());
                    }
                    else
                    {
                        // Console.WriteLine(rec.PartialResult());
                    }
                }
            }
            return FinalizeStringresult(rec.FinalResult());
        }

        public static string DemoSpeaker(Model model, string voicePath)
        {
            // Output speakers
            SpkModel spkModel = new SpkModel("model-spk");
            VoskRecognizer rec = new VoskRecognizer(model, 128000.0f);
            rec.SetSpkModel(spkModel);

            using (Stream source = File.OpenRead(voicePath))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        // Console.WriteLine(rec.Result());
                    }
                    else
                    {
                        // Console.WriteLine(rec.PartialResult());
                    }
                }
            }
            return FinalizeStringresult(rec.FinalResult());
        }

        public static Model InitializeVoiceModel()
        {
            // You can set to -1 to disable logging messages
            Vosk.Vosk.SetLogLevel(0);
            string currentDirectory = Directory.GetCurrentDirectory();
            currentDirectory = currentDirectory.Substring(0, currentDirectory.Length-16);
            string modelPath = Directory.GetDirectories(currentDirectory + "\\modules\\model\\").First();
            Model model = new(modelPath);
            return model;
        }
    }
}