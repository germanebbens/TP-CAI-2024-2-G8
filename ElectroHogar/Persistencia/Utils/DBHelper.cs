using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.Utils
{
    public class DBHelper
    {
        private string filePath;

        // Static constructor to set the default file path
        public DBHelper(String dataBaseName)
        {
            string solutionDirectory = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(solutionDirectory, dataBaseName + ".txt");

            // Create the file if it doesn't exist
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        // Method to insert a key-value pair
        public void Insertar(string key, string value)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{key}|{value}");
            }
        }

        // Method to modify the value of an existing key
        public void Modificar(string key, string newValue)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();
            bool modified = false;

            // Modify the line with the matching key
            for (int i = 0; i < lines.Count; i++)
            {
                string[] keyValue = lines[i].Split('|');
                if (keyValue[0] == key)
                {
                    lines[i] = $"{key}|{newValue}";
                    modified = true;
                    break;
                }
            }

            if (modified)
            {
                File.WriteAllLines(filePath, lines);
            }
        }

        // Method to delete a key-value pair by key
        public void Borrar(string key)
        {
            List<string> lines = File.ReadAllLines(filePath).ToList();

            // Remove the line with the matching key
            lines = lines.Where(line => line.Split('|')[0] != key).ToList();

            File.WriteAllLines(filePath, lines);
        }

        // Method to search for a value by key
        public string Buscar(string key)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                string[] keyValue = line.Split('|');
                if (keyValue[0] == key)
                {
                    return keyValue[1];
                }
            }

            return null; // Return null if not found
        }

        // Method to list all key-value pairs
        public List<string> Listar()
        {
            return File.ReadAllLines(filePath).ToList();
        }
    }
}
