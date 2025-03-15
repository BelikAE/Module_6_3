using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_6_3
{
    public class LevelUtils
    {
        public static Level GetLevel(ExternalCommandData commandData, string nameLevel) 
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> listLevel = new FilteredElementCollector(doc)
           .OfClass(typeof(Level))
           .OfType<Level>()
           .ToList();

            Level level1 = listLevel
            .Where(x => x.Name.Equals(nameLevel))
            .FirstOrDefault();

            return level1;
        }
    }
}
