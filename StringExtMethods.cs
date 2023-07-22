namespace RP.SOI.DotNet.Utils;

public static class StringExtMethods
{
    // Extension Method to Stretch a String
    public static string Stretch(this String str)
    {
        if (str is null)
        {
            throw new ArgumentNullException(nameof(str));
        }
        string newstr = "";
        for (int i = 0; i < str.Length; i++)
        {
            newstr += str.Substring(i, 1) + " ";
        }
        //TODO Task 1: Refer to student pdf for code.
        return newstr.ToUpper();
    }

    // Extension Method to uppercase and lowercase 
    // alternate characters in a String
    public static string UpperLower(this String str)
    {
        string newstr = "";
        for (int i = 0; i < str.Length; i++)
        {
            if (i % 2 == 0)
                newstr += str.Substring(i, 1).ToUpper();
            else
                newstr += str.Substring(i, 1).ToLower();
        }
        return newstr;
    }
}
// 21011435 Damien Foo