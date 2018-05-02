using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Tags
{
    // 列舉常數 for 語境標籤的生命週期（何時要移除該語境標籤）
    public enum ContextLifetime
    {
        BeforeConvertion,   // 只存在於點字轉換動作之前。
        BeforeFormatDoc,    // 在點字轉換過程中，直到整份文件進行斷行之前。
        EndOfFormatDoc      // 在整份文件斷行完畢之後即消失。
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
                case ContextTagNames.TableTopLine:
                    {
                        return new TableTopLineContextTag();
                    }
                case ContextTagNames.TableBottomLine:
                    {
                        return new TableBottomLineContextTag();
                    }
                default:
                    {
                        return new GenericContextTag(tagName);
                    }
            }
        }
    }
}
