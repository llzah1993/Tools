/****************************************************** 
CopyRight：LeTang

FileName: RichTextManager.cs

Writer: Karajan

Create Date: 2015-11-03

Main Content(Function Name、parameters、returns) 

 ******************************************************/

namespace Framework
{

    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.UI;

    ///〈summary〉 

    ///Description：图文混排 

    ///Author：甄扬 

    ///Create Date：2015-11-03

    ///〈/summary〉 
    public class RichTextManager : MonoBehaviour
    {

        [DisplayAttribute(typeof(int), "字数限制")]
        public int characterLimit = 100;
        [DisplayAttribute(typeof(GameObject), "表情图集")]
        public GameObject emojiPrefab;
        //[DisplayAttribute(typeof(string), "输入文本")]
        //public string Content;

        [HideInInspector]
        public GameObject emojiInScene;

        /// <summary>
        /// 图文混排实例 => 图文混排实例 构成的字典
        /// </summary>
        private Dictionary<GameObject, RichTextInstance>    richTextInstanceDict;

        /// <summary>
        /// 单个图文混排实例
        /// </summary>
        private class RichTextInstance
        {
            public RichTextInstance()
            {
                go = outputAreaBG = outputAreaFG = null;
                outputLineWidth = currentline = 0;
                lines = new List<Line>();
                paragraphsList = new List<Paragraph>();
                preferredHeight = -1;
            }

            public GameObject go;
            public GameObject outputAreaBG; //背景，用于显示文字
            public GameObject outputAreaFG; //前景，用于显示表情
            public int outputLineWidth;
            public List<Line> lines;  //行构成的list
            public int currentline;   //当前在处理第Currentline行
            public List<Paragraph> paragraphsList;
            public float preferredHeight;
        }

        //挂接在本控件下的gameobjects和components
        private Text tText;
        private Image tImage;
        private UnityEngine.Object tLine;
        private UnityEngine.Object tUnderline;

        /// <summary>
        /// 表情ID => 表情构成的字典
        /// </summary>
        private Dictionary<string, Sprite> mSpriteDict = new Dictionary<string, Sprite>();  

        private string mParaStart = "<P>";
        private string mParaEnd = "</P>";
        private string mSenStart = "<S";
        private string mSenEnd = "/>";
        private string mTagColor = "c=";
        private string mTagBold = "b=";
        private string mTagItalic = "i=";
        private string mTagSize = "s=";
        private string mTagSpace = "p=";
        private string mTagWrap = "w=";
        private string mTagUnderline = "u=";
        private string mTagUnderlineWidth = "l=";
        private string mTagText = "t=";
        private string mTagImage = "m=";
        private string mSpace = " ";
        private string mPattern = "\\{\\w\\w\\}";   //匹配的模式串
        
        /// <summary>
        /// 转义字符串，用于区分表情和文字
        /// </summary>
        public string Pattern { get { return mPattern; } set { mPattern = value; } }

        private GameObject richTextPrefab;

        private enum BlockType
        {
            text,
            image,
        }

        private class Block
        {
            public Block(int current_line, string in_name, int block_height, int block_width, BlockType bt, int infont_size, Sentence sentence_item)
            {
                lineId = current_line;
                name = in_name;
                height = block_height;
                width = block_width;
                type = bt;
                fontSize = infont_size;
                item = sentence_item;
            }

            //Parse*Block()负责填充
            public int height;
            public int width;
            public int lineId;
            public string name; //sprite_name or text content
            public BlockType type;    //text or image
            public int fontSize;
            public Sentence item;   //对应的Sentence

            //Show()负责填充
            public GameObject goBG;
            public GameObject goFG;
        }

        private class Line
        {
            public Line(int i)
            {
                height = width = 0;
                lineId = i;
                blocks = new List<Block>();
            }

            public int height;
            public int width;
            public int lineId;
            public GameObject goBG;
            public GameObject goFG;
            public List<Block> blocks;
        }

