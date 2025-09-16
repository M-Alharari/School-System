using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public static class LicenseManager
{
    private static string LicenseFilePath => Path.Combine(Application.StartupPath, "license.dat");
    private static string EncryptionKey = "S3cureK3y!";

    // Validate the license key (from file or manual input)
    public static bool ValidateLicense(string licenseKey, bool ignoreMachine = false)
    {
        try
        {
            string decrypted = Decrypt(licenseKey, EncryptionKey);
            string[] parts = decrypted.Split('|');
            if (parts.Length != 2)
                return false;

            string machineId = parts[0];
            DateTime expiry = DateTime.Parse(parts[1]);

            // Check machine only if not ignored
            if (!ignoreMachine && machineId != Environment.MachineName)
                return false;

            // Expiry check (2099-12-31 = unlimited)
            if (expiry < DateTime.Now)
                return false;

            return true;
        }
        catch
        {
            return false; // decryption/parsing errors → invalid
        }
    }

    // Save license to file
    public static void SaveLicense(string licenseKey)
    {
        File.WriteAllText(LicenseFilePath, licenseKey);
    }

    // Check saved license
    public static bool IsLicenseValid(bool ignoreMachine = false)
    {
        if (!File.Exists(LicenseFilePath))
            return false;

        string encrypted = File.ReadAllText(LicenseFilePath);
        return ValidateLicense(encrypted, ignoreMachine);
    }

    // Optional message
    // Optional: message showing remaining license days
    public static string GetLicenseExpiryMessage()
    {
        if (!File.Exists(LicenseFilePath))
            return "No license found. Please enter a license key.";

        string encrypted = File.ReadAllText(LicenseFilePath);
        try
        {
            string decrypted = Decrypt(encrypted, EncryptionKey);
            string[] parts = decrypted.Split('|');
            if (parts.Length != 2) return "Invalid license";

            DateTime expiry = DateTime.Parse(parts[1]);

            if (expiry.Year >= 2099) // unlimited license
                return "License valid indefinitely";

            TimeSpan remaining = expiry.Date - DateTime.Today;
            return remaining.Days >= 0
                ? $"License valid for {remaining.Days} more day(s)."
                : "License expired!";

        }
        catch
        {
            return "Invalid license";
        }
    }

    private static string Decrypt(string cipherText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32));
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (var aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = new byte[16]; // zero IV

            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new MemoryStream(cipherBytes))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
