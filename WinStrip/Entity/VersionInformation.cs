using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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

            foreach (var item in source)
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
            destination.Name = source.Name;
            destination.Version = source.Version;
            destination.Description = source.Description;
            destination.Released = source.Released;
            destination.Setup = source.Setup;
            destination.NewFeatures = Copy(source.NewFeatures);
            destination.BugFixes = Copy(source.BugFixes);
        }

        /// <summary>
        /// Compares current version string with another
        /// </summary>
        /// <param name="version">
        ///     This string must contain positive integers with dots between them f.example "1.1" or "1.1.1" or "1.1.1.1".  
        ///     Max number of integers are 4 and max number of dots are 3</param>
        /// <returns>
        ///      True: If this instance Version is equal to the given parameter. False: Otherwise
        /// </returns>
        ///
        public bool VersionIsEqual(string version)
        {
            return VersionCompareTo(version) == 0;
        }

        /// <summary>
        /// Compares current version string with another
        /// </summary>
        /// <param name="version">
        ///     This string must contain positive integers with dots between them f.example "1.1" or "1.1.1" or "1.1.1.1".  
        ///     Max number of integers are 4 and max number of dots are 3</param>
        /// <returns>
        ///      True: If this instance Version is equal or less than the given parameter. False: Otherwise
        /// </returns>
        ///
        public bool VersionIsEqualOrLess(string version)
        {
            return VersionCompareTo(version) <= 0;
        }

        /// <summary>
        /// Compares current version string with another
        /// </summary>
        /// <param name="version">
        ///     This string must contain positive integers with dots between them f.example "1.1" or "1.1.1" or "1.1.1.1".  
        ///     Max number of integers are 4 and max number of dots are 3</param>
        /// <returns>
        ///      True: If this instance Version is equal or greater than the given parameter. False: Otherwise
        /// </returns>
        ///
        public bool VersionIsEqualOrLarger(string version)
        {
            return VersionCompareTo(version) >= 0;
        }

        public bool VersionIsLess(string version)
        {
            return VersionCompareTo(version) < 0;
        }
        public bool VersionIsLarger(string version)
        {
            return VersionCompareTo(version) > 0;
        }


        /// <summary>
        /// Compares current version string with another
        /// </summary>
        /// <param name="version">
        ///     This string must contain positive integers with dots between them f.example "1.1" or "1.1.1" or "1.1.1.1".  
        ///     Max number of integers are 4 and max number of dots are 3</param>
        /// <returns>
        ///     -1: If this instance Version is less than the given parameter.
        ///      0: If this instance Version is equal to the given parameter.
        ///      1: If this instance Version is larger than the given parameter.
        /// </returns>
        /// 
        public int VersionCompareTo(string version)
        {
            var ver = new Version(version);
            var current = new Version(this.Version);
            return current.CompareTo(ver);
        }

    }
}
