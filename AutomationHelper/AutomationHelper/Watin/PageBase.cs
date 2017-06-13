using WatiN.Core;

namespace AutomationHelper.Watin
{
    public abstract class IEWatinPageBase
    {
        public static Browser Browser;
        protected IEWatinPageBase(string url)
        {
            Browser = Browser.AttachTo<IE>(Find.ByUrl(url));
        }

        public void Close()
        {
            Browser.Close();
        }
    }
}
