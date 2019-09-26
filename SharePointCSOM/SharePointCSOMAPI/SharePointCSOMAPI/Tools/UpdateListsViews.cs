using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    class UpdateListsViews
    {
        public static void Run(ClientContext context)
        {
            using (var reader = new System.IO.StreamReader(""))
            {
                Web web = null;
                List list = null;

                while (reader.EndOfStream)
                {
                    try
                    {
                        var line = reader.ReadLine();
                        var data = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (web == null || string.Equals(web.ServerRelativeUrl, data[0], StringComparison.OrdinalIgnoreCase))
                        {
                            web = context.Site.OpenWeb(data[0]);
                            context.Load(web, w => w.ServerRelativeUrl);
                        }
                        if (list == null || !string.Equals(list.RootFolder.ServerRelativeUrl, data[1], StringComparison.OrdinalIgnoreCase))
                        {
                            list = web.GetList(data[1]);
                            context.Load(list.Views);
                            context.Load(list.RootFolder, f => f.ServerRelativeUrl);
                            context.ExecuteQuery();
                        }
                        foreach (var view in list.Views)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(data[2]))
                                {
                                    continue;
                                }
                                if (string.Equals(view.Title, data[2], StringComparison.OrdinalIgnoreCase))
                                {
                                    var dataChanged = false;
                                    var hidden = Convert.ToBoolean(data[3]);
                                    var mobileView = Convert.ToBoolean(data[4]);
                                    var mobileDefaultView = Convert.ToBoolean(data[5]);
                                    if (view.Hidden != hidden)
                                    {
                                        view.Hidden = hidden;
                                        dataChanged = true;
                                    }

                                    if (view.MobileView != mobileView)
                                    {
                                        view.MobileView = mobileView;
                                        dataChanged = true;
                                    }

                                    if (view.MobileDefaultView != mobileDefaultView)
                                    {
                                        view.MobileDefaultView = mobileDefaultView;
                                        dataChanged = true;
                                    }
                                    if (dataChanged)
                                    {
                                        view.Update();
                                        context.ExecuteQuery();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error1: {0}", e.ToString());
                            }
                            Console.WriteLine("Finsih view:{0},list url: {1}", view.Title, list.RootFolder.ServerRelativeUrl);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error2: {0}", e.ToString());
                    }
                }
            }

        }
    }
}
