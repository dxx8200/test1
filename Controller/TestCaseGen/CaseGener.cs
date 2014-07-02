using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Controller.Util;

namespace Controller.TestCaseGen
{
    class CaseGener
    {
        private const string DEFAULT_TEMPLATEPATH = @"TransConfig\Templates";

        public static void Generate(string casepath, string modelpath, string ioconfigpath,
            string outputpath, string templatepath = "")
        {
            if (String.IsNullOrWhiteSpace(templatepath))
                templatepath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), 
                    DEFAULT_TEMPLATEPATH);
            else
                templatepath = Path.GetFullPath(templatepath);

            modelpath = Path.GetFullPath(modelpath);
            if (!File.Exists(modelpath))
                throw new FileNotFoundException();
            if (!Directory.Exists(templatepath))
                throw new Exception("No Template Path Founded.\n");
            
            string[] TemplateFiles = Directory.GetFiles(templatepath, "*.xslt", SearchOption.AllDirectories);
            outputpath = Path.GetFullPath(outputpath);
            if (!Directory.Exists(outputpath))
                Directory.CreateDirectory(outputpath);
            foreach(string TemplateFile in TemplateFiles)
            {
                string OutputFile = Path.Combine(outputpath, Path.GetFileNameWithoutExtension(TemplateFile));
                try
                {
                    XsltTransformer.TransformFile(TemplateFile, casepath, OutputFile, false,
                        new Dictionary<string, object>() { { "p_model", modelpath},{"p_io", ioconfigpath}});
                }
                catch(Exception ex)
                {
                    throw new Exception(String.Format("Create Test Case File {0} Failed.\n{1}",
                        OutputFile, ex.Message));
                }
            }

        }
    }
}
