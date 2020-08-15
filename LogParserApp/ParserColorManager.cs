using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using static Entities.Enums;

namespace LogParserApp
{
    public class ParserColorManager
    {
        private string[] _baseColorTable;
        private float _colorCorrectionFactorPercent = 0;
        private int _currentBaseColorIndex = -1;
        public Color BaseColor { get; private set; }

        public ParserColorManager()
        {
            var colors = ConfigurationManager.AppSettings["Colors"];
            if (colors != null)
                _baseColorTable = colors.Split(',');
            else
            {

                _baseColorTable = new string[]
                {
                    "#87cefa", //blue
                    "#F9524A", //red
                    "#37FB02", //green
                    "#FB00FF", //magenta
                    "#EDD2FA", //faux-pale lavende
                    "#D2FAFA", //cyan
                };
            }

            BaseColor = ColorTranslator.FromHtml(_baseColorTable[0]);
            _colorCorrectionFactorPercent = (float)Utils.GetConfigValue<float>("ColorCorrectionFactorPercent");
            if (_colorCorrectionFactorPercent == 0)
                _colorCorrectionFactorPercent = 10;
        }

        private static Color DarkerColor(Color color, float correctionfactor = 10f)
        {
            const float hundredpercent = 100f;
            correctionfactor = hundredpercent - correctionfactor;
            return Color.FromArgb((int)((color.R / hundredpercent) * correctionfactor),
                (int)((color.G / hundredpercent) * correctionfactor), (int)((color.B / hundredpercent) * correctionfactor));
        }

        private static Color LighterColor(Color color, float correctionfactor = 10f)
        {
            correctionfactor = correctionfactor / 100f;
            const float rgb255 = 255f;
            return Color.FromArgb((int)(color.R + ((rgb255 - color.R) * correctionfactor)), (int)(color.G + ((rgb255 - color.G) * correctionfactor)), (int)(color.B + ((rgb255 - color.B) * correctionfactor))
                );
        }

        public Color GetNextBaseColor()
        {           
            if (_currentBaseColorIndex == -1 || _currentBaseColorIndex == _baseColorTable.Length - 1)
            {
                BaseColor = ColorTranslator.FromHtml(_baseColorTable[0]);
                _currentBaseColorIndex = 0;
            }
            else
            {
                BaseColor = ColorTranslator.FromHtml(_baseColorTable[_currentBaseColorIndex]);
                _currentBaseColorIndex++;
            }
            return BaseColor;
        }    

        public Color GetColorByState(Color baseColor, State state)
        {
            Color result = baseColor;            
            if (result == Color.Transparent)
                 result = BaseColor;              

            if (state > 0)
                result = DarkerColor(baseColor, _colorCorrectionFactorPercent * (int)state);

            return result;
        }
    }
}
