
public class GUID
{
    public static string GenerateGUID()
    {
        System.Guid guid = System.Guid.NewGuid();
        return guid.ToString();
    }
}