        private class Sentence
        {
            public Sentence()
            {
                color = new Color();
                bold = false;
                italic = false;
                size = 1;
                space = 0;
                wrap = false;
                underline = false;
                underlineWidth = 0;
                bt = BlockType.text;
                content = "";
            }
            public Color color; //c
            public bool bold;   //b
            public bool italic; //i
            public int size;  //s
            public int space;   //p
            public bool wrap;   //w
            public bool underline;  //u
            public float underlineWidth;   //l
            public BlockType bt;    //t or m
            public string content;
        }

        private class Paragraph
        {
            public Paragraph(List<Sentence> List)
            {
                SentencesList = List;
            }

            public List<Sentence> SentencesList;
        }

        //测试用
        void Start()
        {
        }

        #region 暴露给用户的接口

        //private bool isInited = false;

        public void Init()
        {
            richTextInstanceDict = new Dictionary<GameObject, RichTextInstance>();

            tText = transform.FindChild("TextBlock").gameObject.GetComponent<Text>();
            tImage = transform.FindChild("ImageBlock").gameObject.GetComponent<Image>();
            tLine = transform.FindChild("Line").gameObject;
            tUnderline = transform.FindChild("Underline").gameObject;

            ResetAll();

            SetupEmoji();

            richTextPrefab = (GameObject)Resources.Load("UIGameDemo/RichText");

            //isInited = true;
        }

        /// <summary>
        /// 创建新的图文混排实例，用户可以将其加入到自己的控件内
        /// </summary>
        /// <param name="name"></param>
        /// <param name="go"></param>
        /// <param name="width"></param>
        public GameObject AddRichTextInstance(GameObject parentGameObject, int preferredWidth = 400)
        {
            RichTextInstance instance = new RichTextInstance();
            instance.go = Instantiate<GameObject>(richTextPrefab);
            instance.outputAreaBG = instance.go.transform.FindChild("output_bg").gameObject;
            instance.outputAreaFG = instance.go.transform.FindChild("output_fg").gameObject;
            instance.outputLineWidth = preferredWidth;
            instance.lines = new List<Line>();
            instance.currentline = 0;
            richTextInstanceDict.Add(parentGameObject, instance);
            instance.go.transform.parent = parentGameObject.transform;
            return instance.go;
        }

        /// <summary>
        /// 指定图文混排实例名字和待转换文本串，进行转换并显示
        /// </summary>
        /// <param name="go"></param>
        /// <param name="contents"></param>
        public void SetText(GameObject parentGameObject, string contents)
        {
            if (richTextInstanceDict.ContainsKey(parentGameObject) == false)
            {
                Debug.LogError("No such rich text instance existed");
                return;
            }
            RichTextInstance instance = richTextInstanceDict[parentGameObject];
            if (instance.preferredHeight < 0f) //没有调用过GetPreferredHeight接口的情况
            {
                Reset(parentGameObject);
                ParseParagraphs(instance, contents);
            }
            Show(instance, 0);
        }

        /// <summary>
        /// 获取图文混排实例需要占用的高度
        /// </summary>
        /// <param name="parentGameObject"></param>
        /// <param name="contents"></param>
        public float GetPreferredHeight(GameObject parentGameObject, string contents)
        {
            if (richTextInstanceDict.ContainsKey(parentGameObject) == false)
            {
                Debug.LogError("No such rich text instance existed");
                return -1f;
            }
            Reset(parentGameObject);
            RichTextInstance instance = richTextInstanceDict[parentGameObject];
            ParseParagraphs(instance, contents);
            instance.preferredHeight = 0f;
            foreach (Line line in instance.lines)
            {
                instance.preferredHeight += line.height;
            }
            return instance.preferredHeight;
        }

