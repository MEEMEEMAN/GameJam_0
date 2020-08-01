using System;
using System.IO;
using Newtonsoft.Json;

namespace Perhaps
{
    /// <summary>
    /// An object connected to a file.
    /// Uses JSON for data storage.
    /// </summary>
    public sealed class FileObject<T>
    {
        [JsonIgnore]
        public T Object { get; set; }
        [JsonIgnore]
        public string FilePath { get; private set; } = null;
        [JsonIgnore]
        public string FileDirectory
        {
            get
            {
                if (FilePath == null)
                    return null;

                return FilePath.Substring(0, FilePath.LastIndexOf("\\"));
            }
        }


        [JsonIgnore]
        public string ErrorString { get; private set; }

        public FileObject(string _FilePath, T encapsulated)
        {
            FilePath = _FilePath;
            Object = encapsulated;
        }

        public FileObject(string _FilePath)
        {
            FilePath = _FilePath;
        }

        /// <summary>
        /// Saves the Object to disk.
        /// </summary>
        /// <returns>Whether the operation was succesfull.</returns>
        public bool Save()
        {
            try
            {
                string jsonObject = JsonConvert.SerializeObject(Object);
                File.WriteAllText(FilePath, jsonObject);

                return true;
            }
            catch
            {
                ErrorString = $"Failed to Save FileObject<{typeof(T).Name}> to path \"{FilePath}\"";
                return false;
            }
        }

        /// <summary>
        /// Loads the Object from disk.
        /// </summary>
        /// <returns>Whether the operation was succesfull.</returns>
        public bool Load()
        {
            try
            {
                string jsonObject = File.ReadAllText(FilePath);
                Object = JsonConvert.DeserializeObject<T>(jsonObject);

                return Object != null;
            }
            catch
            {
                ErrorString = $"Failed to Load FileObject<{typeof(T).Name}> from path \"{FilePath}\"";
                return false;
            }
        }
    }
}
