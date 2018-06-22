using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    // 列舉常數 for 語境標籤的生命週期（何時要移除該語境標籤）
    public enum ContextLifetime
    {
        BeforeConversion,   // 只存在於點字轉換動作之前。
        DuringConversion,   // 轉點字過程中的某個時間點會消失。
        BeforeFormatDoc,    // 在點字轉換過程中，直到整份文件進行斷行之前。
        EndOfFormatDoc,     // 在整份文件斷行完畢之後即消失。
        Persistent          // 儲存檔案時會保存。
    }


    public static class ContextTagFactory
    {
        /// <summary>
        /// 根據標籤名稱建立對應的 ContextTag 物件。
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static IContextTag CreateInstance(string tagName)
        {
            switch (tagName)
            {
                case ContextTagNames.Title:
                    {
                        return new GenericContextTag(tagName, ContextLifetime.EndOfFormatDoc);
                    }

                case ContextTagNames.Delete:
                    {
                        return new GenericContextTag(tagName)
                        {
                            ConvertablePrefix = BrailleConst.DisplayText.DeleteBegin,
                            ConvertablePostfix = BrailleConst.DisplayText.DeleteEnd
                        };
                    }

                case ContextTagNames.SpecificName:
                    {
                        return new GenericContextTag(tagName)
                        {
                            ConvertablePrefix = BrailleConst.DisplayText.SpecificName
                        };
                    }

                case ContextTagNames.BookName:
                    {
                        return new GenericContextTag(tagName)
                        {
                            ConvertablePrefix = BrailleConst.DisplayText.BookName
                        };
                    }

                case ContextTagNames.BrailleTranslatorNote:
                    {
                        return new GenericContextTag(tagName)
                        {
                            ConvertablePrefix = BrailleConst.DisplayText.BrailleTranslatorNotePrefix,
                            ConvertablePostfix = BrailleConst.DisplayText.BrailleTranslatorNotePostfix
                        };
                    }

                case ContextTagNames.OrgPageNumber:
                    {
                        return new OrgPageNumberContextTag();
                    }
                case ContextTagNames.TableTopLine2:
                    {
                        return new TableTopLineContextTag();
                    }
                case ContextTagNames.TableBottomLine2:
                    {
                        return new TableBottomLineContextTag();
                    }
                case ContextTagNames.TableTopLine1:
                case ContextTagNames.TableBottomLine1:
                    {
                        return new TableSingleLineContextTag(tagName);
                    }
                case ContextTagNames.QuotationMark1:
                    {
                        return CreateQuotationMark1();
                    }
                case ContextTagNames.QuotationMark2:
                    {
                        return CreateQuotationMark2();
                    }
                case ContextTagNames.SeparatorLine:
                    {
                        return CreateSeparatorLine();
                    }
                case ContextTagNames.NoDigitSymbol:
                    {
                        return CreateNoDigitSymbol();
                    }
                default:
                    {
                        return new GenericContextTag(tagName);
                    }
            }
        }

        private static IContextTag CreateQuotationMark1()
        {
            string tagName = ContextTagNames.QuotationMark1;

            var tag = new GenericContextTag(
                            tagName,
                            ContextLifetime.DuringConversion,
                            removeTagOnConversion: true);
            var brWord = new BrailleWord("「");
            brWord.CellList.Add(BrailleCell.GetInstance(new int[] { 2, 3, 6 }));
            tag.PrefixBrailleWords.Add(brWord);

            brWord = new BrailleWord("」");
            brWord.CellList.Add(BrailleCell.GetInstance(new int[] { 3, 5, 6 }));
            tag.PostfixBrailleWords.Add(brWord);

            return tag;
        }

        private static IContextTag CreateQuotationMark2()
        {
            string tagName = ContextTagNames.QuotationMark2;

            var tag = new GenericContextTag(
                            tagName,
                            ContextLifetime.DuringConversion,
                            removeTagOnConversion: true);
            var brWord = new BrailleWord("「");
            brWord.CellList.Add(BrailleCell.GetInstance(new int[] { 2, 3, 6 }));
            tag.PrefixBrailleWords.Add(brWord);

            brWord = new BrailleWord("」");
            brWord.CellList.Add(BrailleCell.GetInstance(new int[] { 4, 5, 6 }));
            brWord.CellList.Add(BrailleCell.GetInstance(new int[] { 3, 5, 6 }));
            tag.PostfixBrailleWords.Add(brWord);

            return tag;
        }

        private static IContextTag CreateSeparatorLine()
        {
            string tagName = ContextTagNames.SeparatorLine;

            var tag = new GenericContextTag(
                            tagName,
                            ContextLifetime.DuringConversion,
                            removeTagOnConversion: true,
                            singleLine: true);

            string text = "×";
            var cell1 = BrailleCell.GetInstance(new int[] { 1, 3, 5 });
            var cell2 = BrailleCell.GetInstance(new int[] { 2, 4, 6 });

            for (int i = 0; i < 20; i++)
            {
                var brWord = new BrailleWord(text)
                {
                    IsConvertedFromTag = true
                };
                brWord.CellList.Add(cell1);
                brWord.CellList.Add(cell2);
                brWord.ContextNames = XmlTagHelper.RemoveBracket(tagName);
                tag.PrefixBrailleWords.Add(brWord);
            }
            return tag;
        }

        private static IContextTag CreateNoDigitSymbol()
        {
            string tagName = ContextTagNames.NoDigitSymbol;

            // 純粹的控制標籤，不帶任何文字、點字的空標籤。
            return new GenericContextTag(
                            tagName,
                            ContextLifetime.DuringConversion,
                            removeTagOnConversion: true,
                            singleLine: false);
        }

    }
}
