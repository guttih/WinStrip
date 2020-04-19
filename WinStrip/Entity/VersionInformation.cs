using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WinStrip.Entity
{
    public class VersionNumbers
    {
        
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }

        public int Build { get; set; }

        public VersionNumbers() {}
        public VersionNumbers(string versionString)
        {
            Copy(ExtractIntegers(versionString), this);
        }

        /// <summary>
        /// Copies VersionNumbers values from one object to another
        /// </summary>
        /// <param name="left">Source object to copy values from</param>
        /// <param name="right">Destination object to copy values to</param>
        public void Copy(VersionNumbers left, VersionNumbers right)
        {
            right.Major = left.Major;
            right.Minor = left.Minor;
            right.Patch = left.Patch;
            right.Build = left.Build;
        }

        public bool Equal(VersionNumbers compareToMe)
        {
            if (compareToMe == null)
                return false;

            if (this == compareToMe)
                return true;

            return Compare(compareToMe) == 0;
        }

        public VersionNumbers(VersionNumbers right)
        {
            Copy(right, this);
        }
        public VersionNumbers ExtractIntegers(string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
            {
                throw new ArgumentException($"Version string is null or empty");
            }
            var ver = new VersionNumbers();
            var numbers = versionString.Split('.');

            for (var i = 0; i<numbers.Length; i++)
            {
                string strNumber = numbers[i];
                try
                {
                    int value = Convert.ToInt32(strNumber);
                    switch (i)
                    {
                        case 0: ver.Major = value; break;
                        case 1: ver.Minor = value; break;
                        case 2: ver.Patch = value; break;
                        case 3: ver.Build = value; break;
                        default: throw new ArgumentException();
                    }
                }
                catch
                {
                    var parts = new string[] {"Major", "Minor", "Patch", "Build" };
                    throw new ArgumentException($"{parts} part of the version string \"{strNumber}\"is invalid.");
                }

            }

            return ver;
        }

        public override string ToString() 
        { 
            return $"{Major}.{Minor}.{Patch}.{Build}"; 
        }

        /// <summary>
        /// Checks if insance is larger than another VersionNumbers object.
        /// </summary>
        /// <param name="compareToMe">the VersionNumbers to compare to instance</param>
        /// <returns>
        /// True  if Instance is Larger than compareToMe
        /// False if Instance is NOT Larger than compareToMe
        /// </returns>
        public bool Larger(VersionNumbers compareToMe)
        {
            return Compare(compareToMe) == 1;
        }

        /// <summary>
        /// Checks if insance is less than another VersionNumbers object.
        /// </summary>
        /// <param name="compareToMe">the VersionNumbers to compare to instance</param>
        /// <returns>
        /// True  if Instance is less than compareToMe
        /// False if Instance is NOT less than compareToMe
        /// </returns>
        public bool Less(VersionNumbers compareToMe)
        {
            return Compare(compareToMe) == -1;

        }

        /// <summary>
        /// Compares a instance with another VersionNumbers object.
        /// </summary>
        /// <param name="compareToMe">the VersionNumbers to compare to instance</param>
        /// <returns>
        ///  1: if instance is larger than compareWithMe.
        /// -1: if instance is less   than compareWithMe.
        ///  0: if instance is equal  to   compareWithMe.
        /// </returns>
        public int Compare(VersionNumbers compareToMe)
        {
            // Major
            if (Major < compareToMe.Major)
                return -1;
            if (Major > compareToMe.Major)
                return 1;

            // Minor
            if (Minor < compareToMe.Minor)
                return -1;
            if (Minor > compareToMe.Minor)
                return 1;

            // Patch
            if (Patch < compareToMe.Patch)
                return -1;
            if (Patch > compareToMe.Patch)
                return 1;

            // Build
            if (Build < compareToMe.Build)
                return -1;
            if (Build > compareToMe.Build)
                return 1;

            // Equal
            return 0;
        }
    }
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

        public VersionInformation() { }
        public VersionInformation(string jsonVersionStringObject)
        {
            var serializer = new JavaScriptSerializer();
            var versionInfo = serializer.Deserialize<VersionInformation>(jsonVersionStringObject);
            Copy(versionInfo, this);

        }

        private List<VesionFeature> Copy(List<VesionFeature> source)
        {
            if (source == null)
                return null;
            
            var list = new List<VesionFeature>();

            foreach(var item in source)
                list.Add(item);

            return list;
        }

        /// <summary>
        /// Copies all values from destination to source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void Copy(VersionInformation source, VersionInformation destination)
        {
            destination.Name        = source.Name;
            destination.Version     = source.Version;
            destination.Description = source.Description;
            destination.Released    = source.Released;
            destination.Setup       = source.Setup;
            destination.NewFeatures = Copy(source.NewFeatures);
            destination.BugFixes    = Copy(source.BugFixes);
        }

        /// <summary>
        /// Checks if insance is less than another VersionNumbers object.
        /// </summary>
        /// <param name="compareToMe">the VersionNumbers to compare to instance</param>
        /// <returns>
        /// True  if Instance is less than compareToMe
        /// False if Instance is NOT less than compareToMe
        /// </returns>
        public bool IsVersionLess(string compareToMe)
        {
            var ver = new VersionNumbers(Version);
            return ver.Less(new VersionNumbers(compareToMe));
        }

        /// <summary>
        /// Checks if insance is larger than another VersionNumbers object.
        /// </summary>
        /// <param name="compareToMe">the VersionNumbers to compare to instance</param>
        /// <returns>
        /// True  if Instance is Larger than compareToMe
        /// False if Instance is NOT Larger than compareToMe
        /// </returns>
        public bool IsVersionLarger(string compareToMe)
        {
            var ver = new VersionNumbers(Version);
            return ver.Larger(new VersionNumbers(compareToMe));
        }
        public bool IsVersionEqual(string compareToMe)
        {
            var ver = new VersionNumbers(Version);
            return ver.Equal(new VersionNumbers(compareToMe));
        }

        /// <summary>
        /// Compares a instance with another VersionNumbers object.
        /// </summary>
        /// <param name="compareToMe">the VersionNumbers to compare to instance</param>
        /// <returns>
        ///  1: if instance is larger than compareWithMe.
        /// -1: if instance is less   than compareWithMe.
        ///  0: if instance is equal  to   compareWithMe.
        /// </returns>
        public int VersionCompare(string compareToMe)
        {
            var ver = new VersionNumbers(Version);
            return ver.Compare(new VersionNumbers(compareToMe));
        }
    }
}
