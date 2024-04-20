using Terraria;
using TShockAPI;

namespace TheObserver
{
    public class Data
    {
        public static string DataPath = Path.Combine(TShock.SavePath, "TheObserver.dat");
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
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(PreBossValue);
                sw.Write(PostEOC);
                sw.Write(PostEvilBoss);
                sw.Write(PostSkeletron);
                sw.Write(PostWOF);
                sw.Write(PostAnyMech);
                sw.Write(PostAllMech);
                sw.Write(PostPlantera);
                sw.Write(PostGolem);
                sw.Write(PostCultist);
                sw.Write(PostML);
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
            using (StreamReader sr = new StreamReader(fs))
            {
                char[] buffer = new char[4];
                sr.Read(buffer);
                
                byte[] byteBuffer = new byte[8];

                for (int i = 0; i < 4; i++) {
                    byteBuffer[i] = (byte)(buffer[i] & 0xFF00);    // 11111111 00000000
                    byteBuffer[i + 1] = (byte)(buffer[i] & 0xFF);  // 00000000 11111111
                }

                double val = BitConverter.ToDouble(byteBuffer, 0);
            }
        }
    }
}