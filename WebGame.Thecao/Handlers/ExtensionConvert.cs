namespace MsWebGame.Thecao.Handlers
{
    public static class ExtensionConvert
    {
        public static string IntToRankFormat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 1)
                val = "Kim Cương";
            else if (inputValue == 2)
                val = "Vàng";
            else if (inputValue == 3)
                val = "Bạc";
            else if (inputValue == 4)
                val = "Đồng";
            else if (inputValue == 5)
                val = "Đá";
            else
                val = inputValue.ToString();

            return val;
        }
    }
}