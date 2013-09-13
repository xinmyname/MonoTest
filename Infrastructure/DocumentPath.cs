using System;
using System.IO;

namespace MonoTest.Infrastructure
{
    public static class DocumentPath
    {
        public static string Get()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return String.Format("{0}{1}Documents",
                                     Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                     Path.DirectorySeparatorChar);

            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public static string For(string name, string extension)
        {
            return String.Format("{1}{0}{4}{0}{2}.{3}",
                                 Path.DirectorySeparatorChar,
                                 DocumentPath.Get(),
                                 name,
                                 extension,
                                 typeof(DocumentPath).Assembly.GetName().Name);

        }

        public static string For(string subFolder, string name, string extension)
        {
            return String.Format("{1}{0}{4}{0}{5}{0}{2}.{3}",
                                 Path.DirectorySeparatorChar,
                                 DocumentPath.Get(),
                                 name,
                                 extension,
                                 typeof(DocumentPath).Assembly.GetName().Name,
                                 subFolder);
        }

        public static string For(string subFolder1, string subFolder2, string name, string extension)
        {
            return String.Format("{1}{0}{4}{0}{5}{0}{6}{0}{2}.{3}",
                                 Path.DirectorySeparatorChar,
                                 DocumentPath.Get(),
                                 name,
                                 extension,
                                 typeof(DocumentPath).Assembly.GetName().Name,
                                 subFolder1,
                                 subFolder2);
        }
    }
}