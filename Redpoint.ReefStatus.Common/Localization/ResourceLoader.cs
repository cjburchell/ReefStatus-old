namespace RedPoint.ReefStatus.Common.Localization
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;

    public static class ResourceLoader
    {
        public static void LoadFileResources(Collection<ResourceDictionary> mergedDictionaries, string culture)
        {
            LoadFileResources(mergedDictionaries, "Strings", culture);
        }

        private static void LoadFileResources(Collection<ResourceDictionary> mergedDictionaries, string searchName, string culture)
        {
            string searchPattern = String.Format("{0}.{1}.xaml", searchName, culture);

            foreach (string resourceFile in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern))
            {
                ResourceDictionary dictionary = LoadFileResources(resourceFile);
                mergedDictionaries.Add(dictionary);
            }
        }

        private static ResourceDictionary LoadFileResources(string resourceFile)
        {
            var resourceUri = new Uri(resourceFile, UriKind.Absolute);
            var dictionary = new ResourceDictionary { Source = resourceUri };
            return dictionary;
        }
    }
}
