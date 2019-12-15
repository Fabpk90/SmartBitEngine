using System;
using System.Collections;

namespace DumBitEngine.Util
{
    public static class AssetLoader
    {
        private struct Asset
        {
            public IDisposable assetRef;
            public uint users;
        }
        
        private static Hashtable hashTable = new Hashtable();

        public static IDisposable GetElement(string key)
        {
            return ((Asset)hashTable[key]).assetRef;
        }

        public static IDisposable UseElement(string key)
        {
            if (hashTable.ContainsKey(key))
            {
                Asset ass = (Asset) hashTable[key];
                ass.users++;

                hashTable[key] = ass;
                return ass.assetRef;
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// Remove the asset from the table
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if the element is removed, false otherwise</returns>
        public static bool RemoveElement(string key)
        {
            if (hashTable.ContainsKey(key))
            {
                Asset elmnt = (Asset) hashTable[key];

                if (elmnt.users == 1)
                {
                    hashTable.Remove(key);
                    return true;
                }
                else
                {
                    elmnt.users--;
                    hashTable[key] = elmnt;

                    return false;
                }
            }
            else
            {
                return true;
            }
            
        }

        public static void AddElement(string key, IDisposable element)
        {
            if (!hashTable.ContainsKey(key))
            {
                Asset ass; //not a typo
                
                ass.users = 1;
                ass.assetRef = element;

                hashTable[key] = ass;
            }
        }
        
    }
}