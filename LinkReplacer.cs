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
            Parallel.ForEach(Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories), f =>
            {
                Encoding encoding = GetEncoding(f);

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
                    File.WriteAllText(f, fileAsText, encoding);
            });
        }

        private static bool IsStartOfLink(EnumeratorEntry node)
        {
            return node.IsOpening && node.Inline != null && node.Inline.Tag == InlineTag.Link;
        }

        //https://stackoverflow.com/a/19283954/1768779
        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default;
        }
    }
}