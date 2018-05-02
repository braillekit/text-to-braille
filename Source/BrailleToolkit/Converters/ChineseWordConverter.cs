using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Data;
using Huanlin.Common.Helpers;
using NChinese;
using NChinese.Phonetic;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// 中文點字轉換器。
    /// </summary>
    public sealed class ChineseWordConverter : WordConverter
    {
        private ChineseBrailleTable _brailleTable;

        public ZhuyinReverseConverter ZhuyinConverter { get; set; }

        public ChineseWordConverter(ZhuyinReverseConverter zhuyinConverter)
            : base()
        {
            _brailleTable = ChineseBrailleTable.GetInstance();
            ZhuyinConverter = zhuyinConverter ?? throw new ArgumentNullException(nameof(zhuyinConverter));
        }

        /// <summary>
        /// 從堆疊中讀取字元，並轉成點字。只處理中文字和中文標點符號。
        /// </summary>
        /// <param name="charStack">字元堆疊。</param>
        /// <param name="context">情境物件。</param>
        /// <returns>傳回轉換後的點字物件串列，若串列為空串列，表示沒有成功轉換的字元。</returns>
        public override List<BrailleWord> Convert(Stack<char> charStack, ContextTagManager context)
        {
            if (charStack.Count < 1)
                throw new ArgumentException("傳入空的字元堆疊!");

            bool done = false;
            char ch;
            string text;
            bool isExtracted;	// 目前處理的字元是否已從堆疊中移出。
            BrailleWord brWord;
            List<BrailleWord> brWordList = null;
            int idx = 0;
            int chineseStartIdx = -1;
            int chineseEndIdx = -1;

            while (!done && charStack.Count > 0)
            {
                ch = charStack.Peek();   // 只讀取但不從堆疊移走。
                isExtracted = false;

                // 小於跟大於符號是情境標籤，不處理（交由 ControlTagConverter 負責）。
                if (ch == '<' || ch == '>')
                {
                    break;
                }

                // 全形英文字母要轉成半形，並且由英文點字元件處理。
                if (CharHelper.IsFullShapeLetter(ch))
                {
                    charStack.Pop();
                    ch = CharHelper.FullShapeToAsciiLetter(ch);
                    charStack.Push(ch);
                    break;
                }

                // 全形數字要轉成半形，並且由英文點字元件處理。
                if (CharHelper.IsFullShapeDigit(ch))
                {
                    charStack.Pop();
                    ch = CharHelper.FullShapeToAsciiDigit(ch);
                    charStack.Push(ch);
                    break;
                }

                text = ch.ToString();

                // 處理雙字元的標點符號。
                if (ch == '…' || ch == '－' || ch == '─' || ch == '╴' || ch == '﹏')
                {
                    // 讀下一個字元，若是相同符號，則可略過；若不同，則下次迴圈仍需處理。
                    if (charStack.Count >= 2)
                    {
                        charStack.Pop();
                        char ch2 = charStack.Pop();
                        if (ch2 == ch)	// 如果是連續兩個刪節號或破折號。
                        {
                            text = text + text;
                            isExtracted = true;
                        }
                        else
                        {
                            // 不是連續符號，把之前取出的字元放回堆疊。
                            charStack.Push(ch2);
                            charStack.Push(ch);
                            isExtracted = false;
                        }
                    }
                }
                else if (ch == '[')	// 處理以半形中括號包住的特殊符號.
                {
                    if (charStack.Count >= 3)
                    {
                        charStack.Pop();
                        char ch2 = charStack.Pop();
                        char ch3 = charStack.Pop();
                        if (ch3 == ']' && (ch2 == '↗' || ch2 == '↘'))
                        {
                            text = text + ch2.ToString() + ch3.ToString();
                            isExtracted = true;
                        }
                        else
                        {
                            // 不是中括號包住的特殊符號，把之前取出的字元放回堆疊。
                            charStack.Push(ch3);
                            charStack.Push(ch2);
                            charStack.Push(ch);
                            isExtracted = false;
                        }
                    }
                }

                /* 特殊數字編號【1】已經改成直接定義在 xml 檔案 (2009-6-22).
                                else if (ch == '【')
                                {   // 特殊數字編號，用於選擇題的答案編號，例如: 【1】，見 ChineseBrailleTable.xml。
                                    if (charStack.Count >= 3)
                                    {
                                        charStack.Pop();
                                        char ch2 = charStack.Pop();
                                        char ch3 = charStack.Pop();
                                        if (Char.IsDigit(ch2) && ch3 == '】')   // 無論是半形或全形數字，Char.IsDigit 都會傳回 true。
                                        {
                                            isExtracted = true;
                                            if (CharHelper.IsFullShapeDigit(ch2))
                                            {
                                                ch2 = CharHelper.FullShapeToAsciiDigit(ch2); 
                                            }
                                            text = ch.ToString() + ch2.ToString() + ch3.ToString();
                                        }
                                        else
                                        {
                                            charStack.Push(ch3);
                                            charStack.Push(ch2);
                                            charStack.Push(ch);
                                            isExtracted = false;
                                        }
                                    }
                                }
                */
                brWord = InternalConvert(text);
                if (brWord == null)
                    break;

                brWord.ContextNames = context.ContextNames;

                if (!isExtracted)
                {
                    charStack.Pop();
                }

                if (!StrHelper.IsEmpty(text))   // 避免將空白字元也列入 Chinese。
                    brWord.Language = BrailleLanguage.Chinese;

                ApplyBrailleConfig(brWord); // 根據組態檔的設定調整點字轉換結果。

                if (brWordList == null)
                {
                    brWordList = new List<BrailleWord>();
                }
                brWordList.Add(brWord);

                // 記錄連續中文字元，以修正破音字的注音字根。
                if (brWord.Text.IsCJK())   // 如果是中文字元，要記錄連續的中文字元區間
                {
                    if (chineseStartIdx < 0)
                    {
                        chineseStartIdx = idx;
                        chineseEndIdx = idx;
                    }
                    else
                    {
                        chineseEndIdx = idx;
                    }
                }
                else  // 不是中文字，則把之前紀錄的連續中文字取出，並修正其注音字根
                {
                    FixPhoneticCodes(brWordList, chineseStartIdx, chineseEndIdx);
                    chineseStartIdx = -1;
                    chineseEndIdx = -1;
                }
                idx++;
            }

            // 處理這段中文字的最後一段連續中文字。
            if (chineseStartIdx >= 0)
            {
                FixPhoneticCodes(brWordList, chineseStartIdx, chineseEndIdx);
            }

            return brWordList;
        }

        /// <summary>
        /// 修正一串連續中文字的注音字根。
        /// 這裡是利用新注音的智慧型詞彙判斷功能，以修正特定破音字的注音字根。
        /// </summary>
        /// <param name="brWordList">點字串列。</param>
        /// <param name="startIdx">起始索引。</param>
        /// <param name="endIdx">終止索引。</param>
        private void FixPhoneticCodes(List<BrailleWord> brWordList, int startIdx, int endIdx)
        {
            if (startIdx < 0 || endIdx < 0)
                return;
            if ((endIdx - startIdx + 1) < 2)    // 連續的中文字數若未達兩個字以上，就不處理
                return;

            // 連續的中文字元個數大於 1，使用新注音取得中文片語的注音字根，以修正破音字的字根。


            // 取出連續中文字。
            StringBuilder sb = new StringBuilder();
            for (int i = startIdx; i <= endIdx; i++)
            {
                sb.Append(brWordList[i].Text);
            }

            // 取得所有中文字的注音字根。
            string[] allPhCodes = ZhuyinConverter.GetZhuyinWithPhraseTable(sb.ToString()); 
            string phCode;
            BrailleWord brWord;
            for (int wordIdx = 0; wordIdx < allPhCodes.Length; wordIdx++)
            {
                phCode = allPhCodes[wordIdx];
                brWord = brWordList[startIdx + wordIdx];
                if (Zhuyin.IsEqual(brWord.PhoneticCode, phCode))    // 如果跟原有的注音字根相同，就略過
                    continue;

                // 將注音字根轉換成點字碼
                BrailleCellList cellList = CreatePhoneticCellList(phCode);
                if (cellList != null)
                {
                    brWord.CellList.Assign(cellList);
                    brWord.PhoneticCode = phCode;
                }
            }
        }

        /// <summary>
        /// 把一個字元轉換成點字。
        /// </summary>
        /// <param name="text">一個中文字或標點符號。</param>
        /// <returns>若指定的字串是中文字且轉換成功，則傳回轉換之後的點字物件，否則傳回 null。</returns>
        private BrailleWord InternalConvert(string text)
        {
            BrailleWord brWord = new BrailleWord(text);

            string brCode;

            if (text.Length == 1)
            {
                char ch = text[0];
                                
                // 如果輸入的明眼字是注音符號，就直接傳回注音的點字碼。
                if (Zhuyin.IsBopomofo(ch))
                {
                    // 注意: 不要指定 brWord.PhoneticCode，因為注音符號本身只是個中文符號，
                    //       它並不是中文字，沒有合法的注音組字字根，因此不可指定注音碼。
                    brCode = _brailleTable.GetPhoneticCode(text);
                    brWord.AddCells(brCode);
                    return brWord;
                }
                // 如果輸入的明眼字是注音符號的音調記號，就直接傳回對應的點字碼。
                if (Zhuyin.IsTone(ch))
                {
                    // 注意: 不要指定 brWord.PhoneticCode，因為音調記號本身只是個中文符號，
                    //       它並不是中文字，沒有合法的注音組字字根，因此不可指定注音碼。
                    brCode = _brailleTable.GetPhoneticToneCode(text);
                    brWord.AddCells(brCode);

                    return brWord;
                }
            }

            // 嘗試取得該字的注音字根，若可成功取得，則將注音字根轉換成點字碼，並傳回 BrailleWord 物件。
            string phcode = null;

            if (text.IsCJK())  // 若是漢字
            {
                /* 2010-01-03: 不取得所有的注音字根，只取得一組預設的字根，且判斷是否為多音字。等到編輯時使用者要更換注音，才取出所有字根。
                    // 取得破音字的所有組字字根，每一組字根長度固定為 4 個字元，不足者以全型空白填補。
                    string[] phCodes = ZhuyinQueryHelper.GetZhuyinSymbols(text, true);  
                    if (phCodes.Length > 0)
                    {
                        brWord.SetPhoneticCodes(phCodes);
                        phcode = phCodes[0];    // 指定第一組字根為預設的字根。
                    }
                */

                // 取得注音字根
                string[] zhuyinCodes = ZhuyinConverter.GetZhuyin(text);

                //if (zhuyinCodes == null || zhuyinCodes.Length == 0)
                //{
                //    // 若 IFELanguage 無法轉換，就用內建的注音字根查詢功能。
                //    zhuyinCodes = ZhuyinQueryHelper.GetZhuyinSymbols(text, true);   // 此方法會傳回一個中文字的所有注音字根。
                //}

                if (zhuyinCodes.Length >= 1)
                {
                    phcode = zhuyinCodes[0];
                }

                if (!String.IsNullOrEmpty(phcode))
                {
                    // 設定多音字旗號屬性.
                    brWord.IsPolyphonic = ZhuyinQueryHelper.IsPolyphonic(text);

                    // TODO: 以下「將注音字根轉換成點字碼」的處理應可省略，因為 FixPhoneticCodes 會重新修正所有中文字的點字碼。

                    // 將注音字根轉換成點字碼
                    BrailleCellList cellList = CreatePhoneticCellList(phcode);
                    if (cellList != null)
                    {
                        brWord.CellList.Assign(cellList);
                        brWord.PhoneticCode = phcode;
                        return brWord;
                    }
                }
            }

            // 不是中文字，或者無法取得注音字根.

            // 處理標點符號
            string puncBrCode = _brailleTable.GetPunctuationCode(text);
            if (!String.IsNullOrEmpty(puncBrCode))
            {
                brWord.AddCells(puncBrCode);
                return brWord;
            }

            // 其它符號
            brCode = _brailleTable.Find(text);
            if (!String.IsNullOrEmpty(brCode))
            {
                brWord.AddCells(brCode);
                return brWord;
            }

            brWord.Clear();
            brWord = null;
            return null;
        }

        /// <summary>
        /// 將一個中文字的注音組字字根碼轉換成點字碼串列，並加入指定的 BrailleWord 物件。
        /// 注意：若轉換成功，會同時加入注音碼和點字碼。
        /// </summary>
        /// <param name="phcode">一個中文字的注音組字字根碼</param>
        /// <param name="brWord">欲加入點字的 BrailleWord 物件</param>
        /// <returns>成功傳回 true，失敗傳回 false。</returns>
        public BrailleCellList CreatePhoneticCellList(string phcode)
        {
            if (StrHelper.IsEmpty(phcode) || phcode.Length < 4)
                return null;   // 不是中文字

            // 先取出注音符號各部份。
            // TODO: 改成不用空白就能判斷結合韻，否則片語無法使用。
            string firstPhCode = phcode.Substring(0, 1);	// 第一個注音符號
            string secondPhCode = phcode.Substring(1, 1);	// 第二個注音符號
            string thirdPhCode = phcode.Substring(2, 1);	// 第三個注音符號
            string joinedPhCode = phcode.Substring(1, 2);	// 第二、三結合韻
            string tonePhCode = phcode.Substring(3, 1);		// 音調

            // 取出注音符號各部份的點字碼。
            string firstBrCode = _brailleTable.GetPhoneticCode(firstPhCode);
            string secondBrCode = _brailleTable.GetPhoneticCode(secondPhCode);
            string thirdBrCode = _brailleTable.GetPhoneticCode(thirdPhCode);
            string toneBrCode = _brailleTable.GetPhoneticToneCode(tonePhCode);

            if (firstBrCode == null && secondBrCode == null && thirdBrCode == null)
            {
                return null;
            }

            BrailleCellList cellList = new BrailleCellList();

            // 處理特殊的單音字。
            if (StrHelper.IsEmpty(secondPhCode) && StrHelper.IsEmpty(thirdPhCode))
            {
                string monoBrCode = _brailleTable.GetPhoneticMonoCode(firstPhCode);
                if (String.IsNullOrEmpty(monoBrCode))
                {
                    throw new Exception("無效的注音符號: " + phcode);
                }
                cellList.Add(monoBrCode);

                // 特殊單音字要附加 'ㄦ'
                string erBrCode = _brailleTable.GetPhoneticCode("ㄦ");
                if (String.IsNullOrEmpty(erBrCode))
                {
                    throw new Exception("點字對照表中無此符號: ㄦ");
                }
                cellList.Add(erBrCode);

                // 再加上聲調
                cellList.Add(toneBrCode);

                return cellList;
            }

            // 處理結合韻。				
            string joinedBrCode = _brailleTable.GetPhoneticJoinedCode(joinedPhCode);
            if (!String.IsNullOrEmpty(joinedBrCode))	// 是結合韻？
            {
                cellList.Add(firstBrCode);	// 加入第一個注音符號
                cellList.Add(joinedBrCode);	// 加入結合韻					
                cellList.Add(toneBrCode);	// 加入聲調               
                return cellList;
            }

            // 不是特殊單音字，也不是結合韻：其他注音符號拼法。例如："ㄋㄧ　ˇ"。
            cellList.Add(firstBrCode);	// 加入第一個注音符號
            cellList.Add(secondBrCode);	// 加入第二個注音符號
            cellList.Add(thirdBrCode);	// 加入第三個注音符號
            cellList.Add(toneBrCode);	// 加入聲調          
            return cellList;
        }

        /// <summary>
        /// 根據組態檔的設定調整點字轉換結果。
        /// </summary>
        /// <param name="brWord"></param>
        private void ApplyBrailleConfig(BrailleWord brWord)
        {
            // Note: use AppGlobals.Config.Braille.[property-name] to access braille configuration items.

            /* 秋華 2009-6-22：水平箭頭不用加 1246 點，其他箭頭固定都要。
             * 所以直接用 ChineseBrailleTable.xml 中定義的就可以了，毋需根據組態檔調整。
            if (brWord.Text.Equals("←") || brWord.Text.Equals("→") ||
                brWord.Text.Equals("↑") || brWord.Text.Equals("↓")) 
            {
                // 根據組態設定決定箭號前面否要加 1246 點。
                if (!BrailleConfig.ArrowWith1246Dots && brWord.Cells[0].Value == 0x2b)
                {
                    // BrailleTableCht.xml 裡面的箭頭符號已經有加上 1246 點，故需去除。
                    brWord.Cells.RemoveAt(0);
                }
            }
            */
        }

        internal override BrailleTableBase BrailleTable
        {
            get
            {
                return _brailleTable;
            }
        }
    }
}
