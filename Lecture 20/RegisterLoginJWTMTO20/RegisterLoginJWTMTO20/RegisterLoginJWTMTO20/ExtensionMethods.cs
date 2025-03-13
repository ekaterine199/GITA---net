namespace RegisterLoginJWTMTO20
{
    public static class ExtensionMethods
    {
        public static string GetFullMessage(this Exception ex)
        {
            if (ex == null) return string.Empty;

            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }

            return string.Join("-->", messages);
        }
    }
}
