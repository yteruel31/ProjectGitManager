using System;
using Microsoft.Win32;

namespace PGM.Gitlab
{
    public class Registry
    {
        /// <summary>
        /// Checks if registry value exists.
        /// </summary>
        /// <param name="Hive"></param>
        /// <param name="Path"></param>
        /// <param name="ValueName"></param>
        /// <returns></returns>
        public static bool Check(RegistryHive Hive, string Path, string ValueName)
        {
            try
            {
                return (Read(Hive, Path, ValueName) != null);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads a value from the registry
        /// </summary>
        /// <param name="Hive"></param>
        /// <param name="Path"></param>
        /// <param name="ValueName"></param>
        /// <returns></returns>
        public static object Read(RegistryHive Hive, string Path, string ValueName)
        {
            RegistryKey Key = RegistryKey.OpenRemoteBaseKey(Hive, "");
            foreach (string k in Path.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                Key = Key.OpenSubKey(k);
            }
            return Key.GetValue(ValueName);
        }

        public static string[] GetAllSubkeys(RegistryHive Hive, string Path)
        {
            RegistryKey Key = RegistryKey.OpenRemoteBaseKey(Hive, "");

            foreach (string k in Path.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                Key = Key.OpenSubKey(k);
            }

            if (Key != null)
                return Key.GetSubKeyNames();
            else return null;
        }

        /// <summary>
        /// Writes a value to the registry
        /// </summary>
        /// <param name="Hive"></param>
        /// <param name="Path"></param>
        /// <param name="ValueName"></param>
        /// <param name="Value"></param>
        /// <param name="Kind"></param>
        public static void Write(RegistryHive Hive, string Path, string ValueName, object Value, RegistryValueKind Kind)
        {
            RegistryKey Key = RegistryKey.OpenRemoteBaseKey(Hive, "");

            string[] PathSplit = null;

            try
            {
                PathSplit = Path.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (string k in PathSplit)
                {
                    try
                    {
                        Key = Key.CreateSubKey(k);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Exception while opening " + Path + " at " + k + ": " + ex.Message, ex);
                    }
                }

                Key.SetValue(ValueName, Value, Kind);
                Key.Flush();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception while Opening " + Path + ": " + ex.Message, ex);
            }
        }
    }
}