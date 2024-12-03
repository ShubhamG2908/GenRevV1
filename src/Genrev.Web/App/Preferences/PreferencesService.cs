using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Genrev.Web.App.Preferences
{
    public class PreferencesService
    {

        private const string CHART_COLORS = "chart_colors";
        private Color[] defaultColors;

        public PreferencesService()
        {
            defaultColors = new Color[4]
            {
                ColorTranslator.FromHtml("#7eacb1"),
                ColorTranslator.FromHtml("#7e93b1"),
                ColorTranslator.FromHtml("#93b17e"),
                ColorTranslator.FromHtml("#acb17e")
            };
        }

        public Models.PreferencesVM GetPreferences()
        {
            int userID = AppService.Current.User.UserID;
            var opt = AppService.Current.DataContext.WebUserOptions.SingleOrDefault(x => 
                x.UserID == userID && x.OptionName == CHART_COLORS);


            Color[] colors = new Color[4];
            string[] customColors;
            if (opt != null)
            {
                customColors = opt.ValueRaw.Split(',');
            }
            else
            {
                customColors = new string[4] { "", "", "", "" };
            }

            for(int i = 0; i < colors.Length; i++)
            {
                if(customColors[i] != "")
                {
                    colors[i] = ColorTranslator.FromHtml(customColors[i]);
                }
                else
                {
                    colors[i] = new Color();
                }
            }

            Models.PreferencesVM model = new Models.PreferencesVM()
            {
                Color0 = colors[0],
                Color1 = colors[1],
                Color2 = colors[2],
                Color3 = colors[3]
            };

            return model;
        }

        public void SavePreferences(Models.PreferencesVM model)
        {
            string[] stringColors = new string[4];
            Color[] customColors = new Color[4];
            customColors[0] = model.Color0;
            customColors[1] = model.Color1;
            customColors[2] = model.Color2;
            customColors[3] = model.Color3;

            for(int i = 0; i < customColors.Length; i++)
            {
                if (!customColors[i].IsEmpty)
                {
                    stringColors[i] = ColorTranslator.ToHtml(customColors[i]);
                }
                else
                {
                    stringColors[i] = "";
                }
            }

            int userID = AppService.Current.User.UserID;
            string optionValue = String.Join(",", stringColors);
            var opt = AppService.Current.DataContext.WebUserOptions.SingleOrDefault(x =>
                x.UserID == userID && x.OptionName == CHART_COLORS);

            if(opt != null)
            {
                opt.ValueRaw = optionValue;
            }
            else
            {
                AppService.Current.DataContext.WebUserOptions.Add(new Domain.Users.WebUserOption()
                {
                    UserID = userID,
                    OptionName = CHART_COLORS,
                    ValueRaw = optionValue
                });
            }

            AppService.Current.DataContext.SaveChanges();
        }

    }
}