        /// <summary>
        /// 把人类可读的文本转换为类XML格式的文本
        /// </summary>
        /// <param name="humanReadableString"></param>
        /// <returns></returns>
        public string HumanReadable2XML(string humanReadableString, string color = "0x00000000", int size = 8, int bold = 0, int space = 0, int underline = 0, float underlineWidth = 1.0f, int wrapper = 0, int italic = 0)
        {
            string XMLString = ""+mParaStart;
            MatchCollection matches = Regex.Matches(humanReadableString,mPattern);
            List<Match> readMatches = new List<Match>();
            foreach (Match match in matches)
            {
                if (mSpriteDict.ContainsKey(match.Value) == true)
                {
                    readMatches.Add(match);
                }
            }
            int startIndex = 0;
            foreach (Match match in readMatches)
            {
                string prevString = humanReadableString.Substring(startIndex,match.Index-startIndex);
                AddXMLSentence(ref XMLString, prevString, color, size, bold, space, underline, underlineWidth, wrapper, italic);

                XMLString += mSenStart;
                XMLString = XMLString + mSpace + mTagUnderline + underline.ToString();
                XMLString = XMLString + mSpace + mTagUnderlineWidth + underlineWidth.ToString();
                XMLString = XMLString + mSpace + mTagImage + match.Value;
                XMLString += mSenEnd;

                startIndex = match.Index + match.Value.Length;
            }

            AddXMLSentence(ref XMLString, humanReadableString.Substring(startIndex,humanReadableString.Length-startIndex), color, size, bold, space, underline, underlineWidth, wrapper, italic);
            return XMLString+mParaEnd;
        }

        /// <summary>
        /// 清空场景中所有的图文混排实例
        /// </summary>
        public void ResetAll()
        {
            foreach (RichTextInstance instance in richTextInstanceDict.Values)
            {
                for (int i = 0; i < instance.lines.Count; i++)
                {
                    Line line = instance.lines[i];
                    if (Application.isPlaying)
                    {
                        Destroy(line.goBG);
                        Destroy(line.goFG);
                    }
                    else
                    {
                        DestroyImmediate(line.goBG);
                        DestroyImmediate(line.goFG);
                    }
                }
                instance.lines.Clear();
                instance.currentline = 0;
                instance.outputAreaBG.GetComponent<VerticalLayout>().ResetLinePosition();
                instance.outputAreaFG.GetComponent<VerticalLayout>().ResetLinePosition();
            }
        }

        /// <summary>
        /// 清空场景中指定的图文混排实例
        /// </summary>
        public void Reset(GameObject gameobject)
        {
            if (richTextInstanceDict.ContainsKey(gameobject) == false)
            {
                Debug.LogError("No such rich text instance existed");
                return;
            }
            RichTextInstance instance = richTextInstanceDict[gameobject];
            for (int i = 0; i < instance.lines.Count; i++)
            {
                Line line = instance.lines[i];
                if (Application.isPlaying)
                {
                    Destroy(line.goBG);
                    Destroy(line.goFG);
                }
                else
                {
                    DestroyImmediate(line.goBG);
                    DestroyImmediate(line.goFG);
                }
            }
            instance.lines.Clear();
            instance.currentline = 0;
            instance.outputAreaBG.GetComponent<VerticalLayout>().ResetLinePosition();
            instance.outputAreaFG.GetComponent<VerticalLayout>().ResetLinePosition();
        }

        #endregion

        private void AddXMLSentence(ref string XMLString, string prevString, string color, int size, int bold, int space, int underline, float underlineWidth, int wrapper, int italic)
        {
            XMLString += mSenStart;
            XMLString = XMLString + mSpace + mTagColor + color;
            XMLString = XMLString + mSpace + mTagSize + size.ToString();
            XMLString = XMLString + mSpace + mTagBold + bold.ToString();
            XMLString = XMLString + mSpace + mTagSpace + space.ToString();
            XMLString = XMLString + mSpace + mTagUnderline + underline.ToString();
            XMLString = XMLString + mSpace + mTagUnderlineWidth + underlineWidth.ToString();
            XMLString = XMLString + mSpace + mTagWrap + wrapper.ToString();
            XMLString = XMLString + mSpace + mTagItalic + italic.ToString();
            XMLString = XMLString + mSpace + mTagText + prevString;
            XMLString += mSenEnd;
            return;
        }

        /// <summary>
        /// 根据EmojiPrefab，建立 表情ID=>表情名字 的字典，EmojiPrefab是拖到gameobject上的
        /// </summary>
        private void SetupEmoji()
        {
            if (emojiPrefab == null)
            {
                Debug.LogError("In SetEmoji: the path you give is invalid!");
                return;
            }
            if (emojiInScene == null)
            {
                emojiInScene = Instantiate(emojiPrefab);
                EmojiInfo ei = emojiInScene.GetComponent<EmojiInfo>();
                foreach (EmojiInfo.SpriteInfo sprite in ei.spriteList)
                {
                    mSpriteDict.Add(sprite.spriteID, sprite.spriteName);
                }
            }
        }

