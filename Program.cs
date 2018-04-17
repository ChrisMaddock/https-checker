using System;

namespace https_checker
{
    static class Program
    {
        static void Main(string[] args)
        {
            LinkReplacer.ReplaceIn(args[0]);
        }
    }
}