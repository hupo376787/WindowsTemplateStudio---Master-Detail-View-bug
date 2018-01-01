using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;

namespace ExifInfo.Helpers
{
    public class LanguageHelper
    {

        //多语言
        public static string GetCurLanguage()
        {
            var languages = GlobalizationPreferences.Languages;
            if (languages.Count > 0)
            {
                List<string> lLang = new List<string>();
                lLang.Add("zh-cn、zh、zh-Hans、zh-hans-cn、zh-sg、zh-hans-sg");
                lLang.Add("en-us、en、en-au、en-ca、en-gb、en-ie、en-in、en-nz、en-sg、en-za、en-bz、en-hk、en-id、en-jm、en-kz、en-mt、en-my、en-ph、en-pk、en-tt、en-vn、en-zw、en-053、en-021、en-029、en-011、en-018、en-014");
                for (int i = 0; i < lLang.Count; i++)
                {
                    if (lLang[i].ToLower().Contains(languages[0].ToLower()))
                    {
                        string temp = lLang[i].ToLower();
                        string[] tempArr = temp.Split('、');

                        return tempArr[0];
                    }
                    //else
                    //    return "en-us";
                }
            }
            return "en-us";
        }

        public static string strOK_zhcn = "确定";
        public static string strOK_en = "OK";

        public static string strCancel_zhcn = "取消";
        public static string strCancel_en = "Cancel";

        public static string strRateContent_zhcn = "即将跳转到应用商店，点击确定继续";
        public static string strRateContent_en = "Ready to jump to App Store, click OK to continue";

        public static string strFeedbackContent_zhcn = "即将跳转到邮件，点击确定继续";
        public static string strFeedbackContent_en = "Ready to jump to Mail, click OK to continue";

        public static string strDisplayName_zhcn = "自然灾害";
        public static string strDisplayName_en = "Natural Hazards";

        public static string strTipExit_zh_cn = "再按一次退出程序";
        public static string strTipExit_en = "Press back once more to exit";

        public static string strTipNoData_zh_cn = "无数据返回";
        public static string strTipNoData_en = "No data returned";

        public static string strTipNetworkError_zh_cn = "网络错误。别着急，这可能是时差造成的。你可以选择另一个时间条件来解决此问题";
        public static string strTipNetworkError_en = "Network error. Don't worry, this may be caused by jet lag. Try another time filter";

        public static string strTipUnknownError_zh_cn = "未知错误，请稍后再试";
        public static string strTipUnknownError_en = "Unknown error. Please try again later";

        public static string strTipDataUpdatedAmount_zh_cn = "已更新数据：";
        public static string strTipDataUpdatedAmount_en = " Data updated";

        public static string strTipSavePicture_zh_cn = "图片已保存至：";
        public static string strTipSavePicture_en = "Picture saved to: ";
    }
}
