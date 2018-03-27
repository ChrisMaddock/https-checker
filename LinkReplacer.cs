using System.IO;
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
                    if (!IsStartOfLink(node))
                        continue;

                    var targetUrl = node.Inline.TargetUrl;
                    if (!targetUrl.StartsWith("http://"))
                        continue;

                    var httpsUrl = HttpsChecker.FindUnredirectedHttpsFor(targetUrl);
                    if (httpsUrl == null)
                        continue;

                    fileAsText = fileAsText.Replace(targetUrl, httpsUrl);
                }

                if (fileAsText != originalFile)
                    File.WriteAllText(f, fileAsText);
            });
        }

        private static bool IsStartOfLink(EnumeratorEntry node)
        {
            return node.IsOpening && node.Inline != null && node.Inline.Tag == InlineTag.Link;
        }
    }
}