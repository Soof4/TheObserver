using System.Reflection;
using Terraria;
using TShockAPI;

namespace TheObserver
{
    public class Data
    {
        private static string DataPath = Path.Combine(TShock.SavePath, "TheObserver.dat");
        public static double PreBossValue { get; set; } = 1;
        public static double PostEOC { get; set; } = 2;
        public static double PostEvilBoss { get; set; } = 3;
        public static double PostSkeletron { get; set; } = 4;
        public static double PostWOF { get; set; } = 5;
        public static double PostAnyMech { get; set; } = 5.5;
        public static double PostAllMech { get; set; } = 6;
        public static double PostPlantera { get; set; } = 6.5;
        public static double PostGolem { get; set; } = 7;
        public static double PostCultist { get; set; } = 7.5;
        public static double PostML { get; set; } = 8;

        public static void Write()
        {
            using (FileStream fs = new FileStream(DataPath, FileMode.Create))
            {
                PropertyInfo[] properties = typeof(Data).GetProperties(BindingFlags.Static | BindingFlags.Public);

                foreach (PropertyInfo p in properties)
                {
                    byte[] byteArray = new byte[sizeof(double)];
                    BitConverter.TryWriteBytes(byteArray, (double)p.GetValue(null)!);

                    fs.Write(byteArray);
                }
            }
        }
        public static void Read()
        {
            if (!File.Exists(DataPath))
            {
                Write();
                return;
            }

            using (FileStream fs = new FileStream(DataPath, FileMode.Open))
            {
                PropertyInfo[] properties = typeof(Data).GetProperties(BindingFlags.Static | BindingFlags.Public);

                foreach (PropertyInfo p in properties)
                {
                    Span<byte> bytes = new byte[sizeof(double)];

                    fs.Read(bytes);
                    double val = BitConverter.ToDouble(bytes);

                    p.SetValue(null, val);
                }
            }
        }
    }
}
