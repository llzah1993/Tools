using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Framework
{
    public class TranslateText : MonoBehaviour
    {

        //文本字数的限制
        public int CharacterLimit;

        //表情集合
        public Sprite[] Sprites;

        public GameObject OutputAreaBG;  //背景，用于显示文字
        public GameObject OutputAreaFG;  //前景，用于显示图片

        public string Pattern = "\\{\\w\\w\\}";   //待匹配的模式串

        private string mText;   //待转换的文本内容
        private int mPatternLength = 4;
        private Font mFont;
        private int mOutputLineWidth;
        private int mCurrentline;

        //预先加载prefab
        private Object mLineObj;
        private Object mImageObj;
        private Object mTextObj;

        private GameObject mTempTextObj;
        private Text mTempText;
        private GameObject mTempImageObj;
        private Image mTempImage;

        private enum BlockType
        {
            TEXT,
            IMAGE,
        }

        private class Block
        {
            public Block(int current_block, int current_line, string in_name, int block_height, int block_width, BlockType bt, int infont_size)
            {
                block_id = current_block;
                line_id = current_line;
                name = in_name;
                height = block_height;
                width = block_width;
                type = bt;
                font_size = infont_size;
            }

            //ParseLine()负责填充
            public int height;
            public int width;
            public int line_id;
            public int block_id;
            public string name; //sprite_name or text content
            public BlockType type;    //text or image
            public int font_size;

            //ShowMessage()负责填充
            public GameObject go_bg;
            public GameObject go_fg;
        }

        private class Line
        {
            public Line(int i)
            {
                height = width = 0;
                line_id = i;
                blocks = new List<Block>();
            }

            public int height;
            public int width;
            public int line_id;
            public GameObject go_bg;
            public GameObject go_fg;
            public List<Block> blocks;
        }

        private List<Line> mLines;  //行构成的list

        void SetProperty()
        {
            mOutputLineWidth = (int)OutputAreaBG.GetComponent<RectTransform>().rect.width;
            mLines = new List<Line>();
            mCurrentline = 0;
            mLineObj = Resources.Load("Prefabs/Line");
            mImageObj = Resources.Load("Prefabs/ImageBlock");
            mTextObj = Resources.Load("Prefabs/TextBlock");
            mTempTextObj = (GameObject)Instantiate(mTextObj);
            mTempText = mTempTextObj.GetComponent<Text>();
            mTempImageObj = (GameObject)Instantiate(mImageObj);
            mTempImage = mTempImageObj.GetComponent<Image>();
        }

        /// <summary>
        /// 表情转移字符定义
        /// </summary>
        private List<string> m_Symbols = new List<string> { "{00}", "{01}", "{02}", "{03}", "{04}", "{05}", "{06}", "{07}", "{08}", "{09}", "{10}" };

        private MatchCollection m_matchs;   //匹配到的转义字符 构成的集合(不一定有效，只是形式上和有效的转义字符一样)
        private List<Match> m_realMatchs;   //匹配到的有效转义字符 构成的list

        void PreProcessSubmitText(string content)
        {
            mText = content.Length > CharacterLimit ? content.Substring(0, CharacterLimit) : content;  //限制输入字符数

            m_matchs = Regex.Matches(mText, Pattern);
            m_realMatchs = new List<Match>();
            if (m_matchs.Count > 0)
                for (int i = 0; i < m_matchs.Count; i++)
                {
                    Match item = m_matchs[i];
                    if (m_Symbols.IndexOf(item.Value) > -1)
                        m_realMatchs.Add(item);
                }
        }

        int SeperateLine(string text, int start_index, int end_index, int left_width)
        {
            int i = start_index;
            while (i <= end_index)
            {
                mTempText.text = mText.Substring(start_index, i - start_index + 1);
                int block_width = (int)mTempText.preferredWidth;
                if (block_width + left_width > mOutputLineWidth)
                    return i - 1;
                else
                    i++;
            }
            return start_index;
        }

        void ParseTextBlock(ref int start_index, int end_index, ref int current_line, ref int left_width, ref int current_block, int sprite_index)
        {
            if (end_index < start_index)
                return;

            mTempText.text = mText.Substring(start_index, end_index - start_index + 1);
            int block_width = (int)mTempText.preferredWidth;
            int block_height = (int)mTempText.preferredHeight;

            if (block_width + left_width <= mOutputLineWidth)
            {   //不用换行的情况
                Block new_block = new Block(current_block, current_line, mTempText.text, block_height, block_width, BlockType.TEXT, mTempText.fontSize);
                mLines[current_line].blocks.Add(new_block);
                mLines[current_line].height = Mathf.Max(mLines[current_line].height, new_block.height);
                ++current_block;
                left_width += block_width;
            }
            else
            {    //要换行
                int seperate_index = SeperateLine(mTempText.text, start_index, end_index, left_width);
                ParseTextBlock(ref start_index, seperate_index, ref current_line, ref left_width, ref current_block, sprite_index);  //处理本行的末尾，该函数只递归一次
                mLines.Add(new Line(++current_line));    //创建新行
                mLines[current_line].width = mOutputLineWidth;
                start_index = seperate_index + 1;
                left_width = 0;
                current_block = 0;
                ParseTextBlock(ref start_index, end_index, ref current_line, ref left_width, ref current_block, sprite_index);  //处理下一行的开头，可能递归多次
            }
        }

        void ParseImageBlock(ref int current_line, ref int left_width, ref int current_block, string sprite_name)
        {
            mTempImage.sprite = Sprites[0];
            int block_width = (int)mTempImage.preferredWidth;
            int block_height = (int)mTempImage.preferredHeight;
            //Debug.Log("block_width = " + block_width.ToString() + ",block_height = " + block_height.ToString());
            if (block_width + left_width <= mOutputLineWidth)
            {   //不用换行的情况
                Block new_block = new Block(current_block++, current_line, sprite_name, block_height, block_width, BlockType.IMAGE, mTempText.fontSize);
                mLines[current_line].blocks.Add(new_block);
                mLines[current_line].height = Mathf.Max(mLines[current_line].height, new_block.height);
                left_width += block_width;
            }
            else
            {    //要换行
                mLines.Add(new Line(++current_line));    //创建新行
                mLines[current_line].width = mOutputLineWidth;
                left_width = 0;
                current_block = 0;
                ParseImageBlock(ref current_line, ref left_width, ref current_block, sprite_name);  //图片大小是受限的，不会超过行宽，所以本函数最多递归一次
            }
        }

        void ParseLine(int start_line)
        {
            if (mText.Length < 1)
                return;

            mLines.Add(new Line(mCurrentline));
            mLines[mCurrentline].width = mOutputLineWidth;  //行宽由OutputArea的宽度决定

            int current_block = 0;  //当前正在处理的块号
            int start_index = 0;
            int end_index = 0;
            int left_width = 0; // 从左向右看当前行已经预留的宽度值，用于判断是否需要换行

            foreach (Match item in m_realMatchs)
            {
                //文字部分
                end_index = item.Index - 1;
                ParseTextBlock(ref start_index, end_index, ref mCurrentline, ref left_width, ref current_block, item.Index);

                //表情部分
                ParseImageBlock(ref mCurrentline, ref left_width, ref current_block, item.Value);

                //从表情结束的地方继续
                start_index = item.Index + mPatternLength;
            }

            //处理最后一部分文本
            if (start_index < mText.Length)
                ParseTextBlock(ref start_index, mText.Length - 1, ref mCurrentline, ref left_width, ref current_block, mText.Length);

            mCurrentline++;

            ShowMessage(start_line);
        }

        void CreateLineBG(Line line)
        {
            line.go_bg = (GameObject)Instantiate(mLineObj);
            line.go_bg.name = "line" + line.line_id.ToString();
            line.go_bg.transform.parent = OutputAreaBG.transform;

            OutputAreaBG.GetComponent<VerticalLayout>().SetLinePosition(line.go_bg, line.width, line.height);
        }

        void CreateLineFG(Line line)
        {
            line.go_fg = (GameObject)Instantiate(mLineObj);
            line.go_fg.name = "line" + line.line_id.ToString();
            line.go_fg.transform.parent = OutputAreaFG.transform;

            OutputAreaFG.GetComponent<VerticalLayout>().SetLinePosition(line.go_fg, line.width, line.height);
        }

        void CreateBlockBG(Block block, Line line, ref int start_offset)
        {
            //Object obj;
            if (block.type == BlockType.IMAGE)
                block.go_bg = (GameObject)Instantiate(mImageObj);
            else
                block.go_bg = (GameObject)Instantiate(mTextObj);

            block.go_bg.transform.parent = line.go_bg.transform;

            RectTransform rt = block.go_bg.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(block.width, line.height);
            rt.localScale = new Vector3(1f, 1f, 1f);
            rt.localPosition = new Vector3(start_offset + block.width / 2, 0f, 0f);

            start_offset += block.width;

            if (block.type == BlockType.IMAGE)
                block.go_bg.GetComponent<Image>().sprite = Sprites[0];
            else
            {
                Text Text_bg = block.go_bg.GetComponent<Text>();
                Text_bg.text = block.name;
                Text_bg.fontSize = block.font_size;
            }
        }

        void CreateBlockFG(Block block, Line line, ref int start_offset)
        {
            Object obj;
            if (block.type == BlockType.IMAGE)
                obj = Resources.Load("Prefabs/ImageBlock");
            else
                obj = Resources.Load("Prefabs/TextBlock");

            block.go_fg = (GameObject)Instantiate(obj);
            block.go_fg.transform.parent = line.go_fg.transform;

            RectTransform rt = block.go_fg.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(block.width, line.height);
            rt.localScale = new Vector3(1f, 1f, 1f);
            rt.localPosition = new Vector3(start_offset + block.width / 2, 0f, 0f);

            start_offset += block.width;

            if (block.type == BlockType.IMAGE)
                block.go_fg.GetComponent<Image>().sprite = Sprites[0];
            else
            {
                Text Text_fg = block.go_fg.GetComponent<Text>();
                Text_fg.text = block.name;
                Text_fg.fontSize = block.font_size;
            }
        }

        void ShowMessage(int start_line)
        {
            for (int i = start_line; i < mLines.Count; i++)
            {
                Line line = mLines[i];
                CreateLineBG(line);
                CreateLineFG(line);

                int start_offset_bg = -(line.width / 2);
                int start_offset_fg = -(line.width / 2);
                foreach (Block block in line.blocks)
                {
                    CreateBlockBG(block, line, ref start_offset_bg);
                    CreateBlockFG(block, line, ref start_offset_fg);
                }

                #region Disable useless gameobjects
                foreach (Block block in line.blocks)
                {
                    if (block.type == BlockType.IMAGE)
                        block.go_bg.SetActive(false);
                    else
                        block.go_fg.SetActive(false);
                }
                #endregion
            }

        }   //end of ShowMessage()

        public void SubmitMessage(string content)
        {
            SetProperty();
            PreProcessSubmitText(content);
            ParseLine(mCurrentline);
        }

    }
}