        /// <summary>
        /// 解析待转换文本内所有的paragraph
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="Paragraphs"></param>
        void ParseParagraphs(RichTextInstance instance, string paragraphs)
        {
            int i = 0;
            while (i < paragraphs.Length)
            {
                int startIndex = paragraphs.IndexOf(mParaStart, i);
                int endIndex = paragraphs.IndexOf(mParaEnd, startIndex + mParaStart.Length);
                instance.paragraphsList.Add(new Paragraph(ParseSentences(paragraphs.Substring(startIndex + mParaStart.Length, endIndex - startIndex - mParaStart.Length)))); //得到一个paragraph里的所有sentences
                AddLines(instance, instance.paragraphsList[instance.paragraphsList.Count - 1]);
                instance.currentline++; //处理完一个paragraph之后强制换行
                i = endIndex + mParaEnd.Length;
            }
        }

        /// <summary>
        /// 解析一个paragraph内所有的sentence
        /// </summary>
        /// <param name="Sentences"></param>
        /// <returns></returns>
        List<Sentence> ParseSentences(string sentences)
        {
            //Debug.Log(Sentences);
            List<Sentence> SentencesList = new List<Sentence>();
            int i = 0;
            while (i < sentences.Length)
            {
                int StartIndex = sentences.IndexOf(mSenStart, i);
                int EndIndex = sentences.IndexOf(mSenEnd, StartIndex + mSenStart.Length);
                string CurrSentence = sentences.Substring(StartIndex + mSenStart.Length, EndIndex - StartIndex - mSenStart.Length);
                if (CurrSentence.Length > characterLimit)
                    CurrSentence = CurrSentence.Substring(0, 100);
                i = EndIndex + mSenEnd.Length;
                AddSentence(SentencesList, CurrSentence);
            }
            return SentencesList;
        }

