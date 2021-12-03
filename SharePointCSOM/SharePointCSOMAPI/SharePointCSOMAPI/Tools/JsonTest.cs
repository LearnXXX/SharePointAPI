using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    class JsonTest
    {
        public static List<string> mUnSupportedFormControlTypeUniqueId = new List<string> {
                "7733d5bf-11c6-4bdc-a430-79c3065a796c",//Sql Request local默认支持
                "aeada2b6-24ad-46e2-894f-562c2a01d38a",//Web Request local 默认支持
                "ff9f65fe-f979-4312-a35b-50f0d3769069",//Change Content Type local 默认支持
                "c0a89c70-0781-4bd4-8623-f73675005e21",//External Data Column  在同一个Service下，是支持的
                "2c285c16-d4e6-49eb-8a6a-d9aa41e9e71b",//List Item   online不支持，local需要做替换。
                "4420d111-8869-49bb-8685-c1b6cdec4873",//List View   online不支持，local需要做替换。
                "2212c7db-a29d-4666-86dd-14e8ad4b3fc9",//Workflow Diagram   online不支持，local默认支持
            };
        public static void Test()
        {
            var content = System.IO.File.ReadAllText(@"C:\Users\xluo\Desktop\JsonTest.txt");
            var jobject = JObject.Parse(content);
            List<JToken> temps = new List<JToken>();
            List<string> AAA = new List<string>();
            foreach (var control in jobject.SelectToken("FormControls"))
            {
                var value1 = control.Value<string>("FormControlTypeUniqueId");
                //var value1 = (control.SelectToken("FormControlTypeUniqueId").Value<string>();//.Value.ToString();
                //var value1 = control.SelectToken("FormControlTypeUniqueId").Value.ToString();
                if (mUnSupportedFormControlTypeUniqueId.Contains(value1.ToLower()))
                {
                    temps.Add(control);
                    AAA.Add(control.Value<string>("UniqueId"));
                }
            }
            foreach (var temp in temps)
            {
                temp.Remove();
                //jobject.SelectTokens("FormControls")
                //(jobject.FormControls as JArray).Remove(temp as JObject);
            }
            var jDynamic = jobject as dynamic;
            var temp2 = new List<JToken>();
            foreach (var formLayouts in jobject.SelectToken("FormLayouts"))
            {
                foreach (var controlLayouts in formLayouts.SelectToken("FormControlLayouts"))
                {
                    GetUnsupportCOntrolLayout(temp2, AAA, controlLayouts);
                }
            }
            foreach (var temp in temp2)
            {
                temp.Remove();
            }

            //foreach (dynamic formLayouts in jDynamic.FormLayouts)
            //{
            //    foreach (dynamic controlLayouts in formLayouts.FormControlLayouts)
            //    {
            //        GetUnsupportCOntrolLayout(new List<JObject>(), AAA, controlLayouts);
            //        //if (controlLayouts.FormControlLayouts != null && controlLayouts.FormControlLayouts.Count > 0)
            //        //{
            //        //    var ssdfdf = controlLayouts.FormControlLayouts[0].FormControlUniqueId.Value;
            //        //}
            //        //var value = controlLayouts.FormControlUniqueId.Value;
            //        //if (AAA.Contains(value))
            //        //{
            //        //    temp2.Add(controlLayouts as JObject);
            //        //}
            //    }
            //}
            System.IO.File.WriteAllText(@"C:\Users\xluo\Desktop\JsonTest2.txt", jobject.ToString());
        }
        private static void GetUnsupportCOntrolLayout(List<JToken> temp2, List<string> AAA, JToken formControlLayout)
        {
            if (formControlLayout.SelectToken("FormControlLayouts") != null)
            {
                foreach (var controlLayouts in formControlLayout.SelectToken("FormControlLayouts"))
                {
                    GetUnsupportCOntrolLayout(temp2, AAA, controlLayouts);
                }
              
            }

            var value = formControlLayout.Value<string>("FormControlUniqueId");
            if (AAA.Contains(value))
            {
                temp2.Add(formControlLayout);
            }

        }


        private static void GetUnsupportCOntrolLayout(List<JObject> temp2, List<string> AAA, dynamic formControlLayout)
        {
            if (formControlLayout.FormControlLayouts != null && formControlLayout.FormControlLayouts.Count > 0)
            {
                temp2 = new List<JObject>();
                foreach (var controlLayouts in formControlLayout.FormControlLayouts)
                {
                    GetUnsupportCOntrolLayout(temp2, AAA, controlLayouts);
                }
                foreach (var temp in temp2)
                {
                    (formControlLayout.FormControlLayouts as JArray).Remove(temp as JObject);
                }
            }

            var value = formControlLayout.FormControlUniqueId.Value;
            if (AAA.Contains(value))
            {
                temp2.Add(formControlLayout as JObject);
            }

        }
    }
}
