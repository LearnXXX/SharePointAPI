using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI.Tools
{
    public class UserInfo : IComparable, IComparer
    {
        public string LoginName { get; set; }
        public string Email { get; set; }

        public override bool Equals(object obj)
        {
            var tempUserInfo = obj as UserInfo;
            return string.Equals(LoginName, tempUserInfo.LoginName, StringComparison.OrdinalIgnoreCase);
        }
        public int Compare(object x, object y)
        {
            var tempUserInfo1 = x as UserInfo;
            var tempUserInfo2 = y as UserInfo;
            if (string.Equals(tempUserInfo2.LoginName, tempUserInfo1.LoginName, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }
            return 1;
        }

        public int CompareTo(object obj)
        {
            var tempUserInfo = obj as UserInfo;
            if (string.Equals(LoginName, tempUserInfo.LoginName, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }
            return 1;
        }
    }
    public class CustomerUserSeatTest
    {
        public static void Run()
        {
            var FarmUsers_SP2010_20210607_0240PMUsers = ReadUserInfo(@"D:\CI\ADO-225497\Reports\Reports\FarmUsers_SP2010_20210607_0240PM.csv");
            var FarmUsers20210607_0247 = ReadUserInfo(@"D:\CI\ADO-225497\Reports\Reports\FarmUsers20210607_0247.csv");
            var V1Result = CountUserSeatV1(FarmUsers_SP2010_20210607_0240PMUsers, FarmUsers20210607_0247);
            var V2Result = CountUserSeatV2(FarmUsers_SP2010_20210607_0240PMUsers, FarmUsers20210607_0247);
        }

        public static List<UserInfo> CountUserSeatV2(List<UserInfo> userInfos1, List<UserInfo> userInfos2)
        {
            var templist = new List<UserInfo>();
            templist.AddRange(userInfos1);
            foreach (var userInfo in userInfos2)
            {
                if (!templist.Contains(userInfo))
                {
                    if (string.IsNullOrEmpty(userInfo.Email))
                    {
                        templist.Add(userInfo);
                    }
                    else
                    {
                        var findResult = userInfos1.FirstOrDefault(tempUserInfo => string.Equals(tempUserInfo.Email, userInfo.Email, StringComparison.OrdinalIgnoreCase));
                        if (findResult == null)
                        {
                            templist.Add(userInfo);
                        }
                        else
                        {

                        }
                    }
                }
            }
            return templist;
        }
        public static List<UserInfo> CountUserSeatV1(List<UserInfo> userInfos1, List<UserInfo> userInfos2)
        {
            var templist = new List<UserInfo>();
            templist.AddRange(userInfos1);
            foreach (var userInfo in userInfos2)
            {
                if (!templist.Contains(userInfo))
                {
                    templist.Add(userInfo);
                }
            }
            return templist;

        }
        private static List<UserInfo> ReadUserInfo(string userSeatFilePath)
        {
            int count1 = 0;
            var userInfos = new List<UserInfo>();
            using (var reader = new StreamReader(userSeatFilePath))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var userInfo = reader.ReadLine().Split(',');
                    var tempUserInfo = new UserInfo
                    {
                        LoginName = userInfo[0],
                        Email = userInfo[5],
                    };
                    if (!userInfos.Contains(tempUserInfo))
                    {
                        userInfos.Add(tempUserInfo);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tempUserInfo.Email))
                        {
                            var addedUserInfos = userInfos[userInfos.IndexOf(tempUserInfo)];
                            if (string.IsNullOrEmpty(addedUserInfos.Email))
                            {
                                userInfos.Remove(tempUserInfo);
                                userInfos.Add(tempUserInfo);
                            }
                            else if (!string.Equals(tempUserInfo.Email, addedUserInfos.Email, StringComparison.OrdinalIgnoreCase))
                            {
                                count1++;
                            }

                        }
                    }
                }
            }
            return userInfos;
        }

    }
}
