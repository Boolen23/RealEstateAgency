using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RealEstateAgency.ViewModel
{
    public static class SQLDataGenerator
    {
        //static string NamePath = @"C:\Users\OskarSparta\Desktop\СУБД\имя фамилия.txt";
        //static string StreetNamePath = @"C:\Users\OskarSparta\Desktop\СУБД\улицы.txt";
        //static string PhoneNumberPath = @"C:\Users\OskarSparta\Desktop\СУБД\номера телефонов.txt";

        static string NamePath =  @"Resources/имя фамилия.txt";
        static string StreetNamePath = @"Resources/улицы.txt";
        static string PhoneNumberPath = @"Resources/номера телефонов.txt";


        static string flatPath = @"C:\Users\OskarSparta\Desktop\СУБД\flat";
        static string hometPath = @"C:\Users\OskarSparta\Desktop\СУБД\home";
        static string sitePath = @"C:\Users\OskarSparta\Desktop\СУБД\site";
        static string commercePath = @"C:\Users\OskarSparta\Desktop\СУБД\commerce";


        public static Random random = new Random();
        #region dataGeneration
        public static DateTime RandomDate => new DateTime(2018, random.Next(1, 10), random.Next(1, 29));
        public static string RandomTransaction => random.Next() % 2 == 0 ? "Продам" : "Сдам";
        public static List<string> GetAdresses()
        {
            string[] temp = File.ReadAllLines(StreetNamePath, Encoding.Default);
            temp = temp.Where(i => i.Length > 5).ToArray();
            List<string> list = new List<string>();
            foreach (string i in temp)
            {
                if (i.IndexOf(',') > -1)
                    list.Add(i.Substring(0, i.IndexOf(',')));
                else list.Add(i);
            }
            return list;
        }
        static List<string> Purposes = new List<string>() { "ИЖС", "ДНП", "СНТ", "ЛПХ", "ДНТ" };
        static List<string> FullNames = File.ReadAllLines(NamePath, Encoding.Default).Select(i => i.Substring(0, i.LastIndexOf(' '))).ToList();
        static List<string> Adresses = GetAdresses();
        static List<string> Phones = File.ReadAllLines(PhoneNumberPath, Encoding.Default).ToList();
        public static string NextPhone => Phones[random.Next(Phones.Count)];
        public static string NextAdress => Adresses[random.Next(Adresses.Count)] + " ," + random.Next(1, 1000);
        public static string NextPurpose => Purposes[random.Next(Purposes.Count)];
        public static int NextNumberOfRooms => random.Next(1, 8);
        public static int NextFloorNumbers => random.Next(1, 5);
        public static int NextPrice => random.Next(500000, 3000001);
        public static int NextArea => random.Next(20, 201);
        public static int NextFloor => random.Next(1, 26);




        static int NameIndex = -1;
        public static string NextName
        {
            get
            {
                NameIndex = random.Next(FullNames.Count);
                return FullNames[NameIndex].Substring(FullNames[NameIndex].IndexOf(' '), FullNames[NameIndex].LastIndexOf(' ') - FullNames[NameIndex].IndexOf(' '));
            }
        }
        public static string NextSurName => FullNames[NameIndex].Substring(0, FullNames[NameIndex].IndexOf(' '));
        #endregion
        #region imageGeneration
        static List<string> FlatImgs = Images(flatPath);
        static List<string> HomeImgs = Images(hometPath);
        static List<string> SiteImgs = Images(sitePath);
        static List<string> CommerceImgs = Images(commercePath);
        static List<string> Images (string path)
        {

            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles();
            List<string> res = files.Select(i => i.FullName).ToList();
            return res;
        }

        public static string NextFlat => FlatImgs[random.Next(FlatImgs.Count)];
        public static string NextHome => HomeImgs[random.Next(HomeImgs.Count)];
        public static string NextSite => SiteImgs[random.Next(SiteImgs.Count)];
        public static string NextCommerce => CommerceImgs[random.Next(CommerceImgs.Count)];
        #endregion
    }
}
