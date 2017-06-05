
/// <summary>
/// 字符串处理类
/// </summary>
public class StringUtil
{
    public static string trim(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            return value.Trim();
        }
    }


    public static string ltrim(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            return value.TrimStart();
        }
    }

    public static string rtrim(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            return value.TrimEnd();
        }
    }

    public static bool isWhitespace(string character)
    {
        switch (character)
        {
            case " ":
            case "\t":
            case "\r":
            case "\n":
            case "\f":
                return true;
        }

        return false;
    }

    public static string substitute(string value, params string[] parms)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        int len = parms.Length;
        for (int i = 0; i < len; i++)
        {
            value = value.Replace("{" + i + "}", parms[i]);
        }
        return value;
    }

    /// <summary>
    /// 计算一个字符串所占用的字节数，英文占用1个，中文占用2个
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GerStringLen(string str)
    {
        int num = 0;
        foreach (char c in str)
        {
            if ((int) c < 128)
            {
                num++;
            }
            else
            {
                num += 2;
            }
        }
        return num;
    }
}
