using System;
using System.Text;

public static class CUltilities {

    public static string APP_URL = "http://www.gomousestudio.com/";

    public static string ToUTF8(this string value)
    {
        var utf8String = Encoding.UTF8.GetBytes(value);
        string final = Encoding.UTF8.GetString(utf8String);
        return final;
    }
    
}
