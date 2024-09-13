using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace İşaret_Dili_Sözlük_Uygulaması
{
    internal class Word
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Example> Examples { get; set; }
        public string Video { get; set; }
    }

    internal class Example
    {
        public string Sentence { get; set; }
        public string Transcript { get; set; }
        public string Video { get; set; }
    }

    static internal class XMLReader
    {
        static List<Word> words = new List<Word>();
        static internal List<Word> ReadWordFile()
        {
            string path = "xml/kelimeler.xml";
            XDocument document = XDocument.Load(path);
            XElement root = document.Root;
            foreach (XElement wordElement in root.Elements("word"))
            {
                Word word = new Word();
                word.Name = wordElement.Element("name").Value;
                word.Description = wordElement.Element("description")?.Value;
                word.Examples = wordElement.Elements("example")?.Select(e => new Example
                {
                    Sentence = e.Element("sentence")?.Value,
                    Transcript = e.Element("transcript")?.Value,
                    Video = e.Element("video")?.Value
                }).ToList();
                word.Video = wordElement.Element("video")?.Value;
                words.Add(word);
            }
            return words;
        }
    }
}