//Написать класс CrawlerPathLogger, который выполняет логирование директорий и файлов этих директорий на заданном диске.
// Атрибуты:
//   • DirsArr – приватный атрибут, который хранит всю иерархию папок
//   • FilePathLog – Приватный атрибут, который хранит путь к файлу
//   • LogicDiskLetter – Приватный атрибу, который хранит букву логического диска (Например, “C://”);
// Методы класса:
//   • Гетторы и сетторы для установки значений.
//   • Рекурсивный перебор всех вложенных директорий и файлов с записью их в DirsArr.
//   • Запись в текстовый файл найденные директории. Если файла нет создаёт его.
//     В текстовом файле нужно отразить следующие данные: 
//
// ДатаСозданияДиректории, ИмяДиректории    
//     ->  ДатаСозданияФайла, ИмяФайла, Расширение
//     ->  ДатаСозданияФайла, ИмяФайла, Расширение
//     -> ДатаСозданияСубДиректории, ИмяСубДиректории   
//              ->  ДатаСозданияФайла, ИмяФайла, Расширение
//              ->  ДатаСозданияФайла, ИмяФайла, Расширение
// ДатаСозданияДиректории, ИмяДиректории    
//      ->  ДатаСозданияФайла, ИмяФайла, Расширение
//      ->  ДатаСозданияФайла, ИмяФайла, Расширение
//      -> ДатаСозданияСубДиректории, ИмяСубДиректории   
//               ->  ДатаСозданияФайла, ИмяФайла, Расширение
// • Чтение из файла
//   Учесть отработку исключения UnauthorizedAccessException т.к. не во все директории вы сможете зайти.

using static System.Console;

public class CrawlerPathLogger
{
    private string[] DirsArr;
    private string FilePathLog;
    private string LogicDiskLetter;

    public string[] GetDirsArr()
    {
        return DirsArr;
    }

    public void SetDirsArr(string[] dirs)
    {
        DirsArr = dirs;
    }

    public string GetFilePathLog()
    {
        return FilePathLog;
    }

    public void SetFilePathLog(string filePath)
    {
        FilePathLog = filePath;
    }

    public string GetLogicDiskLetter()
    {
        return LogicDiskLetter;
    }

    public void SetLogicDiskLetter(string diskLetter)
    {
        LogicDiskLetter = diskLetter;
    }

    public void RecursiveDirectoryTraversal(string path)
    {
        try
        {
            DirsArr = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        }
        catch (UnauthorizedAccessException ex)
        {
            WriteLine($"Ошибка доступа к каталогу: {ex.Message}");
        }
    }

    public void WriteToLogFile()
    {
        using (StreamWriter sw = File.CreateText(FilePathLog))
        {
            foreach (string dir in DirsArr)
            {
                sw.WriteLine($"{DateTime.Now}, {dir}");

                string[] files = Directory.GetFiles(dir);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    sw.WriteLine($"    -> {fileInfo.CreationTime}, {fileInfo.Name}, {fileInfo.Extension}");
                }

                string[] subdirs = Directory.GetDirectories(dir);
                foreach (string subdir in subdirs)
                {
                    sw.WriteLine($"    -> {DateTime.Now}, {subdir}");

                    string[] subFiles = Directory.GetFiles(subdir);
                    foreach (string subFile in subFiles)
                    {
                        FileInfo subFileInfo = new FileInfo(subFile);
                        sw.WriteLine($"        -> {subFileInfo.CreationTime}, {subFileInfo.Name}, {subFileInfo.Extension}");
                    }
                }
            }
        }
    }

    public void ReadFromFile()
    {
        try
        {
            using (StreamReader sr = new StreamReader(FilePathLog))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            WriteLine($"Произошла ошибка при чтении файла: {ex.Message}");
        }
    }
}

class Program
{
    static void Main()
    {
        CrawlerPathLogger crawler = new CrawlerPathLogger();
        crawler.SetDirsArr(new string[] { "dir1", "dir2" });
        crawler.SetFilePathLog("logFile.txt");
        crawler.SetLogicDiskLetter("C://");
       
        string directoryPath = "C:\\AMD";  // Указать существующий путь к директории

        crawler.RecursiveDirectoryTraversal(directoryPath);
        crawler.WriteToLogFile();
        crawler.ReadFromFile();
    }
}