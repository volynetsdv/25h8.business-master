
using System;
using System.IO;
using System.Reflection;
using BackgroundTasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestJsonMethod()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("TestFiles")
                .GetAwaiter().GetResult();
            var file = folder.GetFileAsync("json.text").GetAwaiter().GetResult();

            var jsonText = Windows.Storage.FileIO.ReadTextAsync(file).GetAwaiter().GetResult();

            var runClass = new RunClass();

            MethodInfo methodInfo = typeof(RunClass).GetMethod("SaveData", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { jsonText };
            methodInfo.Invoke(runClass, parameters);

           

            //var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            //var folder = localFolder.GetFolderAsync("TestFiles").GetResults();
            //var file = folder.GetFileAsync("json.text").GetResults();
        }
    }
}
