﻿/*
MIT License

Copyright (c) 2021 eprilx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;

namespace BMFONT
{
    class BMFontStruct
    {
        public general generalInfo;
        public List<charDesc> charDescList;
        public List<kernelDesc> kernelDescList;

        public void SortCharDescListById()
        {
            this.charDescList.Sort((x, y) => x.id.CompareTo(y.id));
        }

        public BMFontStruct()
        {
            generalInfo = new();
            charDescList = new();
            kernelDescList = new();
        }
        public class general
        {
            public int lineHeight;
            public int _base;
            public int WidthImg; // width image
            public int HeightImg; // height image
            public int pages;

            public string face;
            public int size;
            public int bold;
            public int italic;
            public int charsCount;
            public int kernsCount;
            public List<int> idImg;
            public List<string> fileImg;

            public general()
            {
                face = "";
                size = 0;
                bold = 0;
                italic = 0;
                lineHeight = 0;
                _base = 0;
                idImg = new();
                fileImg = new();
            }
        }
        public class charDesc
        {
            public int id;
            public float x;
            public float y;
            public float width;
            public float height;
            public int xoffset;
            public int yoffset;
            public int xadvance;
            public int page;
            public int chnl;

            public charDesc()
            {
                id = 0;
                x = 0;
                y = 0;
                width = 0;
                height = 0;
                xoffset = 0;
                yoffset = 0;
                xadvance = 0;
                page = 0;
                chnl = 0;
            }
        }
        public class kernelDesc
        {
            public int first;
            public int second;
            public float amount;
        }
    }
}
