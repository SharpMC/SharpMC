using System;

namespace SharpMCRewrite
{
    public class ConfigFileReader
    {
        private string FileName = string.Empty;
        private string FileContents = string.Empty;
        private string[] InitialValue;
        public ConfigFileReader (string Configfile)
        {
            FileName = Configfile;
            InitialValue = new string[] {
                "#DO NOT REMOVE THIS LINE - SharpMC",
                "MaxPlayers=10",
                "Leveltype=FlatLand",
                "ProxyCheck=false",
                "OriginalIP=127.0.0.1",
                "WorldName=world"
            };
            if (Check ())
                return;
        }

        private bool Check()
        {
            if (!System.IO.File.Exists (FileName))
            {
                System.IO.File.WriteAllLines (FileName, InitialValue);
                return Check ();
            } 
            else
            {
                FileContents = System.IO.File.ReadAllText (FileName);
                if (!FileContents.Contains ("#DO NOT REMOVE THIS LINE - SharpMC"))
                {
                    System.IO.File.Delete (FileName);
                    return Check ();
                } 
                else
                {
                    return true;
                }
            }
        }

        public string ReadString(string Rule)
        {
            foreach (string Line in FileContents.Split(new string[] { "\r\n", "\n", Environment.NewLine }, StringSplitOptions.None))
            {
                if (Line.StartsWith(Rule + "="))
                {
                    string Value = Line.Split ('=') [1];
                    return Value;
                }
            }
            return "";
        }

        public int ReadInt(string Rule)
        {
            foreach (string Line in FileContents.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                if (Line.StartsWith(Rule + "="))
                {
                    string Value = Line.Split ('=') [1];
                    return Convert.ToInt32(Value);
                }
            }
            return 0;
        }
    }
}

