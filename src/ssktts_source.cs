/*
 * Simple Slovak Text to Speech - SSKTTS
 * Copyright (C) 2006 Andrej Pancik apancik <> gmail <> com
 * 
 * This is free and unencumbered software released into the public domain.
 * 
 * Anyone is free to copy, modify, publish, use, compile, sell, or
 * distribute this software, either in source code form or as a compiled
 * binary, for any purpose, commercial or non-commercial, and by any
 * means.
 * 
 * In jurisdictions that recognize copyright laws, the author or authors
 * of this software dedicate any and all copyright interest in the
 * software to the public domain. We make this dedication for the benefit
 * of the public at large and to the detriment of our heirs and
 * successors. We intend this dedication to be an overt act of
 * relinquishment in perpetuity of all present and future rights to this
 * software under copyright law.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleSKTTS
{
    /// <summary>
    /// A very simple slovak text to speech engine.
    /// </summary>
    public class SKTTS
    {
        /// <summary>
        /// Relative or absolute path of data folder.
        /// </summary>
        protected const string DATA_PATH = "data\\";

        /// <summary>
        /// Name of wav file representing space (' ') character.
        /// </summary>
        protected const string SPACE_NAME = "medzera";
        
        /// <summary>
        /// Reads aloud string passed in parameter. Automatically detects available syllables or word parts from data direcory (Path is specified in DATA_PATH constant).
        /// </summary>
        /// <param name="input">String used as for slovak reading routine.</param>
        public static void TellString(string input)
        {
            List<string> syllables = new List<string>();

            DirectoryInfo directory = new DirectoryInfo(DATA_PATH);

            foreach (FileInfo file in directory.GetFiles("*.wav"))
            {
                if (file.Name.Substring(0, file.Name.Length - file.Extension.Length) != SPACE_NAME) syllables.Add(file.Name.Substring(0, file.Name.Length - file.Extension.Length));
            }

            Queue<string> readingQueue = new Queue<string>();

            foreach (string word in input.Replace("q", "k").Replace("w", "v").Replace("ä","e").Replace("nn","n").Replace(":",".").ToLower().Split(' '))
            {
                string wordPart = word;
                while (wordPart != "")
                {
                    string longestSyllable = "";
                    foreach (string syllable in syllables)
                    {
                        if (wordPart.StartsWith(syllable) && syllable.Length > longestSyllable.Length) longestSyllable = syllable;
                    }
                    if (longestSyllable == "") break;
                    readingQueue.Enqueue(longestSyllable);
                    wordPart = wordPart.Remove(0, longestSyllable.Length);
                }
                readingQueue.Enqueue(SPACE_NAME);
            }

            System.Media.SoundPlayer player = new System.Media.SoundPlayer();

            while (readingQueue.Count > 0)
            {
                player.SoundLocation = DATA_PATH + readingQueue.Dequeue() + ".wav";
                player.PlaySync();
            }
        }
    }
}
