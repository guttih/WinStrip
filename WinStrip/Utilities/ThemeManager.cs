using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WinStrip.Entity;
using WinStrip.EntityTransfer;

namespace WinStrip.Utilities
{
    public class ThemeManager
    {
        private int selectedIndex;

        List<Theme> Themes { get; set; }
        public int SelectedIndex
        {
            get => selectedIndex; set
            {
                if (value < 0 || value > Count - 1)
                    selectedIndex = -1;
                else
                    selectedIndex = value;
            }
        }
        public int Count { get => Themes.Count; }

        /// <summary>
        /// True if the themes in memory are different from themes on disk
        /// </summary>
        public bool IsDirty { get; private set; }

        public ThemeManager()
        {
            Themes = new List<Theme>();
            SelectedIndex = -1;
        }

        /// <summary>
        /// Saves list of themes
        /// Note, current themes will not be changed.
        /// If you want that to happen, call Load() directly after calling this method.
        /// </summary>
        /// <param name="themes"></param>
        private void SaveThemes(List<Theme> themes)
        {
            //Sort all themes
            themes.Sort(new Theme());

            //Sort all steps in all themes so that highest From value will be first.
            themes.ForEach(theme => theme.SortStepsAndFix(true));
            var str = (new JavaScriptSerializer()).Serialize(themes);
            Properties.Settings.Default.Themes = str;
            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// Loads all themes.
        /// Note, if any themes have been changed but not saved, you will loose all changes.
        /// </summary>
        public void Load()
        {
            var str = Properties.Settings.Default.Themes;

            if (string.IsNullOrEmpty(str))
            {
                SaveThemes(CreateDefaultThemeList());
                str = Properties.Settings.Default.Themes;
            }

            SetThemes(str);
            IsDirty = false;
            if (SelectedIndex < 0 && Count > 0)
                SelectedIndex = 0;
        }

        internal void SetThemes(string str)
        {
            var ser = new JavaScriptSerializer();
            Themes = ser.Deserialize<List<Theme>>(str);
            SelectedIndex = GetDefaultIndex();
        }

        internal int GetDefaultIndex()
        {
            return Themes.FindIndex(t => t.Default == true);
        }

        internal bool IsThereADefaultTheme()
        {
            var index = GetDefaultIndex();
            return !(index > -1);
        }

        internal void AddNamesToComboBoxCollection(ComboBox.ObjectCollection items)
        {
            var names = Themes.Select(e => e.Name).ToArray();
            items.AddRange(names);
        }

        /// <summary>
        /// Gets the Name of the default theme
        /// </summary>
        /// <returns>Fail: null if no theme is default.  Success: string with name of the default theme</returns>
        internal string GetDefaultThemeName()
        {
            int index = GetDefaultIndex();

            if (index > -1)
                return Themes[index].Name;

            return null;
        }

        /// <summary>
        /// Gets name of the selected Theme
        /// </summary>
        /// <returns>Fail: null if No theme is selected. Success: string with the name</returns>
        internal string GetSelectedThemeName()
        {
            if (SelectedIndex > -1 && SelectedIndex < Count)
                return Themes[SelectedIndex].Name;

            return null;
        }

        internal Theme GetSelectedTheme()
        {
            if (SelectedIndex > -1 && SelectedIndex < Count)
                return Themes[SelectedIndex];

            return null;
        }

        /// <summary>
        /// Gets theme by name
        /// </summary>
        /// <param name="name">Theme name to search for</param>
        /// <returns>Fail: null if no theme is found with given name. Success: A Theme</returns>
        internal Theme GetThemeByName(string name)
        {
            var index = Themes.FindIndex(a => a.Name == name);
            if (index > -1)
                return Themes[index];
            return null;
        }

        /// <summary>
        /// Gets the index of a theme
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>Fail: -1 if no theme is found.  Success: index of the theme</returns>
        internal int IndexOfThemeByName(string name)
        {
            return Themes.FindIndex(a => a.Name == name);
        }

        /// <summary>
        /// Replaces selected theme with the given theme.
        /// Note, if no theme is selected, nothing happens.
        /// </summary>
        /// <param name="theme">The new theme</param>
        internal void ReplaceSelectedTheme(Theme theme)
        {
            if (SelectedIndex > -1 && SelectedIndex < Count)
                Themes[SelectedIndex] = theme;
        }

        internal List<Theme> CreateDefaultThemeList()
        {
            return new List<Theme> { CreateDefaultTheme(), CreateStepUpTheme() };
        }

        private Theme CreateDefaultTheme()
        {
            var theme = new Theme
            {
                Name = "Default",
                Steps = new List<Step> {
                    new Step { From = 0,  ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness =   1, values = new List<int> { 0, 0,0}, colors = new List<ulong> {      255, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 10, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 255, values = new List<int> { 0, 0,0}, colors = new List<ulong> {      255, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 30, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 177, values = new List<int> { 0, 0,0}, colors = new List<ulong> {    65535, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 50, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 240, values = new List<int> { 0, 0,0}, colors = new List<ulong> {    65280, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 60, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 180, values = new List<int> { 0, 0,0}, colors = new List<ulong> { 16711808, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 70, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 255, values = new List<int> { 0, 0,0}, colors = new List<ulong> { 16711680, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 90, ValuesAndColors = new StripValuesAndColors { delay=2, com=7, brightness = 102, values = new List<int> {32,50,0}, colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 95, ValuesAndColors = new StripValuesAndColors { delay=2, com=7, brightness = 255, values = new List<int> {32,20,0}, colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 99, ValuesAndColors = new StripValuesAndColors { delay=1, com=7, brightness = 255, values = new List<int> {32,20,0}, colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } } }
                }
            };

            return theme;
        }

        private Theme CreateStepUpTheme()
        {
            var theme = new Theme
            {
                Name = "Default StepUp",
                Steps = new List<Step>()
            };

            var step = new Step(0, "{\"delay\":500,\"com\":4,\"brightness\":10,\"values\":[1,0,0],\"colors\":[255,0,32768,255,16777215,10824234]}");
            //step 100 skal ver 1 í delay

            var stepDown = 6;
            step.ValuesAndColors.delay += stepDown;
            for (int i = 0; i < 81; i++)
            {
                step.From = i; step.ValuesAndColors.delay -= stepDown;
                theme.Steps.Add(new Step(step));
            }
            stepDown = 1;
            step.ValuesAndColors.delay += stepDown;
            for (int i = 80; i < 101; i++)
            {
                step.From = i; step.ValuesAndColors.delay -= stepDown;
                theme.Steps.Add(new Step(step));
            }

            return theme;
        }

        internal List<Theme> GetThemeList()
        {
            return Themes;
        }

        internal void AddTheme(Theme theme)
        {
            Themes.Add(theme);
            IsDirty = true;
        }

        /// <summary>
        /// Replaces a theme name
        /// </summary>
        /// <param name="oldName">Searches for a theme with this name</param>
        /// <param name="newName">The new name of the theme</param>
        /// <returns>True if a theme was found and it got a new name. False if nothing was replaced.</returns>
        internal bool ReplaceThemeName(string oldName, string newName)
        {
            if (oldName == newName)
                return false;

            int i = IndexOfThemeByName(oldName);
            if (i < 0)
                return false;

            Themes[i].Name = newName;
            IsDirty = true;

            return true;
        }

        /// <summary>
        /// Makes all themes not the default theme
        /// </summary>
        internal void MakeNoThemeDefault()
        {
            Themes.ForEach(theme => theme.Default = false);
        }

        /// <summary>
        /// Sets a theme the default theme
        /// </summary>
        /// <param name="index">The zero-based index of the theme to set default</param>
        internal void SetDefaultThemeAt(int index)
        {
            if (SelectedIndex > -1 && SelectedIndex < Count)
                Themes[index].Default = true;
        }

        internal void RemoveThemeAt(int i)
        {
            if (i == SelectedIndex)
                SelectedIndex = -1;
            Themes.RemoveAt(i);
            IsDirty = true;
        }

        internal bool SetSelectedThemeByName(string name)
        {
            int i = IndexOfThemeByName(name);
            if (i < 0)
                return false;

            SelectedIndex = i;
            return true;

        }

        internal void Save()
        {
            SaveThemes(Themes);
            IsDirty = false;
        }

        internal void ReplaceExistingOrAddNewStepsToTheme(Theme theme, List<Step> list)
        {
            var OverWrittenStepIndexes = new List<int>();
            theme.Steps.ForEach(old => {
                var i = list.Select(newItem => newItem.From).ToList().IndexOf(old.From);
                if (i > -1)
                {
                    old.ValuesAndColors = list[i].ValuesAndColors;
                    OverWrittenStepIndexes.Add(i);
                }
            });

            for (int i = 0; i < list.Count; i++)
            {
                if (!OverWrittenStepIndexes.Contains(i))
                {
                    theme.Steps.Add(list[i]);
                }
            }
        }
        public void AddThemeSafely(Theme addMe)
        {
            AddThemeSafely(Themes, addMe);
        }
        private void AddThemeSafely(List<Theme> list, Theme addMe)
        {
            int index, nameExtender;
            string testName;
            index = list.FindIndex(current => current.Name == addMe.Name);
            testName = addMe.Name;
            nameExtender = 1;
            while (index > -1)
            {
                nameExtender++;
                testName = addMe.Name + nameExtender;
                index = list.FindIndex(current => current.Name == testName);
            }

            addMe.Name = testName;
            list.Add(addMe);
        }
    }
}
