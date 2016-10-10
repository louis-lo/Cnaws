using System;

namespace Cnaws.Html
{
    public sealed class HtmlParserOptions
    {
        
        internal bool scriptEnabled;
        internal bool pluginsEnabled;
        internal bool usePreHTML5ParserQuirks;
        internal uint maximumDOMTreeDepth;

        public HtmlParserOptions()
        {
            scriptEnabled = false;
            pluginsEnabled = false;
            usePreHTML5ParserQuirks = false;
            maximumDOMTreeDepth = Settings.defaultMaximumHTMLParserDOMTreeDepth;
        }
        //public HtmlParserOptions(Document)
        //{
        //    Frame* frame = document.frame();
        //    scriptEnabled = frame && frame->script().canExecuteScripts(NotAboutToExecuteScript);
        //    pluginsEnabled = frame && frame->loader().subframeLoader().allowPlugins();

        //    Settings* settings = document.settings();
        //    usePreHTML5ParserQuirks = settings && settings->usePreHTML5ParserQuirks();
        //    maximumDOMTreeDepth = settings ? settings->maximumHTMLParserDOMTreeDepth() : Settings::defaultMaximumHTMLParserDOMTreeDepth;
        //}
    }
}