        /// <summary>
        /// 添加一个sentence到sentence集合，一个paragraph对应一个sentence集合
        /// </summary>
        /// <param name="SentencesList"></param>
        /// <param name="CurrSentence"></param>
        void AddSentence(List<Sentence> sentencesList, string currSentence)
        {   //把单个sentence添加到sentence集合
            Sentence temp = new Sentence();

            int startIndex, endIndex;

            startIndex = currSentence.IndexOf(mTagColor, 0);
            if (startIndex > 0)
            {
                startIndex += mTagColor.Length;
                endIndex = currSentence.IndexOf(" ", startIndex);
                //Debug.Log(CurrSentence.Substring(StartIndex, EndIndex - StartIndex));
                temp.color = ToColor(int.Parse(currSentence.Substring(startIndex, endIndex - startIndex), System.Globalization.NumberStyles.HexNumber));
            }

            startIndex = currSentence.IndexOf(mTagBold, 0);
            if (startIndex > 0)
            {
                startIndex += mTagBold.Length;
                endIndex = currSentence.IndexOf(" ", startIndex);
                temp.bold = int.Parse(currSentence.Substring(startIndex, endIndex - startIndex)) == 1 ? true : false;
            }

            startIndex = currSentence.IndexOf(mTagItalic, 0);
            if (startIndex > 0)
            {
                startIndex += mTagItalic.Length;
                endIndex = currSentence.IndexOf(" ", startIndex);
                temp.italic = int.Parse(currSentence.Substring(startIndex, endIndex - startIndex)) == 1 ? true : false;
            }

            startIndex = currSentence.IndexOf(mTagSize, 0);
            if (startIndex > 0)
            {
                startIndex += mTagSize.Length;
                endIndex = currSentence.IndexOf(" ", startIndex);
                temp.size = int.Parse(currSentence.Substring(startIndex, endIndex - startIndex));
            }

            startIndex = currSentence.IndexOf(mTagSpace, 0);
            if (startIndex > 0)
            {
                startIndex += mTagSpace.Length;
                endIndex = currSentence.IndexOf(" ", startIndex);
                temp.space = int.Parse(currSentence.Substring(startIndex, endIndex - startIndex));
            }

            startIndex = currSentence.IndexOf(mTagWrap, 0);
            if (startIndex > 0)
            {
                startIndex += mTagWrap.Length;
                endIndex = currSentence.IndexOf(" ", startIndex);
                temp.wrap = int.Parse(currSentence.Substring(startIndex, endIndex - startIndex)) == 1 ? true : false;
            }

            startIndex = currSentence.IndexOf(mTagUnderline, 0);
            if (startIndex > 0)
            {
                startIndex += mTagUnderline.Length;
                endIndex = currSentence.IndexOf(" ", startIndex);
                temp.underline = int.Parse(currSentence.Substring(startIndex, endIndex - startIndex)) == 1 ? true : false;
                if (temp.underline == true)
                {
                    startIndex = currSentence.IndexOf(mTagUnderlineWidth, 0);
                    if (startIndex > 0)
                    {
                        startIndex += mTagUnderline.Length;
                        endIndex = currSentence.IndexOf(" ", startIndex);
                        temp.underlineWidth = float.Parse(currSentence.Substring(startIndex, endIndex - startIndex));
                    }
                }
            }

            startIndex = currSentence.IndexOf(mTagImage, 0);
            if (startIndex > 0)
            {
                startIndex += mTagImage.Length;
                endIndex = currSentence.Length;
                temp.content = currSentence.Substring(startIndex, endIndex - startIndex);
                temp.bt = BlockType.image;
            }

            startIndex = currSentence.IndexOf(mTagText, 0);
            if (startIndex > 0)
            {
                startIndex += mTagText.Length;
                endIndex = currSentence.Length;
                temp.content = currSentence.Substring(startIndex, endIndex - startIndex);
                temp.bt = BlockType.text;
            }

            if (temp.bt == BlockType.text && temp.space > 0)
            {   //添加空格
                string spaces = "";
                for (int i = 0; i < temp.space; ++i)
                    spaces += " ";

                List<string> splited = new List<string>();
                for (int i = 0; i < temp.content.Length; ++i)
                    splited.Add(temp.content.Substring(i, 1));

                string str = "";
                for (int i = 0; i < splited.Count; ++i)
                {
                    str += splited[i];
                    if (i < splited.Count - 1)
                        str += spaces;
                }
                temp.content = str;
            }

            sentencesList.Add(temp);
        }

        /// <summary>
        /// 把十进制形式的color转换为RGBA
        /// </summary>
        /// <param name="HexVal"></param>
        /// <returns></returns>
        Color ToColor(int HexVal)
        {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return new Color32(R, G, B, 255);
        }

        /// <summary>
        /// 把解析到的一个paragraph转换并添加到内部数据结构Lines
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="paragraph"></param>
        void AddLines(RichTextInstance instance, Paragraph paragraph)
        {
            instance.lines.Add(new Line(instance.currentline));
            instance.lines[instance.currentline].width = instance.outputLineWidth;  //行宽是由用户设定的

            int left_width = 0; // 从左向右看当前行已经预留的宽度值，用于判断是否需要换行

            foreach (Sentence item in paragraph.SentencesList)
            {
                if (item.bt == BlockType.text)
                {   //文字部分
                    int start_index = 0;
                    int end_index = item.content.Length - 1;
                    ParseTextBlock(instance, ref start_index, end_index, ref left_width, item);
                }
                else
                {    //表情部分
                    ParseImageBlock(instance, ref left_width, item);
                }
            }

        }

