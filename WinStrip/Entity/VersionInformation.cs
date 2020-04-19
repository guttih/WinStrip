using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinStrip.Entity
{
    public class VesionFeature
    {
        public string Title { get; set; }
        public string Description { get; set; }

        
    }
    public class VersionInformation
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Released { get; set; }
        public List<VesionFeature> NewFeatures { get; set; }
        public List<VesionFeature> BugFixes { get; set; }
        public string Setup { get; set; }

        private void ExtractMajorMinorIntegers(string versionString, ref int major, ref int minor)
        {
            int index = versionString.IndexOf('.');
            if (index < 1)
            {
                throw new ArgumentException($"Version string {versionString} is invalid");
            }
            string strMajor = versionString.Substring(0, index);
            string strMinor = versionString.Substring(index+1);
            int ma = Convert.ToInt32(strMajor);
            int mi = Convert.ToInt32(strMinor);
            major = ma;
            minor = mi;
        }

        /// <summary>
        /// Compares two version strings
        /// 
        /// Note the function throws an Invalid argument if parameters are invalid
        /// </summary>
        /// <param name="version1">This string must only contain two integers with a dot between them</param>
        /// <param name="version2">This string must only contain two integers with a dot between them</param>
        /// <returns>
        ///     -1: If version1 is less   than version 2.
        ///      0: If version1 is equal  than version 2.
        ///      1: If version1 is larger than version 2.
        /// </returns>
        /// 

        private int CompareMajorMinorVersionStrings(string version1, string version2)
        {
            int v1Major=0, v2Major=0, v1Minor=0, v2Minor=0;
            ExtractMajorMinorIntegers(version1, ref v1Major, ref v1Minor);
            ExtractMajorMinorIntegers(version2, ref v2Major, ref v2Minor);
            
            if (v1Major < v2Major)
                return -1;
            if (v1Major > v2Major)
                return 1;

            //Major are equal
            if (v1Minor < v2Minor)
                return -1;
            if (v1Minor > v2Minor)
                return 1;

            return 0;

        }

        /// <summary>
        /// Compares current version string with another
        /// 
        /// Note the function throws an Invalid argument if parameters are invalid
        /// </summary>
        /// <param name="version2">This string must only contain two integers with a dot between them</param>
        /// <returns>
        ///     -1: If this instance Version is less than the given parameter.
        ///      0: If this instance Version is equal  than the given parameter.
        ///      1: If this instance Version is larger than the given parameter.
        /// </returns>
        /// 
        public int CompareMajorMinorVersionStrings(string version)
        {
            return CompareMajorMinorVersionStrings(this.Version, version);
        }


    }
}
