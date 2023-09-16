using Newtonsoft.Json;

namespace ProtectedMobs
{
    public class Config
    {
        public int[] ProtectedMob = { -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public string KilledMessage = "{1} Were killed for murdering a {0}";
        public byte Red = 255;
        public byte Blue = 255;
        public byte Green = 255;

        public void Write()
        {
            File.WriteAllText(ProtectedMobs.path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read()
        {
            if (!File.Exists(ProtectedMobs.path))
                return new Config();
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ProtectedMobs.path));
        }
    }
}