        void ParseTextBlock(RichTextInstance instance, ref int startIndex, int endIndex, ref int leftWidth, Sentence item)
        {
            if (endIndex < startIndex)
                return;

            //设置本句的格式，加粗、斜体、下划线...
            tText.text = item.content.Substring(startIndex, endIndex - startIndex + 1);
            if (item.bold == true && item.italic == false)
            { //粗体、斜体
                tText.fontStyle = FontStyle.Bold;
            }
            else if (item.bold == true && item.italic == true)
            {
                tText.fontStyle = FontStyle.BoldAndItalic;
            }
            else if (item.bold == false && item.italic == true)
            {
                tText.fontStyle = FontStyle.Italic;
            }
            else
            {
                tText.fontStyle = FontStyle.Normal;
            }
            tText.color = item.color;   //字体颜色
            tText.fontSize = item.size; //字体大小

            //计算出本句的宽度、高度
            int block_width = (int)tText.preferredWidth;
            int block_height = (int)tText.preferredHeight;

            if (block_width + leftWidth <= instance.outputLineWidth)
            {   //不用换行
                Block new_block = new Block(instance.currentline, tText.text, block_height, block_width, BlockType.text, tText.fontSize, item);
                instance.lines[instance.currentline].blocks.Add(new_block);
                instance.lines[instance.currentline].height = Mathf.Max(instance.lines[instance.currentline].height, new_block.height);
                leftWidth += block_width;
            }
            else
            {    //要换行
                int seperate_index = FindSeperateIndex(instance, tText.text, startIndex, endIndex, leftWidth, item);
                ParseTextBlock(instance, ref startIndex, seperate_index, ref leftWidth, item);  //处理本行的末尾，该函数只递归一次
                instance.lines.Add(new Line(++instance.currentline));    //创建新行
                instance.lines[instance.currentline].width = instance.outputLineWidth;
                startIndex = seperate_index + 1;
                leftWidth = 0;
                ParseTextBlock(instance, ref startIndex, endIndex, ref leftWidth, item);  //处理下一行的开头，可能递归多次
            }
        }

        int FindSeperateIndex(RichTextInstance instance, string text, int startIndex, int endIndex, int leftWidth, Sentence item)
        {
            int i = startIndex;
            while (i <= endIndex)
            {
                tText.text = item.content.Substring(startIndex, i - startIndex + 1);
                int blockWidth = (int)tText.preferredWidth;
                if (blockWidth + leftWidth > instance.outputLineWidth)
                    return i - 1;
                else
                    i++;
            }
            return startIndex;
        }

        void ParseImageBlock(RichTextInstance instance, ref int leftWidth, Sentence item)
        {
            if (mSpriteDict.ContainsKey(item.content))
                tImage.sprite = mSpriteDict[item.content];

            int block_width = (int)tImage.preferredWidth;
            int block_height = (int)tImage.preferredHeight;

            if (block_width + leftWidth <= instance.outputLineWidth)
            {   //不用换行的情况
                Block new_block = new Block(instance.currentline, item.content, block_height, block_width, BlockType.image, tText.fontSize, item);
                instance.lines[instance.currentline].blocks.Add(new_block);
                instance.lines[instance.currentline].height = Mathf.Max(instance.lines[instance.currentline].height, new_block.height);
                leftWidth += block_width;
            }
            else
            {    //要换行
                instance.lines.Add(new Line(++instance.currentline));    //创建新行
                instance.lines[instance.currentline].width = instance.outputLineWidth;
                leftWidth = 0;
                ParseImageBlock(instance, ref leftWidth, item);  //表情图片大小是受限的，不会超过行宽，所以本函数最多递归一次
            }

        }

        void Show(RichTextInstance instance, int startLine)
        {
            for (int i = startLine; i < instance.lines.Count; i++)
            {
                Line line = instance.lines[i];
                CreateLineBG(instance, line);
                CreateLineFG(instance, line);
                int start_offset_bg = -(line.width / 2);
                int start_offset_fg = -(line.width / 2);
                foreach (Block block in line.blocks)
                {
                    if (block.type == BlockType.image)
                    {
                        CreateBlockBG(block, line, ref start_offset_bg);
                        CreateBlockFG(block, line, ref start_offset_fg);
                    }
                    else
                    {
                        CreateBlockBG(block, line, ref start_offset_bg);
                        CreateBlockFG(block, line, ref start_offset_fg);
                    }

                }

                #region Disable useless gameobjects
                foreach (Block block in line.blocks)
                {
                    if (block.type == BlockType.image)
                        block.goBG.SetActive(false);
                    else
                        block.goFG.SetActive(false);
                }
                #endregion
            }

        }

        void CreateLineBG(RichTextInstance instance, Line line)
        {
            line.goBG = (GameObject)Instantiate(tLine);
            line.goBG.name = "line" + line.lineId.ToString();
            line.goBG.transform.parent = instance.outputAreaBG.transform;

            instance.outputAreaBG.GetComponent<VerticalLayout>().SetLinePosition(line.goBG, line.width, line.height);
        }

