using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommonMark;
using CommonMark.Syntax;

namespace https_checker
{
    internal static class LinkReplacer
    {
        public static void ReplaceIn(string dir)
        {
            Parallel.ForEach(Directory.GetFiles(dir, "*.md", SearchOption.AllDirectories), f =>
            {
                var originalFile = File.ReadAllText(f);
                var fileAsText = originalFile;
                var document = CommonMarkConverter.Parse(originalFile);

                foreach (var node in document.AsEnumerable())
                {
                    if (node.IsOpening && node.Inline != null && node.Inline.Tag == InlineTag.Link)
                    {
                        var targetUrl = node.Inline.TargetUrl;
                        if (targetUrl.StartsWith("http://"))
                        {
                            var httpsUrl = HttpsChecker.FindHttpsFor(targetUrl);
                            if (httpsUrl != null)
                                fileAsText = fileAsText.Replace(targetUrl, httpsUrl);
                        }
                    }
                }

                if (fileAsText != originalFile)
                    File.WriteAllText(f, fileAsText);
            });
        }
    }
}