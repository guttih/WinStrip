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
        List<Theme> Themes { get; set; }
        public int SelectedThemeIndex { get; set; }
        public int Count { get =>  Themes.Count;  }

        
        public ThemeManager()
        {
            Themes = new List<Theme>();
            SelectedThemeIndex = -1;
        }


        internal void SetThemes(string str)
        {
            var ser = new JavaScriptSerializer();
            Themes = ser.Deserialize<List<Theme>>(str);
            SelectedThemeIndex = GetDefaultThemeIndex();
        }

        internal int GetDefaultThemeIndex()
        {
            return Themes.FindIndex(t => t.Default == true);
        }

        internal bool IsThereADefaultTheme()
        {
            var index = GetDefaultThemeIndex();
            return !(index > -1);
        }

        internal void AddNames(ComboBox.ObjectCollection items)
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
            int index = GetDefaultThemeIndex();

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
            if (SelectedThemeIndex > -1 && SelectedThemeIndex < Count)
                return Themes[SelectedThemeIndex].Name;

            return null;
        }

        internal Theme GetSelectedTheme()
        {
            if (SelectedThemeIndex > -1 && SelectedThemeIndex < Count)
                return Themes[SelectedThemeIndex];

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
            if (SelectedThemeIndex > -1 && SelectedThemeIndex < Count)
                Themes[SelectedThemeIndex] = theme;
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
        }

        /// <summary>
        /// Replaces a theme name
        /// </summary>
        /// <param name="oldName">Searches for a theme with this name</param>
        /// <param name="newName">The new name of the theme</param>
        /// <returns>True if a theme was found and it got a new name. False if nothing was replaced.</returns>
        internal bool ReplaceThemeName(string oldName, string newName)
        {
            int i = IndexOfThemeByName(oldName);
            if (i < 0)
                return false;
            Themes[i].Name = newName;

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
            if (SelectedThemeIndex > -1 && SelectedThemeIndex < Count)
                Themes[index].Default = true;
        }

        internal void RemoveThemeAt(int i)
        {
            if (i == SelectedThemeIndex)
                SelectedThemeIndex = -1;
            Themes.RemoveAt(i);
        }

        internal bool SetSelectedThemeByName(string name)
        {
            int i = IndexOfThemeByName(name);
            if (i < 0)
                return false;

            SelectedThemeIndex = i;
            return true;

        }
    }
}