        void CreateLineFG(RichTextInstance instance, Line line)
        {
            line.goFG = (GameObject)Instantiate(tLine);
            line.goFG.name = "line" + line.lineId.ToString();
            line.goFG.transform.parent = instance.outputAreaFG.transform;

            instance.outputAreaFG.GetComponent<VerticalLayout>().SetLinePosition(line.goFG, line.width, line.height);
        }

        void CreateBlockBG(Block block, Line line, ref int startOffset)
        {
            if (block.type == BlockType.image)
            {
                tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 1f);
                block.goBG = (GameObject)Instantiate(tImage.gameObject);
                tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0f);
            }
            else
            {
                tText.color = new Color(tText.color.r, tText.color.g, tText.color.b, 1f);
                block.goBG = (GameObject)Instantiate(tText.gameObject);
                tText.color = new Color(tText.color.r, tText.color.g, tText.color.b, 0f);
            }

            block.goBG.transform.parent = line.goBG.transform;

            RectTransform rt = block.goBG.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(block.width, line.height);
            rt.localScale = new Vector3(1f, 1f, 1f);
            rt.localPosition = new Vector3(startOffset + block.width / 2, 0f, 0f);

            startOffset += block.width;

            if (block.type == BlockType.image)
            {
                if (mSpriteDict.ContainsKey(block.item.content))
                {
                    block.goBG.GetComponent<Image>().sprite = mSpriteDict[block.item.content];
                    block.goBG.GetComponent<Image>().SetNativeSize();
                }
            }
            else
            {
                Text Text_bg = block.goBG.GetComponent<Text>();
                Text_bg.text = block.name;
                Text_bg.fontSize = block.fontSize;
            }

            if (block.item.underline == true)
            {
                AddUnderline(block.goBG, block.width, block.item.underlineWidth);
            }

            if (block.item.wrap == true)
            {
                block.goBG.AddComponent<Outline>();
            }

        }

        void CreateBlockFG(Block block, Line line, ref int startOffset)
        {
            if (block.type == BlockType.image)
            {
                tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 1f);
                block.goFG = (GameObject)Instantiate(tImage.gameObject);
                tImage.color = new Color(tImage.color.r, tImage.color.g, tImage.color.b, 0f);
            }
            else
            {
                tText.color = new Color(tText.color.r, tText.color.g, tText.color.b, 1f);
                block.goFG = (GameObject)Instantiate(tText.gameObject);
                tText.color = new Color(tText.color.r, tText.color.g, tText.color.b, 0f);
            }

            block.goFG.transform.parent = line.goFG.transform;
            RectTransform rt = block.goFG.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(block.width, line.height);
            rt.localScale = new Vector3(1f, 1f, 1f);
            rt.localPosition = new Vector3(startOffset + block.width / 2, 0f, 0f);

            startOffset += block.width;

            if (block.type == BlockType.image)
            {
                if (mSpriteDict.ContainsKey(block.item.content))
                {
                    block.goFG.GetComponent<Image>().sprite = mSpriteDict[block.item.content];
                    block.goFG.GetComponent<Image>().SetNativeSize();
                }
            }
            else
            {
                Text Text_fg = block.goFG.GetComponent<Text>();
                Text_fg.text = block.name;
                Text_fg.fontSize = block.fontSize;
            }

            if (block.item.underline == true)
            {
                AddUnderline(block.goFG, block.width, block.item.underlineWidth);
            }

            if (block.item.wrap == true)
            {
                block.goFG.AddComponent<Outline>();
            }

        }

        //添加下划线
        void AddUnderline(GameObject parent, int width, float height)
        {
            GameObject underline = (GameObject)Instantiate(tUnderline);
            underline.transform.parent = parent.transform;
            RectTransform rt = (RectTransform)underline.transform;
            rt.sizeDelta = new Vector2(width, height);
            rt.localScale = new Vector3(1f, 1f, 1f);
            rt.anchoredPosition = new Vector2(0f, 0f);
        }

    }

}