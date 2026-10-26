using System;
using System.IO;
using System.Net;

#pragma warning disable SYSLIB0014 

class Program
{
    static string server = "ftp://172.20.208.126/";
    static string user = "test";
    static string pass = "1234";

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n1 - Faylların siyahısı");
            Console.WriteLine("2 - Fayl yüklə (upload)");
            Console.WriteLine("3 - Fayl endir (download)");
            Console.WriteLine("4 - Fayl sil");
            Console.WriteLine("5 - Faylı yenilə (Put)");
            Console.WriteLine("0 - Çıxış");
            Console.Write("Seçim: ");
            string secim = Console.ReadLine();

            try
            {
                if (secim == "1") Siyahi();
                else if (secim == "2") Yukle();
                else if (secim == "3") Endir();
                else if (secim == "4") Sil();
                else if (secim == "5") Put();
                else if (secim == "0") break;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Xəta: " + ex.Message);
            }
        }
    }

    // Hər əməliyyat üçün sorğu yaradan köməkçi metod
    static FtpWebRequest Sorgu(string fayl, string metod)
    {
        var req = (FtpWebRequest)WebRequest.Create(server + fayl);
        req.Method = metod;
        req.Credentials = new NetworkCredential(user, pass);
        return req;
    }

    // 1. Serverdəki faylları göstər
    static void Siyahi()
    {
        var req = Sorgu("", WebRequestMethods.Ftp.ListDirectory);
        using var res = (FtpWebResponse)req.GetResponse();
        using var reader = new StreamReader(res.GetResponseStream());
        Console.WriteLine("--- Fayllar ---");
        Console.WriteLine(reader.ReadToEnd());
    }

    // 2. Kompüterdən serverə fayl göndər
    static void Yukle()
    {
        Console.Write("Yüklənəcək faylın yolu: ");
        string yol = Console.ReadLine();

        var req = Sorgu(Path.GetFileName(yol), WebRequestMethods.Ftp.UploadFile);
        byte[] data = File.ReadAllBytes(yol);
        using (var stream = req.GetRequestStream())
            stream.Write(data, 0, data.Length);

        using var res = (FtpWebResponse)req.GetResponse();
        Console.WriteLine("Yükləndi: " + res.StatusDescription);
    }

    // 3. Serverdən kompüterə fayl endir
    static void Endir()
    {
        Console.Write("Endiriləcək faylın adı: ");
        string ad = Console.ReadLine();

        var req = Sorgu(ad, WebRequestMethods.Ftp.DownloadFile);
        using var res = (FtpWebResponse)req.GetResponse();
        using var stream = res.GetResponseStream();
        using var file = File.Create(ad);
        stream.CopyTo(file);
        Console.WriteLine("Endirildi: " + Path.GetFullPath(ad));
    }

    static void Sil()
    {
        Console.Write("Silinəcək faylın adı: ");
        string ad = Console.ReadLine();
        var req = Sorgu(ad, WebRequestMethods.Ftp.DeleteFile);
        using var res = (FtpWebResponse)req.GetResponse();
        Console.WriteLine("Silindi: " + res.StatusDescription);
    }

    static void Put()
    {
        Console.Write("Yenilənəcək fayl adı: ");
        string ad = Console.ReadLine();

        Console.Write("Yeni fayl yolu: ");
        string yol = Console.ReadLine();

        var req = Sorgu(ad, WebRequestMethods.Ftp.UploadFile);
        byte[] data = File.ReadAllBytes(yol);
        using (var stream = req.GetRequestStream())
            stream.Write(data, 0, data.Length);

        using var res = (FtpWebResponse)req.GetResponse();
        Console.WriteLine("Yeniləndi: " + res.StatusDescription);
    }
}